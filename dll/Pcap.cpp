
#include <MsRdpEx/Pcap.h>

#ifndef _WIN32
#include <sys/time.h>
#else
#include <time.h>
#include <sys/timeb.h>

static int gettimeofday(struct timeval* tp, void* tz)
{
    struct _timeb timebuffer;
    _ftime(&timebuffer);
    tp->tv_sec = (long)timebuffer.time;
    tp->tv_usec = timebuffer.millitm * 1000;
    return 0;
}
#endif

struct _MsRdpEx_Pcap
{
    FILE* fp;
    char* name;
    bool write;
    int64_t file_size;
    int record_count;
    PCAP_FILE_HEADER header;
    PCAP_RECORD* head;
    PCAP_RECORD* tail;
    PCAP_RECORD* record;
};

#define PCAP_MAGIC_NUMBER 0xA1B2C3D4

static bool MsRdpEx_PcapFile_ReadHeader(MsRdpEx_PcapFile* pcap, PCAP_FILE_HEADER* header)
{
    return fread((void*)header, sizeof(PCAP_FILE_HEADER), 1, pcap->fp) == 1;
}

static bool MsRdpEx_PcapFile_WriteHeader(MsRdpEx_PcapFile* pcap, PCAP_FILE_HEADER* header)
{
    return fwrite((void*)header, sizeof(PCAP_FILE_HEADER), 1, pcap->fp) == 1;
}

static bool MsRdpEx_PcapFile_ReadRecordHeader(MsRdpEx_PcapFile* pcap, PCAP_RECORD_HEADER* record)
{
    return fread((void*)record, sizeof(PCAP_RECORD_HEADER), 1, pcap->fp) == 1;
}

static bool MsRdpEx_PcapFile_WriteRecordHeader(MsRdpEx_PcapFile* pcap, PCAP_RECORD_HEADER* record)
{
    return fwrite((void*)record, sizeof(PCAP_RECORD_HEADER), 1, pcap->fp) == 1;
}

static bool MsRdpEx_PcapFile_ReadRecord(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record)
{
    if (!MsRdpEx_PcapFile_ReadRecordHeader(pcap, &record->header))
        return false;

    record->length = record->header.incl_len;
    record->data = malloc(record->length);

    if (!record->data)
        return false;

    if (fread(record->data, record->length, 1, pcap->fp) != 1)
    {
        free(record->data);
        record->data = NULL;
        return false;
    }

    return true;
}

static bool MsRdpEx_PcapFile_WriteRecord(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record)
{
    return MsRdpEx_PcapFile_WriteRecordHeader(pcap, &record->header) &&
           (fwrite(record->cdata, record->length, 1, pcap->fp) == 1);
}

bool MsRdpEx_PcapFile_AddRecord(MsRdpEx_PcapFile* pcap, const uint8_t* data, uint32_t length)
{
    struct timeval tp;
    PCAP_RECORD* record = NULL;

    if (!pcap->tail)
    {
        pcap->tail = (PCAP_RECORD*) calloc(1, sizeof(PCAP_RECORD));

        if (!pcap->tail)
            return false;

        pcap->head = pcap->tail;
        pcap->record = pcap->head;
        record = pcap->tail;
    }
    else
    {
        record = (PCAP_RECORD*) calloc(1, sizeof(PCAP_RECORD));

        if (!record)
            return false;

        pcap->tail->next = record;
        pcap->tail = record;
    }

    if (!pcap->record)
        pcap->record = record;

    record->cdata = data;
    record->length = length;
    record->header.incl_len = length;
    record->header.orig_len = length;

    gettimeofday(&tp, 0);
    record->header.ts_sec = tp.tv_sec;
    record->header.ts_usec = tp.tv_usec;

    return true;
}

bool MsRdpEx_PcapFile_HasNextRecord(MsRdpEx_PcapFile* pcap)
{
    if (pcap->file_size - (MsRdpEx_FileTell(pcap->fp)) <= 16)
        return false;

    return true;
}

bool MsRdpEx_PcapFile_GetNextRecordHeader(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record)
{
    if (MsRdpEx_PcapFile_HasNextRecord(pcap) != true)
        return false;

    MsRdpEx_PcapFile_ReadRecordHeader(pcap, &record->header);
    record->length = record->header.incl_len;

    return true;
}

bool MsRdpEx_PcapFile_GetNextRecordContent(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record)
{
    return fread(record->data, record->length, 1, pcap->fp) == 1;
}

bool MsRdpEx_PcapFile_GetNextRecord(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record)
{
    return MsRdpEx_PcapFile_HasNextRecord(pcap) && MsRdpEx_PcapFile_ReadRecord(pcap, record);
}

MsRdpEx_PcapFile* MsRdpEx_PcapFile_Open(const char* name, bool write)
{
    MsRdpEx_PcapFile* pcap;

    pcap = (MsRdpEx_PcapFile*) calloc(1, sizeof(MsRdpEx_PcapFile));
    
    if (!pcap)
        goto fail;

    pcap->name = _strdup(name);
    pcap->write = write;
    pcap->record_count = 0;
    pcap->fp = MsRdpEx_FileOpen(name, write ? "w+b" : "rb");

    if (!pcap->fp)
        goto fail;

    if (write)
    {
        pcap->header.magic_number = PCAP_MAGIC_NUMBER;
        pcap->header.version_major = 2;
        pcap->header.version_minor = 4;
        pcap->header.thiszone = 0;
        pcap->header.sigfigs = 0;
        pcap->header.snaplen = 0xFFFFFFFF;
        pcap->header.network = 0;

        if (!MsRdpEx_PcapFile_WriteHeader(pcap, &pcap->header))
            goto fail;
    }
    else
    {
        MsRdpEx_FileSeek(pcap->fp, 0, SEEK_END);
        pcap->file_size = MsRdpEx_FileTell(pcap->fp);
        MsRdpEx_FileSeek(pcap->fp, 0, SEEK_SET);

        if (!MsRdpEx_PcapFile_ReadHeader(pcap, &pcap->header))
            goto fail;
    }

    return pcap;

fail:
    MsRdpEx_PcapFile_Close(pcap);
    return NULL;
}

void MsRdpEx_PcapFile_Flush(MsRdpEx_PcapFile* pcap)
{
    while (pcap->record)
    {
        MsRdpEx_PcapFile_WriteRecord(pcap, pcap->record);
        pcap->record = pcap->record->next;
    }

    if (pcap->fp)
        fflush(pcap->fp);
}

void MsRdpEx_PcapFile_Close(MsRdpEx_PcapFile* pcap)
{
    if (!pcap)
        return;

    MsRdpEx_PcapFile_Close(pcap);

    if (pcap->fp)
        fclose(pcap->fp);

    free(pcap->name);
    free(pcap);
}

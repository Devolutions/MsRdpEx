
#include <MsRdpEx/Stream.h>

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
    bool synchronized;
    CRITICAL_SECTION lock;
    uint32_t inboundSeqNo;
    uint32_t outboundSeqNo;
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

static bool MsRdpEx_PcapFile_WriteRecordData(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record)
{
    return fwrite(record->cdata, record->length, 1, pcap->fp) == 1;
}

static bool MsRdpEx_PcapFile_WriteRecord(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record)
{
    return MsRdpEx_PcapFile_WriteRecordHeader(pcap, &record->header) &&
        MsRdpEx_PcapFile_WriteRecordData(pcap, record);
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

void MsRdpEx_PcapFile_Lock(MsRdpEx_PcapFile* pcap)
{
    if (pcap->synchronized)
        EnterCriticalSection(&pcap->lock);
}

void MsRdpEx_PcapFile_Unlock(MsRdpEx_PcapFile* pcap)
{
    if (pcap->synchronized)
        LeaveCriticalSection(&pcap->lock);
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
    pcap->synchronized = true;

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
        pcap->header.network = 1; // ethernet

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

    if (!InitializeCriticalSectionAndSpinCount(&pcap->lock, 4000))
        goto fail;

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

    DeleteCriticalSection(&pcap->lock);

    free(pcap);
}

static bool MsRdpEx_PcapFile_WriteEthernetHeader(MsRdpEx_PcapFile* pcap, PCAP_ETHERNET_HEADER* ethernet)
{
    MsRdpEx_Stream* s;
    uint8_t buffer[14];
    bool success = true;

    if (!pcap || !pcap->fp || !ethernet)
        return false;

    ZeroMemory(buffer, sizeof(buffer));

    s = MsRdpEx_Stream_New(buffer, 14, false);

    if (!s)
        return false;

    MsRdpEx_StreamWrite(s, ethernet->Destination, 6);
    MsRdpEx_StreamWrite(s, ethernet->Source, 6);
    MsRdpEx_StreamWrite_UINT16_BE(s, ethernet->Type);

    if (fwrite(buffer, 14, 1, pcap->fp) != 1)
        success = false;
    
    MsRdpEx_Stream_Free(s);

    return success;
}

static uint16_t IPv4Checksum(uint8_t* ipv4, int length)
{
    uint16_t tmp16;
    uint32_t checksum = 0;

    while (length > 1)
    {
        tmp16 = *((uint16_t*)ipv4);
        checksum += tmp16;
        length -= 2;
        ipv4 += 2;
    }

    if (length > 0)
        checksum += *ipv4;

    while (checksum >> 16)
        checksum = (checksum & 0xFFFF) + (checksum >> 16);

    return (uint16_t)(~checksum);
}

static bool MsRdpEx_PcapFile_WriteIPv4Header(MsRdpEx_PcapFile* pcap, PCAP_IPV4_HEADER* ipv4)
{
    MsRdpEx_Stream* s;
    uint8_t buffer[20];
    int success = true;

    if (!pcap || !pcap->fp || !ipv4)
        return false;

    ZeroMemory(buffer, sizeof(buffer));

    s = MsRdpEx_Stream_New(buffer, 20, false);
    
    if (!s)
        return false;

    MsRdpEx_StreamWrite_UINT8(s, (ipv4->Version << 4) | ipv4->InternetHeaderLength);
    MsRdpEx_StreamWrite_UINT8(s, ipv4->TypeOfService);
    MsRdpEx_StreamWrite_UINT16_BE(s, ipv4->TotalLength);
    MsRdpEx_StreamWrite_UINT16_BE(s, ipv4->Identification);
    MsRdpEx_StreamWrite_UINT16_BE(s, (ipv4->InternetProtocolFlags << 13) | ipv4->FragmentOffset);
    MsRdpEx_StreamWrite_UINT8(s, ipv4->TimeToLive);
    MsRdpEx_StreamWrite_UINT8(s, ipv4->Protocol);
    MsRdpEx_StreamWrite_UINT16(s, ipv4->HeaderChecksum);
    MsRdpEx_StreamWrite_UINT32_BE(s, ipv4->SourceAddress);
    MsRdpEx_StreamWrite_UINT32_BE(s, ipv4->DestinationAddress);
    ipv4->HeaderChecksum = IPv4Checksum((uint8_t*)buffer, 20);
    MsRdpEx_StreamRewind(s, 10);
    MsRdpEx_StreamWrite_UINT16(s, ipv4->HeaderChecksum);
    MsRdpEx_StreamSeek(s, 8);

    if (fwrite(buffer, 20, 1, pcap->fp) != 1)
        success = false;
  
    MsRdpEx_Stream_Free(s);
    return success;
}

static bool MsRdpEx_PcapFile_WriteTcpHeader(MsRdpEx_PcapFile* pcap, PCAP_TCP_HEADER* tcp)
{
    MsRdpEx_Stream* s;
    uint8_t buffer[20];
    bool success = true;

    if (!pcap || !pcap->fp || !tcp)
        return false;

    ZeroMemory(buffer, sizeof(buffer));

    s = MsRdpEx_Stream_New(buffer, 20, false);
    
    if (!s)
        return false;

    MsRdpEx_StreamWrite_UINT16_BE(s, tcp->SourcePort);
    MsRdpEx_StreamWrite_UINT16_BE(s, tcp->DestinationPort);
    MsRdpEx_StreamWrite_UINT32_BE(s, tcp->SequenceNumber);
    MsRdpEx_StreamWrite_UINT32_BE(s, tcp->AcknowledgementNumber);
    MsRdpEx_StreamWrite_UINT8(s, (tcp->Offset << 4) | tcp->Reserved);
    MsRdpEx_StreamWrite_UINT8(s, tcp->TcpFlags);
    MsRdpEx_StreamWrite_UINT16_BE(s, tcp->Window);
    MsRdpEx_StreamWrite_UINT16_BE(s, tcp->Checksum);
    MsRdpEx_StreamWrite_UINT16_BE(s, tcp->UrgentPointer);

    if (fwrite(buffer, 20, 1, pcap->fp) != 1)
        success = false;

    MsRdpEx_Stream_Free(s);
    return success;
}

bool MsRdpEx_PcapFile_WritePacket(MsRdpEx_PcapFile* pcap, const uint8_t* data, size_t length, uint32_t flags)
{
    PCAP_TCP_HEADER tcp;
    PCAP_IPV4_HEADER ipv4;
    struct timeval tp;
    PCAP_RECORD record;
    PCAP_ETHERNET_HEADER ethernet;

    ZeroMemory(&tcp, sizeof(PCAP_TCP_HEADER));
    ZeroMemory(&ipv4, sizeof(PCAP_IPV4_HEADER));
    ZeroMemory(&ethernet, sizeof(PCAP_ETHERNET_HEADER));

    ethernet.Type = 0x0800;

    if (!pcap || !pcap->fp)
        return false;

    if (flags & PCAP_PACKET_FLAG_OUTBOUND)
    {
        /* 00:15:5D:01:64:04 */
        ethernet.Source[0] = 0x00;
        ethernet.Source[1] = 0x15;
        ethernet.Source[2] = 0x5D;
        ethernet.Source[3] = 0x01;
        ethernet.Source[4] = 0x64;
        ethernet.Source[5] = 0x04;
        /* 00:15:5D:01:64:01 */
        ethernet.Destination[0] = 0x00;
        ethernet.Destination[1] = 0x15;
        ethernet.Destination[2] = 0x5D;
        ethernet.Destination[3] = 0x01;
        ethernet.Destination[4] = 0x64;
        ethernet.Destination[5] = 0x01;
    }
    else
    {
        /* 00:15:5D:01:64:01 */
        ethernet.Source[0] = 0x00;
        ethernet.Source[1] = 0x15;
        ethernet.Source[2] = 0x5D;
        ethernet.Source[3] = 0x01;
        ethernet.Source[4] = 0x64;
        ethernet.Source[5] = 0x01;
        /* 00:15:5D:01:64:04 */
        ethernet.Destination[0] = 0x00;
        ethernet.Destination[1] = 0x15;
        ethernet.Destination[2] = 0x5D;
        ethernet.Destination[3] = 0x01;
        ethernet.Destination[4] = 0x64;
        ethernet.Destination[5] = 0x04;
    }

    ipv4.Version = 4;
    ipv4.InternetHeaderLength = 5;
    ipv4.TypeOfService = 0;
    ipv4.TotalLength = (uint16_t)(length + 20 + 20);
    ipv4.Identification = 0;
    ipv4.InternetProtocolFlags = 0x02;
    ipv4.FragmentOffset = 0;
    ipv4.TimeToLive = 128;
    ipv4.Protocol = 6; /* TCP */
    ipv4.HeaderChecksum = 0;

    if (flags & PCAP_PACKET_FLAG_OUTBOUND)
    {
        ipv4.SourceAddress = 0xC0A80196;      /* 192.168.1.150 */
        ipv4.DestinationAddress = 0x4A7D64C8; /* 74.125.100.200 */
    }
    else
    {
        ipv4.SourceAddress = 0x4A7D64C8;      /* 74.125.100.200 */
        ipv4.DestinationAddress = 0xC0A80196; /* 192.168.1.150 */
    }

    tcp.SourcePort = 3389;
    tcp.DestinationPort = 3389;

    if (flags & PCAP_PACKET_FLAG_OUTBOUND)
    {
        tcp.SequenceNumber = pcap->outboundSeqNo;
        tcp.AcknowledgementNumber = pcap->inboundSeqNo;
        pcap->outboundSeqNo += length;
    }
    else
    {
        tcp.SequenceNumber = pcap->inboundSeqNo;
        tcp.AcknowledgementNumber = pcap->outboundSeqNo;
        pcap->inboundSeqNo += length;
    }

    tcp.Offset = 5;
    tcp.Reserved = 0;
    tcp.TcpFlags = 0x0018;
    tcp.Window = 0x7FFF;
    tcp.Checksum = 0;
    tcp.UrgentPointer = 0;
    record.data = (void*) data;
    record.length = length;
    record.header.incl_len = record.length + 14 + 20 + 20;
    record.header.orig_len = record.length + 14 + 20 + 20;
    record.next = NULL;

    gettimeofday(&tp, 0);
    record.header.ts_sec = tp.tv_sec;
    record.header.ts_usec = tp.tv_usec;

    if (!MsRdpEx_PcapFile_WriteRecordHeader(pcap, &record.header) ||
        !MsRdpEx_PcapFile_WriteEthernetHeader(pcap, &ethernet) ||
        !MsRdpEx_PcapFile_WriteIPv4Header(pcap, &ipv4) ||
        !MsRdpEx_PcapFile_WriteTcpHeader(pcap, &tcp) ||
        !MsRdpEx_PcapFile_WriteRecordData(pcap, &record)) {
        return false;
    }

    return true;
}

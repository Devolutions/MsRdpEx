#ifndef MSRDPEX_PCAP_H
#define MSRDPEX_PCAP_H

#include <MsRdpEx/MsRdpEx.h>

struct _pcap_file_header
{
    uint32_t magic_number;  /* magic number */
    uint16_t version_major; /* major version number */
    uint16_t version_minor; /* minor version number */
    int32_t thiszone;       /* GMT to local correction */
    uint32_t sigfigs;       /* accuracy of timestamps */
    uint32_t snaplen;       /* max length of captured packets, in octets */
    uint32_t network;       /* data link type */
};
typedef struct _pcap_file_header PCAP_FILE_HEADER;

struct _pcap_record_header
{
    uint32_t ts_sec;   /* timestamp seconds */
    uint32_t ts_usec;  /* timestamp microseconds */
    uint32_t incl_len; /* number of octets of packet saved in file */
    uint32_t orig_len; /* actual length of packet */
};
typedef struct _pcap_record_header PCAP_RECORD_HEADER;

typedef struct _pcap_record PCAP_RECORD;

struct _pcap_record
{
    PCAP_RECORD_HEADER header;
    union
    {
        void* data;
        const void* cdata;
    };
    uint32_t length;
    PCAP_RECORD* next;
};

typedef struct _MsRdpEx_Pcap MsRdpEx_PcapFile;

#ifdef __cplusplus
extern "C" {
#endif

MsRdpEx_PcapFile* MsRdpEx_PcapFile_Open(const char* name, bool write);
void MsRdpEx_PcapFile_Close(MsRdpEx_PcapFile* pcap);

bool MsRdpEx_PcapFile_AddRecord(MsRdpEx_PcapFile* pcap, const uint8_t* data, uint32_t length);
bool MsRdpEx_PcapFile_HasNextRecord(MsRdpEx_PcapFile* pcap);
bool MsRdpEx_PcapFile_GetNextRecord(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record);
bool MsRdpEx_PcapFile_GetNextRecordHeader(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record);
bool MsRdpEx_PcapFile_GetNextRecordContent(MsRdpEx_PcapFile* pcap, PCAP_RECORD* record);
void MsRdpEx_PcapFile_Flush(MsRdpEx_PcapFile* pcap);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_PCAP_H

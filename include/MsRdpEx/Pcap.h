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

#pragma pack(push, 1)

struct _PCAP_ETHERNET_HEADER
{
    uint8_t Destination[6];
    uint8_t Source[6];
    uint16_t Type;
};
typedef struct _PCAP_ETHERNET_HEADER PCAP_ETHERNET_HEADER;

struct _PCAP_IPV4_HEADER
{
    uint8_t Version;
    uint8_t InternetHeaderLength;
    uint8_t TypeOfService;
    uint16_t TotalLength;
    uint16_t Identification;
    uint8_t InternetProtocolFlags;
    uint16_t FragmentOffset;
    uint8_t TimeToLive;
    uint8_t Protocol;
    uint16_t HeaderChecksum;
    uint32_t SourceAddress;
    uint32_t DestinationAddress;
};
typedef struct _PCAP_IPV4_HEADER PCAP_IPV4_HEADER;

struct _PCAP_TCP_HEADER
{
    uint16_t SourcePort;
    uint16_t DestinationPort;
    uint32_t SequenceNumber;
    uint32_t AcknowledgementNumber;
    uint8_t Offset;
    uint8_t Reserved;
    uint8_t TcpFlags;
    uint16_t Window;
    uint16_t Checksum;
    uint16_t UrgentPointer;
};
typedef struct _PCAP_TCP_HEADER PCAP_TCP_HEADER;

#pragma pack(pop)

#define PCAP_PACKET_FLAG_INBOUND        0x00000001
#define PCAP_PACKET_FLAG_OUTBOUND       0x00000002

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

void MsRdpEx_PcapFile_Lock(MsRdpEx_PcapFile* pcap);
void MsRdpEx_PcapFile_Unlock(MsRdpEx_PcapFile* pcap);

bool MsRdpEx_PcapFile_WritePacket(MsRdpEx_PcapFile* pcap, const uint8_t* data, size_t length, uint32_t flags);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_PCAP_H

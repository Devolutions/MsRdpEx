
add_library(MsRdpEx_Core STATIC
    ArrayList.c
    HashTable.c
    NameResolver.c
    File.c
    Log.c
    Paths.c
    RdpFile.c
    RdpSession.c
    String.c)

target_link_libraries(MsRdpEx_Core detours version.lib)

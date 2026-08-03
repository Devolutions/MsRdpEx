add_executable(MsRdpEx_AxBackend_Test
    AxBackendTests.cpp
    ../AxBackend.cpp)

target_include_directories(MsRdpEx_AxBackend_Test PRIVATE "${CMAKE_CURRENT_SOURCE_DIR}/..")
target_compile_options(MsRdpEx_AxBackend_Test PRIVATE /W4)

add_test(NAME MsRdpEx_AxBackend_Test COMMAND MsRdpEx_AxBackend_Test)

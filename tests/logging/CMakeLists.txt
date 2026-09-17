add_executable(MsRdpEx_LoggingTest LoggingTest.cpp)
target_compile_features(MsRdpEx_LoggingTest PRIVATE cxx_std_17)
add_executable(MsRdpEx_GatewayIsolationTest GatewayIsolationTest.cpp)
target_compile_features(MsRdpEx_GatewayIsolationTest PRIVATE cxx_std_17)
target_compile_options(MsRdpEx_GatewayIsolationTest PRIVATE /W4 /EHsc)
target_link_libraries(MsRdpEx_GatewayIsolationTest PRIVATE detours rpcrt4.lib)
add_test(NAME logging.gateway-isolation COMMAND MsRdpEx_GatewayIsolationTest)
add_test(NAME logging.gateway-isolation.startup-off COMMAND MsRdpEx_GatewayIsolationTest off)
set_tests_properties(logging.gateway-isolation logging.gateway-isolation.startup-off PROPERTIES TIMEOUT 45)
add_executable(MsRdpEx_GatewayCoreApiTest GatewayCoreApiTest.cpp)
target_compile_features(MsRdpEx_GatewayCoreApiTest PRIVATE cxx_std_17)
target_compile_options(MsRdpEx_GatewayCoreApiTest PRIVATE /W4 /EHsc)
add_dependencies(MsRdpEx_GatewayCoreApiTest MsRdpEx_Dll)
foreach(mode default off hooks-off)
    add_test(NAME logging.gateway-core-api.${mode}
        COMMAND MsRdpEx_GatewayCoreApiTest "$<TARGET_FILE:MsRdpEx_Dll>" ${mode})
    set_tests_properties(logging.gateway-core-api.${mode} PROPERTIES TIMEOUT 45)
endforeach()
target_compile_options(MsRdpEx_LoggingTest PRIVATE /W4 /EHsc)
add_dependencies(MsRdpEx_LoggingTest MsRdpEx_Dll)

foreach(scenario late startup toggle repeat switch disabled-path recovery levels concurrent shutdown)
    foreach(hooks 0 1)
        add_test(NAME logging.${scenario}.hooks-${hooks}
            COMMAND MsRdpEx_LoggingTest "$<TARGET_FILE:MsRdpEx_Dll>" ${scenario} ${hooks})
        set_tests_properties(logging.${scenario}.hooks-${hooks} PROPERTIES TIMEOUT 45)
    endforeach()
endforeach()

# Exercise reentrant file-open callbacks deterministically: not every OS/build
# invokes a logging hook when the CRT opens a file.
add_executable(MsRdpEx_LoggerReentrancyTest LoggerReentrancyTest.cpp ../../dll/Log.c)
target_compile_features(MsRdpEx_LoggerReentrancyTest PRIVATE cxx_std_17)
target_compile_options(MsRdpEx_LoggerReentrancyTest PRIVATE /W4 /EHsc)
add_test(NAME logging.reentrancy COMMAND MsRdpEx_LoggerReentrancyTest)
set_tests_properties(logging.reentrancy PROPERTIES TIMEOUT 45)
add_test(NAME logging.abandoned-lock COMMAND MsRdpEx_LoggerReentrancyTest shutdown)
set_tests_properties(logging.abandoned-lock PROPERTIES TIMEOUT 45)

# Build a test-only copy with fixtures for a manager-owned recording and
# process exit while a worker holds the gateway binding lock. Use the production
# lifecycle and recording sources without
# adding a testing API to the shipped DLL or requiring an RDP connection.
get_target_property(shutdown_sources MsRdpEx_Dll SOURCES)
list(FILTER shutdown_sources INCLUDE REGEX "\\.(c|cpp)$")
# Replace only the gateway translation unit with a wrapper exposing its private lock.
list(FILTER shutdown_sources EXCLUDE REGEX "(^|/)GatewayIsolation[.]cpp$")
set(shutdown_absolute_sources)
foreach(source IN LISTS shutdown_sources)
    if(NOT IS_ABSOLUTE "${source}")
        set(source "${PROJECT_SOURCE_DIR}/dll/${source}")
    endif()
    list(APPEND shutdown_absolute_sources "${source}")
endforeach()
get_target_property(shutdown_libraries MsRdpEx_Dll LINK_LIBRARIES)
add_library(MsRdpEx_ShutdownTestDll SHARED ${shutdown_absolute_sources} ShutdownFixture.cpp GatewayShutdownFixture.cpp ShutdownFixture.def)
target_include_directories(MsRdpEx_ShutdownTestDll PRIVATE "${PROJECT_SOURCE_DIR}/dll")
target_link_libraries(MsRdpEx_ShutdownTestDll PRIVATE ${shutdown_libraries})
target_compile_options(MsRdpEx_ShutdownTestDll PRIVATE /W4)

add_executable(MsRdpEx_ShutdownTest ShutdownTest.cpp)
target_compile_features(MsRdpEx_ShutdownTest PRIVATE cxx_std_17)
target_compile_options(MsRdpEx_ShutdownTest PRIVATE /W4 /EHsc)
add_dependencies(MsRdpEx_ShutdownTest MsRdpEx_ShutdownTestDll)
foreach(scenario exit unload exit-gateway-lock)
    add_test(NAME logging.recording-${scenario}
        COMMAND MsRdpEx_ShutdownTest "$<TARGET_FILE:MsRdpEx_ShutdownTestDll>" ${scenario})
    set_tests_properties(logging.recording-${scenario} PROPERTIES TIMEOUT 45)
endforeach()

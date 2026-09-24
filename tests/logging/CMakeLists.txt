add_executable(MsRdpEx_LoggingTest LoggingTest.cpp)
target_compile_features(MsRdpEx_LoggingTest PRIVATE cxx_std_17)
target_compile_options(MsRdpEx_LoggingTest PRIVATE /W4 /EHsc)
add_dependencies(MsRdpEx_LoggingTest MsRdpEx_Dll)

foreach(scenario late startup levels concurrent first-open)
    foreach(hooks 0 1)
        add_test(NAME logging.${scenario}.hooks-${hooks}
            COMMAND MsRdpEx_LoggingTest "$<TARGET_FILE:MsRdpEx_Dll>" ${scenario} ${hooks})
        set_tests_properties(logging.${scenario}.hooks-${hooks} PROPERTIES TIMEOUT 45)
    endforeach()
endforeach()

# Gateway isolation and COM compatibility.
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

# Exercise the production DLL lifecycle with a private binding-lock fixture and
# a completion signal after DllMain returns. No test exports enter the shipped DLL.
get_target_property(gateway_shutdown_sources MsRdpEx_Dll SOURCES)
list(FILTER gateway_shutdown_sources INCLUDE REGEX "\\.(c|cpp)$")
list(FILTER gateway_shutdown_sources EXCLUDE REGEX "(^|/)(GatewayIsolation|MsRdpEx|RdpInstance)[.]cpp$")
set(gateway_shutdown_absolute_sources)
foreach(source IN LISTS gateway_shutdown_sources)
    if(NOT IS_ABSOLUTE "${source}")
        set(source "${PROJECT_SOURCE_DIR}/dll/${source}")
    endif()
    list(APPEND gateway_shutdown_absolute_sources "${source}")
endforeach()
get_target_property(gateway_shutdown_libraries MsRdpEx_Dll LINK_LIBRARIES)
add_library(MsRdpEx_GatewayShutdownTestDll SHARED ${gateway_shutdown_absolute_sources}
    GatewayShutdownFixture.cpp GatewayShutdownDllMain.cpp GatewayShutdownFixture.def
    PluginReferenceFixture.cpp)
target_include_directories(MsRdpEx_GatewayShutdownTestDll PRIVATE "${PROJECT_SOURCE_DIR}/dll")
target_link_libraries(MsRdpEx_GatewayShutdownTestDll PRIVATE ${gateway_shutdown_libraries})
target_compile_options(MsRdpEx_GatewayShutdownTestDll PRIVATE /W4)
add_executable(MsRdpEx_GatewayShutdownTest GatewayShutdownTest.cpp)
target_compile_features(MsRdpEx_GatewayShutdownTest PRIVATE cxx_std_17)
target_compile_options(MsRdpEx_GatewayShutdownTest PRIVATE /W4 /EHsc)
add_dependencies(MsRdpEx_GatewayShutdownTest MsRdpEx_GatewayShutdownTestDll)
foreach(mode exit unload exit-held-lock)
    add_test(NAME logging.gateway-shutdown.${mode}
        COMMAND MsRdpEx_GatewayShutdownTest "$<TARGET_FILE:MsRdpEx_GatewayShutdownTestDll>" ${mode})
    set_tests_properties(logging.gateway-shutdown.${mode} PROPERTIES TIMEOUT 45)
endforeach()

add_executable(MsRdpEx_PluginReferenceTest PluginReferenceTest.cpp)
target_include_directories(MsRdpEx_PluginReferenceTest PRIVATE "${PROJECT_SOURCE_DIR}/dll")
target_compile_features(MsRdpEx_PluginReferenceTest PRIVATE cxx_std_17)
target_compile_options(MsRdpEx_PluginReferenceTest PRIVATE /W4 /EHsc)
add_dependencies(MsRdpEx_PluginReferenceTest MsRdpEx_GatewayShutdownTestDll)
foreach(scenario replacement same-pointer clearing destruction)
    add_test(NAME logging.plugin-reference.${scenario}
        COMMAND MsRdpEx_PluginReferenceTest
            "$<TARGET_FILE:MsRdpEx_GatewayShutdownTestDll>" ${scenario})
    set_tests_properties(logging.plugin-reference.${scenario} PROPERTIES TIMEOUT 45)
endforeach()

add_executable(MsRdpEx_DvcFactoryLifetimeTest DvcFactoryLifetimeTest.cpp)
target_include_directories(MsRdpEx_DvcFactoryLifetimeTest PRIVATE "${PROJECT_SOURCE_DIR}/dll")
target_compile_features(MsRdpEx_DvcFactoryLifetimeTest PRIVATE cxx_std_17)
target_compile_options(MsRdpEx_DvcFactoryLifetimeTest PRIVATE /W4 /EHsc)
target_link_libraries(MsRdpEx_DvcFactoryLifetimeTest PRIVATE uuid.lib)
add_dependencies(MsRdpEx_DvcFactoryLifetimeTest MsRdpEx_GatewayShutdownTestDll)
foreach(scenario removed replacement addref-reentrant release-reentrant manager-shutdown)
    add_test(NAME logging.dvc-factory.${scenario}
        COMMAND MsRdpEx_DvcFactoryLifetimeTest
            "$<TARGET_FILE:MsRdpEx_GatewayShutdownTestDll>" ${scenario})
    set_tests_properties(logging.dvc-factory.${scenario} PROPERTIES TIMEOUT 45)
endforeach()

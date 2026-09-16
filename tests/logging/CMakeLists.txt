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

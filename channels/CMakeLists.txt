
add_executable(DvcServer
	DvcServer.cpp)

target_link_libraries(DvcServer wtsapi32.lib)

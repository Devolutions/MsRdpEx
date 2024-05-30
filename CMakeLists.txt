cmake_minimum_required(VERSION 3.8)

set(CMAKE_CONFIGURATION_TYPES "Debug;Release")

project(MsRdpEx C CXX)

set(CMAKE_MODULE_PATH "${CMAKE_CURRENT_SOURCE_DIR}/cmake")

include(CSharpUtilities)
include(CMakePackageConfigHelpers)
include(WindowsRC)

file(STRINGS "${CMAKE_SOURCE_DIR}/dotnet/Devolutions.MsRdpEx/Devolutions.MsRdpEx.csproj" CSPROJ_LINES)
foreach(CSPROJ_LINE ${CSPROJ_LINES})
    if(CSPROJ_LINE MATCHES "<Version>.*</Version>")
        string(REGEX REPLACE "<Version>(.*)</Version>" "\\1" VERSION_STRING ${CSPROJ_LINE})
        string(STRIP ${VERSION_STRING} VERSION_STRING)
        break()
    endif()
endforeach()

string(REGEX REPLACE "([0-9]+).[0-9]+.[0-9]+.[0-9]+" "\\1" MSRDPEX_VERSION_MAJOR ${VERSION_STRING})
string(REGEX REPLACE "[0-9]+.([0-9]+).[0-9]+.[0-9]+" "\\1" MSRDPEX_VERSION_MINOR ${VERSION_STRING})
string(REGEX REPLACE "[0-9]+.[0-9]+.([0-9]+).[0-9]+" "\\1" MSRDPEX_VERSION_PATCH ${VERSION_STRING})
string(REGEX REPLACE "[0-9]+.[0-9]+.[0-9]+.([0-9]+)" "\\1" MSRDPEX_VERSION_BUILD ${VERSION_STRING})
set(MSRDPEX_VERSION "${MSRDPEX_VERSION_MAJOR}.${MSRDPEX_VERSION_MINOR}.${MSRDPEX_VERSION_PATCH}.${MSRDPEX_VERSION_BUILD}")

message(STATUS "VERSION: ${MSRDPEX_VERSION}")

set(MSRDPEX_NAME "MsRdpEx")
set(MSRDPEX_VENDOR "Devolutions Inc.")
set(MSRDPEX_COPYRIGHT "Copyright 2021-2023 ${MSRDPEX_VENDOR}")

set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR})
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR})
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR})

if(NOT DEFINED WITH_NATIVE)
    set(WITH_NATIVE TRUE)
endif()

if(NOT DEFINED WITH_DOTNET)
    set(WITH_DOTNET TRUE)
endif()

if(MSVC)
    include(MSVCRuntime)

    if(NOT DEFINED MSVC_RUNTIME)
        set(MSVC_RUNTIME "static")
    endif()

    configure_msvc_runtime()
endif()

if(WIN32)
    set(C_FLAGS "")
    set(C_FLAGS "${C_FLAGS} -D_UNICODE")
    set(C_FLAGS "${C_FLAGS} -D_CRT_SECURE_NO_WARNINGS")
    set(C_FLAGS "${C_FLAGS} -DWIN32_LEAN_AND_MEAN")
    set(C_FLAGS "${C_FLAGS} -D_WINSOCK_DEPRECATED_NO_WARNINGS")
    set(C_FLAGS "${C_FLAGS} -DWINVER=0x0602 -D_WIN32_WINNT=0x0602")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} ${C_FLAGS}")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} ${C_FLAGS}")

    set(C_FLAGS_RELEASE "/Zi /GF") # produce debug symbols in release builds
    set(CMAKE_C_FLAGS_RELEASE "${CMAKE_C_FLAGS_RELEASE} ${C_FLAGS_RELEASE}")
    set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} ${C_FLAGS_RELEASE}")

    # http://devcenter.wintellect.com/jrobbins/correctly-creating-native-c-release-build-pdbs
    set(LINKER_FLAGS_RELEASE "/DEBUG")
    set(LINKER_FLAGS_RELEASE "${LINKER_FLAGS_RELEASE} /OPT:REF /OPT:ICF")
    set(LINKER_FLAGS_RELEASE "${LINKER_FLAGS_RELEASE} /MAP /MAPINFO:EXPORTS")
    set(CMAKE_EXE_LINKER_FLAGS_RELEASE "${CMAKE_EXE_LINKER_FLAGS_RELEASE} ${LINKER_FLAGS_RELEASE}" CACHE STRING "" FORCE)
    set(CMAKE_SHARED_LINKER_FLAGS_RELEASE "${CMAKE_SHARED_LINKER_FLAGS_RELEASE} ${LINKER_FLAGS_RELEASE}" CACHE STRING "" FORCE)
    set(CMAKE_MODULE_LINKER_FLAGS_RELEASE "${CMAKE_MODULE_LINKER_FLAGS_RELEASE} ${LINKER_FLAGS_RELEASE}" CACHE STRING "" FORCE)
endif()

set(DEPENDENCIES_DIR "${CMAKE_SOURCE_DIR}/dependencies")
set(DETOURS_ROOT_DIR "${DEPENDENCIES_DIR}/detours")

include_directories("${DETOURS_ROOT_DIR}/include")

add_library(detours STATIC IMPORTED)

if(CMAKE_GENERATOR_PLATFORM MATCHES "ARM64")
    set(MSVC_PLATFORM_TARGET "arm64")
elseif(CMAKE_GENERATOR_PLATFORM MATCHES "Win32")
    set(MSVC_PLATFORM_TARGET "x86")
else()
    set(MSVC_PLATFORM_TARGET "x64")
endif()

message(STATUS "MSVC_PLATFORM_TARGET: ${MSVC_PLATFORM_TARGET}")

set_property(TARGET detours PROPERTY
    IMPORTED_LOCATION "${DETOURS_ROOT_DIR}/lib/${MSVC_PLATFORM_TARGET}/Release/detours.lib")
set_property(TARGET detours PROPERTY
    IMPORTED_LOCATION_DEBUG "${DETOURS_ROOT_DIR}/lib/${MSVC_PLATFORM_TARGET}/Debug/detours.lib")

include_directories("${CMAKE_SOURCE_DIR}/include")
include_directories("${CMAKE_SOURCE_DIR}/com")

if(WITH_NATIVE)
    add_subdirectory(dll)
    add_subdirectory(exe)
    add_subdirectory(channels)
endif()

if(WITH_DOTNET)
    add_subdirectory(dotnet)
endif()

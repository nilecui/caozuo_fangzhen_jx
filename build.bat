@echo off
setlocal EnableDelayedExpansion

:: -----------------------------------------------
:: 用法：build.bat [版本号]
::   build.bat          → 构建到 Builds\Windows\
::   build.bat 1.2.3    → 额外生成 VirtualSimSystem-Windows-v1.2.3.zip
:: -----------------------------------------------

set VERSION=%~1

:: Unity 可执行文件路径 —— 如果你的版本不同，修改这里
set UNITY_VERSION=2022.3.62f1
set UNITY_EXE=C:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Unity.exe

if not exist "%UNITY_EXE%" (
    echo [ERROR] 找不到 Unity: %UNITY_EXE%
    echo 请修改 build.bat 中的 UNITY_VERSION 或 UNITY_EXE 路径
    exit /b 1
)

set PROJECT_PATH=%~dp0
set LOG_FILE=%PROJECT_PATH%Logs\build.log
set BUILD_OUTPUT_DIR=%PROJECT_PATH%Builds\Windows

mkdir "%PROJECT_PATH%Logs" 2>nul
mkdir "%BUILD_OUTPUT_DIR%" 2>nul

echo [Build] Unity %UNITY_VERSION%
echo [Build] 项目路径: %PROJECT_PATH%
echo [Build] 输出目录: %BUILD_OUTPUT_DIR%
echo [Build] 日志: %LOG_FILE%
echo.

"%UNITY_EXE%" ^
  -batchmode ^
  -nographics ^
  -quit ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod BuildScript.BuildWindows64 ^
  -logFile "%LOG_FILE%"

if !ERRORLEVEL! NEQ 0 (
    echo.
    echo [ERROR] 构建失败，查看日志: %LOG_FILE%
    exit /b 1
)

echo.
echo [Build] 构建成功: %BUILD_OUTPUT_DIR%\VirtualSimSystem.exe

:: 如果传入了版本号，打 zip 包
if "%VERSION%"=="" goto :done
set ZIP_NAME=VirtualSimSystem-Windows-v%VERSION%.zip

where 7z >nul 2>&1
if !ERRORLEVEL! EQU 0 (
    7z a "%PROJECT_PATH%%ZIP_NAME%" "%BUILD_OUTPUT_DIR%\*" -mx=5
    echo [Build] 压缩包: %ZIP_NAME%
) else (
    echo [WARN] 未找到 7z，跳过打包。安装 7-Zip 后可自动打包。
)

:done
echo.
echo 完成。
endlocal

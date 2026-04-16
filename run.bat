@echo off
echo ========================================
echo Personal Finance Manager
echo ========================================
echo.
echo Stopping any running instances...
taskkill /F /IM PersonalFinanceManager.exe 2>nul
taskkill /F /IM dotnet.exe /FI "WINDOWTITLE eq *PersonalFinanceManager*" 2>nul
timeout /t 2 /nobreak >nul
echo.
echo Checking .NET installation...
dotnet --version
if errorlevel 1 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo.
echo Restoring NuGet packages...
cd PersonalFinanceManager
dotnet restore
if errorlevel 1 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)
echo.
echo Building application...
dotnet build
if errorlevel 1 (
    echo ERROR: Build failed
    pause
    exit /b 1
)
echo.
echo Starting application...
echo.
echo The application will open in your browser at:
echo https://localhost:5001
echo.
echo Press Ctrl+C to stop the application
echo.
dotnet run
pause

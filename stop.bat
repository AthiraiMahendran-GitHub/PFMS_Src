@echo off
echo Stopping Personal Finance Manager...
taskkill /F /IM PersonalFinanceManager.exe 2>nul
taskkill /F /IM dotnet.exe /FI "WINDOWTITLE eq *PersonalFinanceManager*" 2>nul
echo.
echo Application stopped.
timeout /t 2 /nobreak >nul

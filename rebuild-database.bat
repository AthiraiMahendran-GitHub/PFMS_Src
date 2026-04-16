@echo off
echo ========================================
echo Rebuild Database with New Schema
echo ========================================
echo.

echo Step 1: Stopping any running instances...
taskkill /F /IM PersonalFinanceManager.exe 2>nul
taskkill /F /IM dotnet.exe /FI "WINDOWTITLE eq *PersonalFinanceManager*" 2>nul
timeout /t 2 /nobreak >nul
echo.

echo Step 2: Deleting old database files...
cd PersonalFinanceManager
del /F /Q personalfinance.db* 2>nul
del /F /Q bin\Debug\net8.0\personalfinance.db* 2>nul
echo Database files deleted.
echo.

echo Step 3: Cleaning build...
dotnet clean
echo.

echo Step 4: Building application...
dotnet build
if errorlevel 1 (
    echo ERROR: Build failed
    pause
    exit /b 1
)
echo.

echo Step 5: Running application...
echo.
echo The database will be recreated with new schema including:
echo - DematAccounts table
echo - InvestmentHoldings table
echo - InvestmentTransactions table
echo.
echo Press Ctrl+C to stop the application when done.
echo.
dotnet run
pause

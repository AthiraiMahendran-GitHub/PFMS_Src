# Fix Portfolio Page Error

## Error
```
SqliteException: SQLite Error 1: 'no such table: DematAccounts'
```

## Cause
The database was created with the old schema and doesn't have the new investment tables:
- DematAccounts
- InvestmentHoldings  
- InvestmentTransactions

## Solution

### Option 1: Use the Rebuild Script (Recommended)
```bash
# Run this script - it will stop the app, delete DB, and rebuild
.\rebuild-database.bat
```

### Option 2: Manual Steps

1. **Stop the Running Application**
   - Press `Ctrl + C` in the terminal where the app is running
   - Or use Task Manager to end "PersonalFinanceManager" process
   - Or run: `.\stop.bat`

2. **Delete Old Database Files**
   ```bash
   cd PersonalFinanceManager
   del /F /Q personalfinance.db*
   del /F /Q bin\Debug\net8.0\personalfinance.db*
   ```

3. **Clean and Rebuild**
   ```bash
   dotnet clean
   dotnet build
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```
   
   The database will be automatically recreated with the new schema.

## Verify the Fix

After running the application:

1. Navigate to http://localhost:55891 (or the port shown)
2. Click on "Portfolio" in the sidebar
3. You should see the Portfolio page without errors
4. Click "Add Demat Account" to create your first demat account

## What's New in the Database

The new schema includes:

### DematAccounts Table
- Stores your brokerage accounts (Zerodha, Upstox, etc.)
- Links to your bank accounts

### InvestmentHoldings Table
- Current portfolio holdings
- Tracks quantity, average price, current price
- Calculates unrealized P/L

### InvestmentTransactions Table
- Complete buy/sell history
- Tracks realized P/L
- Links to bank accounts for money flow

## Next Steps

After the database is recreated:

1. **Create a Demat Account**
   - Go to Portfolio page
   - Click "Add Demat Account"
   - Enter details (Name, Account Number, Broker)
   - Link to a bank account

2. **Buy Your First Stock**
   - Click "Buy Stock"
   - Enter stock details
   - Money will be deducted from linked bank account
   - Holding will be created

3. **Track Your Portfolio**
   - View all holdings
   - See unrealized P/L
   - Update current prices
   - Sell stocks when needed

## Troubleshooting

### If the error persists:

1. Make sure the application is completely stopped
2. Check if database files are deleted:
   ```bash
   dir PersonalFinanceManager\*.db*
   dir PersonalFinanceManager\bin\Debug\net8.0\*.db*
   ```
3. If files still exist, delete them manually
4. Rebuild and run again

### If build fails:

1. Check for compilation errors
2. Make sure all new files are included:
   - DematAccountService.cs
   - InvestmentTransactionService.cs
   - Portfolio.razor
3. Verify Program.cs has the new services registered

## Important Notes

- The old `Investments` table is kept for backward compatibility
- You can access the old investments page via "Investments (Old)" in the menu
- The new Portfolio system is separate and more comprehensive
- All your existing data (accounts, expenses, income, etc.) will remain intact

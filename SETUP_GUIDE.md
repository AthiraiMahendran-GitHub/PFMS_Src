# Personal Finance Manager - Setup Guide

## Prerequisites

Before you begin, ensure you have the following installed:

1. **.NET 8.0 SDK** or later
   - Download from: https://dotnet.microsoft.com/download
   - Verify installation: `dotnet --version`

2. **Visual Studio 2022** (recommended) or **Visual Studio Code**
   - Visual Studio 2022: https://visualstudio.microsoft.com/
   - VS Code: https://code.visualstudio.com/

## Installation Steps

### Option 1: Using Visual Studio 2022

1. Open Visual Studio 2022
2. Click "Open a project or solution"
3. Navigate to the project folder and open `PersonalFinanceManager.sln`
4. Wait for NuGet packages to restore automatically
5. Press `F5` or click the "Run" button to start the application
6. The application will open in your default browser at `https://localhost:5001`

### Option 2: Using Command Line

1. Open a terminal/command prompt
2. Navigate to the project directory:
   ```bash
   cd PersonalFinanceManager
   ```

3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Build the project:
   ```bash
   dotnet build
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

6. Open your browser and navigate to: `https://localhost:5001`

### Option 3: Using VS Code

1. Open VS Code
2. Open the project folder
3. Install the C# extension if not already installed
4. Open the integrated terminal (Ctrl+`)
5. Run:
   ```bash
   dotnet restore
   dotnet run
   ```
6. Open browser to `https://localhost:5001`

## First Time Setup

When you run the application for the first time:

1. The SQLite database (`personalfinance.db`) will be created automatically
2. All necessary tables will be initialized
3. You can start adding your data immediately

## Features Overview

### 1. Dashboard
- View summary of today's expenses
- Monthly and yearly expense totals
- Investment portfolio overview with profit/loss
- Task status overview
- Quick access to all modules

### 2. Expense Management
- Add, edit, and delete expenses
- Categorize expenses
- Date-based tracking
- Filter and sort capabilities
- Export to Excel

### 3. Investment Portfolio
- Track multiple investment types (Stocks, Crypto, Mutual Funds, Real Estate, etc.)
- Real-time profit/loss calculation
- ROI percentage tracking
- Update current values
- Performance analytics

### 4. Task Management
- Organize tasks by company
- Set priority levels (Low, Medium, High, Critical)
- Track status (Pending, In Progress, Completed, Cancelled)
- Due date management
- Quick completion marking

### 5. Daily Notes
- Category-based organization
- Rich text support
- Search and filter
- View full note content
- Timestamp tracking

### 6. Reports & Analytics
- Expense analysis by category
- Monthly expense trends with charts
- Investment performance reports
- Task completion analytics
- Export all data to Excel

## Database Location

The SQLite database file is created in the project root directory:
- File name: `personalfinance.db`
- Location: Same folder as the executable

To backup your data, simply copy this file to a safe location.

## Troubleshooting

### Issue: Port already in use
**Solution:** Edit `appsettings.json` or use a different port:
```bash
dotnet run --urls "https://localhost:5002"
```

### Issue: NuGet packages not restoring
**Solution:** Clear NuGet cache and restore:
```bash
dotnet nuget locals all --clear
dotnet restore
```

### Issue: Database errors
**Solution:** Delete `personalfinance.db` file and restart the application. A new database will be created.

### Issue: Browser doesn't open automatically
**Solution:** Manually open your browser and navigate to:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## Configuration

### Change Database Location
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=C:\\MyData\\personalfinance.db"
  }
}
```

### Change Port
Edit `Properties/launchSettings.json` or use command line:
```bash
dotnet run --urls "https://localhost:YOUR_PORT"
```

## Technology Stack

- **Framework:** .NET 8.0 Blazor Server
- **Database:** SQLite with Entity Framework Core
- **UI Library:** Radzen Blazor Components
- **Export:** ClosedXML for Excel
- **Architecture:** Clean Architecture with Service Layer

## Security Features

- HTTPS enforcement
- Input validation on all forms
- SQL injection prevention (EF Core parameterized queries)
- XSS protection (Blazor automatic encoding)

## Performance Tips

1. The application uses server-side Blazor for optimal performance
2. Database queries are indexed for faster retrieval
3. Pagination is enabled on all data grids
4. Lazy loading for large datasets

## Support & Customization

### Adding New Categories
Categories are free-form text fields. Simply type a new category name when adding an expense or note.

### Customizing Investment Types
Edit `InvestmentDialog.razor` to add more investment types to the dropdown.

### Changing Theme
Radzen supports multiple themes. Edit `_Host.cshtml` to change the CSS reference:
- Material: `material-base.css`
- Standard: `standard-base.css`
- Dark: `dark-base.css`

## Backup & Restore

### Backup
1. Copy `personalfinance.db` to a safe location
2. Or use the "Export All Data" button in Reports to export to Excel

### Restore
1. Replace `personalfinance.db` with your backup file
2. Restart the application

## Next Steps

1. Start by adding some expenses to see the dashboard populate
2. Add your investments to track portfolio performance
3. Create tasks for your daily work
4. Use notes for journaling or quick thoughts
5. Check the Reports page for analytics and insights

## License

This is a personal project. Feel free to modify and customize as needed.

# Personal Finance Manager - Professional .NET Application

A comprehensive personal finance and productivity management application built with .NET 8.0 Blazor Server, featuring modern UI/UX and advanced reporting capabilities.

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Blazor](https://img.shields.io/badge/Blazor-Server-purple)
![SQLite](https://img.shields.io/badge/Database-SQLite-green)
![License](https://img.shields.io/badge/License-MIT-yellow)

## 🚀 Quick Start

```bash
# Navigate to project directory
cd PersonalFinanceManager

# Restore packages
dotnet restore

# Run the application
dotnet run

# Open browser to https://localhost:5001
```

For detailed setup instructions, see [SETUP_GUIDE.md](SETUP_GUIDE.md)

## ✨ Features

### 💰 Expense Management
- Track daily expenses with custom categories
- Advanced filtering and sorting
- Category-wise analysis with visual charts
- Monthly and yearly summaries
- Export to Excel

### 📈 Investment Portfolio
- Support for multiple investment types (Stocks, Crypto, Mutual Funds, Real Estate, Bonds, Gold)
- Real-time profit/loss calculation
- ROI percentage tracking
- Portfolio performance analytics
- Historical tracking

### ✅ Task Management
- Company-wise task organization
- Priority levels (Low, Medium, High, Critical)
- Status tracking (Pending, In Progress, Completed, Cancelled)
- Due date management
- Quick completion marking
- Overdue task alerts

### 📝 Daily Notes
- Category-based organization
- Full-text search capability
- Rich text support
- Timestamp tracking
- Personal journaling

### 📊 Advanced Reporting
- Interactive dashboard with key metrics
- Expense trends with line charts
- Category-wise pie charts
- Investment performance analysis
- Task completion analytics
- Export all data to Excel

## 🏗️ Architecture

### Technology Stack
- **Framework**: .NET 8.0 Blazor Server
- **Database**: SQLite with Entity Framework Core
- **UI Library**: Radzen Blazor Components (Professional-grade)
- **Export**: ClosedXML for Excel reports
- **Architecture**: Clean Architecture with Service Layer Pattern

### Project Structure
```
PersonalFinanceManager/
├── Data/
│   ├── AppDbContext.cs          # EF Core database context
│   └── Models.cs                # Domain models
├── Services/
│   ├── ExpenseService.cs        # Expense business logic
│   ├── InvestmentService.cs     # Investment operations
│   ├── TaskService.cs           # Task management
│   ├── NoteService.cs           # Note operations
│   ├── ReportService.cs         # Analytics and reporting
│   └── ExportService.cs         # Excel export functionality
├── Pages/
│   ├── Index.razor              # Dashboard
│   ├── Expenses.razor           # Expense management
│   ├── Investments.razor        # Investment portfolio
│   ├── Tasks.razor              # Task management
│   ├── Notes.razor              # Daily notes
│   └── Reports.razor            # Analytics and reports
├── Shared/
│   └── MainLayout.razor         # Main layout with navigation
└── wwwroot/
    ├── css/                     # Stylesheets
    └── js/                      # JavaScript utilities
```

## 🔧 Configuration

### Database
The application uses SQLite for data storage. The database file (`personalfinance.db`) is created automatically on first run.

To change the database location, edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=your-path/personalfinance.db"
  }
}
```

### Port Configuration
Default ports:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

To change ports, use:
```bash
dotnet run --urls "https://localhost:YOUR_PORT"
```

## 📦 Dependencies

- Microsoft.EntityFrameworkCore.Sqlite (8.0.0)
- Microsoft.EntityFrameworkCore.Design (8.0.0)
- Radzen.Blazor (4.25.0)
- ClosedXML (0.102.1)

## 🔒 Security Features

- HTTPS enforcement
- Input validation on all forms
- SQL injection prevention (EF Core parameterized queries)
- XSS protection (Blazor automatic encoding)
- Secure data storage

## 📊 Performance Optimizations

- Server-side Blazor for optimal performance
- Database query indexing
- Pagination for large datasets
- Lazy loading
- Efficient data caching

## 💾 Backup & Restore

### Backup
1. Copy `personalfinance.db` file to a safe location
2. Or use "Export All Data" in Reports page

### Restore
1. Replace `personalfinance.db` with your backup
2. Restart the application

## 🎨 UI/UX Features

- Modern, responsive design
- Material Design theme
- Interactive charts and graphs
- Intuitive navigation
- Mobile-friendly interface
- Real-time data updates
- Toast notifications
- Confirmation dialogs

## 📈 Future Enhancements

- [ ] Multi-user support with authentication
- [ ] Cloud synchronization
- [ ] Mobile app (Xamarin/MAUI)
- [ ] Budget planning and forecasting
- [ ] Bill reminders and recurring expenses
- [ ] Tax calculation and reporting
- [ ] Bank account integration
- [ ] PDF report generation
- [ ] Email notifications
- [ ] Data import from CSV/Excel

## 🐛 Troubleshooting

See [SETUP_GUIDE.md](SETUP_GUIDE.md) for detailed troubleshooting steps.

## 📝 License

This project is open source and available for personal use.

## 🤝 Contributing

This is a personal project, but suggestions and improvements are welcome!

## 📧 Support

For issues or questions, please create an issue in the repository.

---

**Built with ❤️ using .NET 8.0 and Blazor**

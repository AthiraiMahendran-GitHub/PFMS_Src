# Personal Finance Manager - User Guide

## 🚀 Quick Start

### Running the Application
```bash
cd PersonalFinanceManager
dotnet run
```

Access the application at:
- **HTTPS**: https://localhost:55891
- **HTTP**: http://localhost:55892

---

## 💼 Investment Portfolio Management

### Getting Started with Investments

#### 1. Create a Demat Account
1. Navigate to **Portfolio** page
2. Click **"Add Demat Account"** button
3. Fill in the form:
   - **Account Name**: e.g., "Zerodha", "Upstox"
   - **Broker Name**: Your broker's name
   - **Account Number**: Your demat account number
4. Click **"Create"**

#### 2. Buy Stocks
1. Select your demat account from the dropdown
2. Click **"Buy Stock"** button
3. Enter stock details:
   - **Stock Symbol**: e.g., RELIANCE, TCS, INFY
   - **Stock Name**: Full company name
   - **Quantity**: Number of shares
   - **Price per Share**: Purchase price
   - **Transaction Date**: Date of purchase
   - **Brokerage Fee**: Broker charges (optional)
   - **Notes**: Any additional notes (optional)
4. Click **"Buy Stock"**

**What happens:**
- If it's a new stock, a new holding is created
- If you already own the stock, quantity is added and average price is recalculated
- Total investment is automatically calculated

#### 3. Sell Stocks
1. Find the stock in your holdings table
2. Click the **"Sell"** button next to the holding
3. Enter sale details:
   - **Quantity to Sell**: How many shares (max shown)
   - **Selling Price per Share**: Sale price
   - **Transaction Date**: Date of sale
   - **Brokerage Fee**: Broker charges
   - **Notes**: Any additional notes
4. Review the estimated P/L
5. Click **"Sell Stock"**

**What happens:**
- Holding quantity is reduced
- Realized profit/loss is calculated and recorded
- If all shares are sold, the holding is removed
- Sale amount is calculated (price × quantity - brokerage)

#### 4. Update Stock Prices
1. Click the **update icon** (🔄) next to any holding
2. Enter the new current market price
3. Click **"Update Price"**

**What happens:**
- Current price is updated
- Unrealized P/L is recalculated
- Portfolio value is updated

#### 5. View Transaction History
1. Click **"Transaction History"** button
2. View all buy/sell transactions
3. See realized P/L for sell transactions
4. Filter and sort as needed

### Understanding Portfolio Metrics

**Summary Cards:**
- **Total Invested**: Sum of all money invested (quantity × average price)
- **Current Value**: Current market value (quantity × current price)
- **Unrealized P/L**: Profit/loss on holdings you still own
- **Realized P/L**: Profit/loss from stocks you've sold

**Holdings Table Columns:**
- **Symbol**: Stock ticker symbol
- **Name**: Company name
- **Type**: Stock/MutualFund/Bond
- **Quantity**: Number of shares owned
- **Avg Price**: Average purchase price
- **Current Price**: Latest market price
- **Invested**: Total money invested
- **Current Value**: Current market value
- **P/L**: Unrealized profit/loss with percentage

---

## 📝 Notes with Star & Pin

### Using Star Feature
1. Navigate to **Notes** page
2. Click the **star icon** (⭐) next to any note
3. Starred notes are marked as important
4. Click again to unstar

### Using Pin Feature
1. Click the **pin icon** (📌) next to any note
2. Pinned notes appear in a separate section at the top
3. Click again to unpin

### Best Practices
- **Star**: Use for important notes you need to remember
- **Pin**: Use for notes you need quick access to
- You can star and pin the same note

---

## 📊 Income Reports

### Accessing Income Reports
1. Navigate to **Reports** page
2. Click on **"Income Analysis"** tab

### Available Reports

#### 1. Income by Source
**What it shows:**
- Total income from each source
- Number of transactions per source
- Average income per transaction
- Pie chart visualization

**Use cases:**
- Identify your primary income sources
- Track income diversity
- Compare income from different sources

#### 2. Monthly Income Trends
**What it shows:**
- Income for each month over the last 12 months
- Line chart showing trends
- Transaction count per month

**Use cases:**
- Identify seasonal patterns
- Track income growth
- Spot unusual months

#### 3. Income vs Expense Comparison
**What it shows:**
- Total income vs total expense
- Net savings (income - expense)
- Monthly comparison chart
- Color-coded cards (green = positive, red = negative)

**Use cases:**
- Track your savings rate
- Identify months with overspending
- Monitor financial health
- Plan budget adjustments

### Understanding the Charts

**Pie Chart (Income by Source):**
- Each slice represents a source
- Larger slice = more income from that source
- Hover to see exact amounts

**Line Chart (Monthly Trends):**
- X-axis: Months
- Y-axis: Amount in ₹
- Hover to see exact values

**Multi-Line Chart (Income vs Expense):**
- Green line: Income
- Red line: Expense
- Blue line: Net Savings
- Hover to see exact values for each month

---

## 💡 Tips & Best Practices

### Investment Management
1. **Update prices regularly**: Keep your portfolio value accurate
2. **Record all transactions**: Don't forget brokerage fees
3. **Use notes field**: Record why you bought/sold
4. **Review P/L regularly**: Check your investment performance
5. **Multiple demat accounts**: Create separate accounts for different brokers

### Notes Organization
1. **Pin active tasks**: Keep current work at the top
2. **Star important info**: Mark critical information
3. **Regular cleanup**: Unpin completed items
4. **Use categories**: Organize notes by topic

### Income Tracking
1. **Consistent sources**: Use same source names
2. **Regular entries**: Record income promptly
3. **Review monthly**: Check income trends
4. **Compare with expenses**: Monitor savings rate

---

## 🔧 Troubleshooting

### Portfolio Issues

**Problem**: Can't see my holdings
- **Solution**: Make sure you've selected a demat account from the dropdown

**Problem**: Average price seems wrong
- **Solution**: Average price is calculated as: (old_qty × old_price + new_qty × new_price) / total_qty

**Problem**: Can't sell more than I own
- **Solution**: This is by design. Check your current holding quantity.

### Report Issues

**Problem**: No data in reports
- **Solution**: Make sure you have income/expense entries in the system

**Problem**: Charts not showing
- **Solution**: Refresh the page. Ensure you have data for the selected period.

### General Issues

**Problem**: Application won't start
- **Solution**: 
  1. Stop any running instances
  2. Run `dotnet clean`
  3. Run `dotnet build`
  4. Run `dotnet run`

**Problem**: Database errors
- **Solution**: Delete `personalfinance.db*` files and restart the application

---

## 📱 Navigation

### Main Menu
- **Dashboard**: Overview of all finances
- **Expenses**: Track spending
- **Income**: Record income
- **Accounts**: Manage bank accounts
- **Categories**: Organize expenses
- **Portfolio**: Investment management (NEW)
- **Investments (Old)**: Legacy investment page
- **Loans**: Track lending/borrowing
- **Tasks**: Manage financial tasks
- **Notes**: Personal notes with star/pin
- **Reports**: Financial analytics with income reports
- **Transactions**: Complete transaction history

---

## 🎯 Common Workflows

### Workflow 1: Recording a Stock Purchase
1. Portfolio → Select Demat Account
2. Click "Buy Stock"
3. Enter: Symbol, Name, Quantity, Price, Date
4. Add brokerage fee if applicable
5. Click "Buy Stock"
6. Verify in holdings table

### Workflow 2: Checking Investment Performance
1. Portfolio → Select Demat Account
2. Review summary cards (Total Invested, Current Value, P/L)
3. Check individual holdings in table
4. Update prices if needed
5. Review transaction history

### Workflow 3: Monthly Financial Review
1. Reports → Income Analysis
2. Check total income vs expense
3. Review net savings
4. Check monthly trends
5. Identify areas for improvement
6. Reports → Expense Analysis
7. Review category-wise spending

### Workflow 4: Organizing Important Notes
1. Notes → Review all notes
2. Star important information
3. Pin active/current notes
4. Unpin completed items
5. Delete obsolete notes

---

## 📞 Support

For issues or questions:
1. Check this user guide
2. Review IMPLEMENTATION_COMPLETE.md for technical details
3. Check the Error List in Visual Studio
4. Review application logs in the console

---

**Version**: 1.0
**Last Updated**: April 11, 2026

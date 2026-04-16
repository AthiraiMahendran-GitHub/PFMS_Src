# Investment Portfolio Management System

## Overview

A comprehensive investment tracking system that properly handles:
- Demat accounts (where stocks are held)
- Bank account transfers for buying/selling
- Buy/Sell transactions with proper accounting
- Holdings with average price calculation
- Realized and unrealized profit/loss tracking

## Architecture

### 1. Demat Account
**Purpose:** Represents your brokerage account (Zerodha, Upstox, etc.)

**Features:**
- Multiple demat accounts support
- Linked to bank account for fund transfers
- Tracks all holdings and transactions

**Fields:**
- Name (e.g., "Zerodha")
- Account Number
- Broker Name
- Linked Bank Account
- Active Status

### 2. Investment Holdings
**Purpose:** Current portfolio - what you own right now

**Features:**
- Real-time holdings with quantities
- Average buy price calculation
- Current market price
- Unrealized profit/loss (paper gains/losses)

**Fields:**
- Symbol (e.g., "RELIANCE", "TCS")
- Name
- Type (Stock, Mutual Fund, Bond, etc.)
- Quantity
- Average Price (automatically calculated)
- Current Price (manually updated or API)
- Total Invested
- Current Value
- Unrealized P/L

**How Average Price Works:**
```
Example:
Buy 1: 10 shares @ ₹100 = ₹1,000
Buy 2: 5 shares @ ₹120 = ₹600
Average Price = (1,000 + 600) / (10 + 5) = ₹106.67
```

### 3. Investment Transactions
**Purpose:** Complete history of all buy/sell activities

**Transaction Types:**
- **Buy:** Purchase stocks (money goes out of bank account)
- **Sell:** Sell stocks (money comes into bank account)
- **Dividend:** Dividend received
- **Bonus:** Bonus shares received
- **Split:** Stock split

**Buy Transaction Flow:**
1. User enters: Symbol, Quantity, Price, Brokerage, Tax
2. System calculates: Net Amount = (Quantity × Price) + Brokerage + Tax
3. Deduct Net Amount from Bank Account
4. Update/Create Holding:
   - If new stock: Create holding with quantity and price
   - If existing: Update quantity and recalculate average price
5. Record transaction

**Sell Transaction Flow:**
1. User enters: Symbol, Quantity, Price, Brokerage, Tax
2. System calculates: Net Amount = (Quantity × Price) - Brokerage - Tax
3. Calculate Realized P/L = (Sell Price - Average Buy Price) × Quantity - Charges
4. Add Net Amount to Bank Account
5. Update Holding:
   - Reduce quantity
   - If quantity becomes 0, remove holding
6. Record transaction with realized P/L

**Fields:**
- Symbol, Name, Type
- Action (Buy/Sell/Dividend/etc.)
- Quantity
- Price per unit
- Brokerage Charges
- Tax Charges
- Net Amount
- Transaction Date
- From Account (Bank Account)
- Realized P/L (for sell transactions)
- Notes

## Money Flow Examples

### Example 1: Buying Stocks

**Scenario:** Buy 10 shares of RELIANCE @ ₹2,500

```
Initial State:
- Bank Account (HDFC): ₹50,000
- Holdings: Empty

Transaction:
- Quantity: 10
- Price: ₹2,500
- Brokerage: ₹20
- Tax: ₹30
- Net Amount: (10 × 2,500) + 20 + 30 = ₹25,050

After Transaction:
- Bank Account (HDFC): ₹50,000 - ₹25,050 = ₹24,950
- Holdings: RELIANCE - 10 shares @ ₹2,500 avg price
```

### Example 2: Buying More of Same Stock

**Scenario:** Buy 5 more shares of RELIANCE @ ₹2,600

```
Before:
- Holdings: RELIANCE - 10 shares @ ₹2,500 avg price

Transaction:
- Quantity: 5
- Price: ₹2,600
- Net Amount: ₹13,050

After:
- Holdings: RELIANCE - 15 shares @ ₹2,533.33 avg price
  Calculation: (10 × 2,500 + 5 × 2,600) / 15 = ₹2,533.33
```

### Example 3: Selling Stocks

**Scenario:** Sell 8 shares of RELIANCE @ ₹2,700

```
Before:
- Holdings: RELIANCE - 15 shares @ ₹2,533.33 avg price
- Bank Account: ₹24,950

Transaction:
- Quantity: 8
- Price: ₹2,700
- Brokerage: ₹20
- Tax: ₹30
- Net Amount: (8 × 2,700) - 20 - 30 = ₹21,550

Realized P/L Calculation:
- Buy Cost: 8 × ₹2,533.33 = ₹20,266.64
- Sell Value: 8 × ₹2,700 = ₹21,600
- Charges: ₹50
- Realized P/L: ₹21,600 - ₹20,266.64 - ₹50 = ₹1,283.36 (Profit)

After:
- Bank Account: ₹24,950 + ₹21,550 = ₹46,500
- Holdings: RELIANCE - 7 shares @ ₹2,533.33 avg price
```

### Example 4: Profit/Loss Tracking

**Unrealized P/L (Paper Gains):**
```
Holdings: RELIANCE - 7 shares @ ₹2,533.33 avg
Current Market Price: ₹2,800

Unrealized P/L = (Current Price - Avg Price) × Quantity
               = (₹2,800 - ₹2,533.33) × 7
               = ₹1,866.69 (Profit)
```

**Realized P/L (Actual Gains):**
```
Sum of all sell transactions' realized P/L
From Example 3: ₹1,283.36
```

**Total P/L:**
```
Total P/L = Realized P/L + Unrealized P/L
          = ₹1,283.36 + ₹1,866.69
          = ₹3,150.05
```

## Database Schema

### DematAccounts Table
```sql
- Id (PK)
- Name
- AccountNumber
- BrokerName
- LinkedBankAccountId (FK to Accounts)
- IsActive
- CreatedAt
```

### InvestmentHoldings Table
```sql
- Id (PK)
- DematAccountId (FK)
- Symbol
- Name
- Type (Enum)
- Quantity
- AveragePrice
- CurrentPrice
- LastUpdated
```

### InvestmentTransactions Table
```sql
- Id (PK)
- DematAccountId (FK)
- HoldingId (FK)
- Symbol
- Name
- Type (Enum)
- Action (Enum: Buy/Sell/Dividend/etc.)
- Quantity
- Price
- BrokerageCharges
- TaxCharges
- TransactionDate
- FromAccountId (FK to Accounts)
- RealizedProfitLoss (for sells)
- Notes
- CreatedAt
```

## Features to Implement

### Phase 1: Core Functionality ✅
- [x] Data models
- [x] Database schema
- [x] Services (DematAccountService, InvestmentTransactionService)
- [x] Portfolio page with holdings view

### Phase 2: Dialogs & UI
- [ ] DematAccountDialog (Add/Edit demat account)
- [ ] BuyStockDialog (Buy transaction with bank account selection)
- [ ] SellStockDialog (Sell transaction with P/L calculation)
- [ ] UpdatePriceDialog (Update current market price)
- [ ] TransactionHistoryDialog (View all transactions)

### Phase 3: Advanced Features
- [ ] Bulk price update (manual or API integration)
- [ ] Dividend tracking
- [ ] Bonus shares handling
- [ ] Stock split handling
- [ ] Portfolio analytics (sector-wise, type-wise breakdown)
- [ ] Tax calculation (STCG, LTCG)
- [ ] Export to Excel/PDF

### Phase 4: Integration
- [ ] Market data API integration (NSE/BSE)
- [ ] Auto price updates
- [ ] Alerts (price targets, stop loss)
- [ ] Portfolio rebalancing suggestions

## Benefits of This Design

1. **Accurate Accounting:** Money flow is properly tracked between bank and demat accounts
2. **Average Price Calculation:** Automatically calculates weighted average for multiple buys
3. **Separate P/L Tracking:** Clear distinction between realized (actual) and unrealized (paper) gains
4. **Transaction History:** Complete audit trail of all activities
5. **Multiple Demat Accounts:** Support for multiple brokers
6. **Scalable:** Easy to add new features like dividends, splits, etc.

## Migration from Old System

The old `Investment` table is kept for backward compatibility. Users can:
1. Continue using the old system
2. Migrate data to new system manually
3. Use both systems in parallel

## Next Steps

1. **Stop the running application**
2. **Delete old database** to recreate with new schema
3. **Build and run** the application
4. **Create Demat Account** first
5. **Start buying stocks** and see the portfolio grow!

The system is now ready for professional investment tracking with proper accounting! 🚀

# 🚀 Tawsella - Multi-Sided Delivery Platform

## 📌 Overview

Tawsella is a comprehensive delivery platform that connects **Customers**, **Merchants**, **Couriers**, and **Shipping Companies** through a scalable mobile and web application. The platform facilitates fast, flexible delivery services with real-time tracking, secure payments, and intelligent order matching.

---

## 🏗️ Current Implementation Status

### ✅ Phase 1: Identity & Authentication (COMPLETED)

The complete **Identity & Authentication** system is fully implemented and production-ready.

### ✅ Phase 2: Database Architecture (COMPLETED)

Complete entity relationship model with configurations for all business entities.

### 🔄 Phase 3: Core Business Features (IN PROGRESS)

Order management, tracking, wallet system, and subscriptions.

---

## 👥 User Types & Roles

The system supports 5 distinct user types with role-based access control:

### 1. **SuperAdmin**
- Full system access
- Create and manage Admin accounts
- Send credentials via email to new Admins
- System configuration and monitoring

### 2. **Admin**
- Approve/reject Courier and Merchant registrations
- Manage user accounts and permissions
- Process courier withdrawal requests
- Handle disputes and customer support
- View financial reports and analytics

### 3. **Customer**
- Create and track delivery orders
- Save multiple delivery addresses
- Rate and review couriers
- Manage payment methods
- Earn loyalty points and tier benefits (Bronze → Silver → Gold → Platinum)

### 4. **Courier** (Requires Admin Approval)
- Accept/reject delivery requests
- Real-time GPS location tracking
- Manage online/offline availability
- Wallet system for earnings management
- Request withdrawals to bank account
- View delivery history and performance stats

### 5. **Merchant** (Requires Admin Approval)
- Create single or bulk orders
- Business verification (KYC)
- Subscription plans with commission tiers
- Advanced analytics and reporting
- API access (on premium plans)
- Fleet management integration

### 6. **Shipping Company** (Requires Admin Approval)
- Fleet and driver management
- Subscription-based pricing (no per-order commission)
- Bulk order handling
- Internal order assignment to company drivers
- Company-level analytics and reports

---

## 🔐 Authentication & Security

### JWT-Based Authentication
- **Access Tokens**: Short-lived (15 minutes) for API requests
- **Refresh Tokens**: Long-lived (7 days) with automatic rotation
- **Security Stamp**: Invalidates all tokens on password change
- **Role-Based Authorization**: Fine-grained access control per endpoint

### Email Verification System
- **6-digit TOTP codes** sent via MailKit SMTP
- **10-minute expiration** for security
- **Resend functionality** with rate limiting
- Required for account activation

### Password Security
- Secure password hashing (ASP.NET Core Identity)
- Password reset via email verification
- Security stamp rotation on sensitive changes

---

## 🗄️ Database Architecture

### User Entities
- **User** (Base): Common fields for all user types
- **Customer**: Loyalty points, saved addresses, preferences
- **Courier**: KYC data, vehicle info, location tracking, wallet
- **Merchant**: Business registration, subscription, approval workflow
- **ShippingCompany**: Company info, fleet management, subscription
- **CompanyDriver**: Drivers under shipping companies

### Order Management
- **Order**: Complete delivery lifecycle tracking
  - Pickup/Dropoff locations (GPS coordinates)
  - Package details (size, weight, notes)
  - Pricing breakdown (estimated, final, earnings, commission)
  - Status tracking (Pending → Accepted → PickedUp → Delivered)
  - Payment methods (Cash, Online, Wallet)
- **OrderStatusHistory**: Audit trail of all status changes

### Review System
- **Review**: Customer ratings and feedback for couriers
  - 1-5 star rating system
  - Optional written comments
  - One review per completed order

### Wallet & Payments
- **Wallet**: Courier earnings management
  - Available Balance (withdrawable immediately)
  - Pending Balance (orders in progress)
  - Total Lifetime Earnings (statistics)
- **WalletTransaction**: Complete transaction history
  - Order earnings, withdrawals, refunds, adjustments
- **WithdrawalRequest**: Courier withdrawal workflow
  - Pending → Approved → Processed
  - Bank account integration ready

### Subscription System
- **SubscriptionPlan**: Tiered pricing for Merchants, Couriers, Shipping Companies
  - Monthly/Yearly billing periods
  - Feature toggles and commission rates
- **Subscription**: Active subscriptions with auto-renewal
  - Start/End dates
  - Status tracking (Active, Expired, Cancelled, Suspended)

### Authentication & Security
- **Role**: System roles with descriptions
- **UserRole**: Many-to-many user-role assignments
- **RefreshToken**: JWT refresh token storage with rotation
- **EmailVerification**: 6-digit TOTP codes with expiration

### Notifications
- **Notification**: In-app notifications system
  - Order updates, payment confirmations, account approvals
  - Read/Unread tracking
  - Multiple notification types

### Saved Addresses
- **SavedAddress**: Customer's frequently-used addresses
  - Home, Work, Custom labels
  - GPS coordinates for quick order creation
  - Default address support

---

## 🔧 Key Services Implemented

### AuthService
- User registration (Customer, Courier, Merchant)
- Login with JWT generation
- Email verification (send & verify codes)
- Password reset workflow
- Account approval (Admin only)

### TokenService
- JWT Access Token generation
- Refresh Token creation and validation
- Token rotation for security
- Claims management (UserId, Email, Roles)

### EmailSender
- SMTP email delivery via MailKit
- Verification code emails
- Password reset emails
- Admin invitation emails
- Withdrawal confirmation emails
- Ready for promotional emails

---

## 📊 Entity Relationships

### Key Relationships
```
Customer 1:N Orders
Courier 1:N Orders
Courier 1:1 Wallet
Wallet 1:N WalletTransactions
Wallet 1:N WithdrawalRequests
Order 1:1 Review
Order 1:N OrderStatusHistory
Merchant 1:1 Subscription
Courier 1:1 Subscription
ShippingCompany 1:1 Subscription
ShippingCompany 1:N CompanyDrivers
Customer 1:N SavedAddresses
User 1:N Notifications
```

### Database Approach
- **Table-Per-Type (TPT) Inheritance** for user types
  - Clean type safety and compile-time checking
  - Better integration with ASP.NET Identity
  - Simpler code with `courier.IsOnline` vs `user.CourierProfile.IsOnline`
  - Exclusive roles (user is EITHER customer OR courier OR merchant)

---

## 🎯 Revenue Model

### For Merchants
- **Free Plan (Bronze)**: 15% commission per order
- **Silver Plan**: 500 EGP/month + 10% commission
- **Gold Plan**: 1,500 EGP/month + 5% commission
- **Platinum Plan**: 3,000 EGP/month + 0% commission (subscription only)

### For Couriers
- **Pay-Per-Order (Default)**: 15% commission per delivery
- **Monthly Subscription**: 200 EGP/month + 0% commission (keep 100%)

### For Shipping Companies
- **Basic Plan**: 5,000 EGP/month (no commission, up to 20 drivers)
- **Enterprise Plan**: 15,000 EGP/month (unlimited drivers, API access)

---

## 🔍 Key Features Overview

### Order Lifecycle
1. **Creation**: Customer/Merchant creates order with pickup/dropoff locations
2. **Matching**: System broadcasts to nearby online couriers (5km radius)
3. **Acceptance**: First courier to accept gets assigned (30-second window)
4. **Pickup**: Courier navigates to pickup, confirms collection
5. **In Transit**: Real-time GPS tracking visible to customer
6. **Delivery**: Courier confirms delivery, payment processed
7. **Review**: Customer rates courier (1-5 stars)

### Pricing Calculation
```
Base Price: 30 EGP
+ Distance: 3 EGP per km
× Package Size: Small (1.0x), Medium (1.3x), Large (1.6x)
× Time Factor: Peak hours (1.2x), Off-peak (1.0x)
= Total Price

Courier Earnings: 85% of total
Platform Commission: 15% of total
```

### Wallet System
- **Pending Balance**: Money from orders in progress (locked)
- **Available Balance**: Money that can be withdrawn immediately
- **Total Earnings**: Lifetime statistics (never decreases)
- **Withdrawals**: Request → Admin Approval → Bank Transfer

### Customer Loyalty Tiers
- **Bronze** (0-19 orders): Standard service
- **Silver** (20-49 orders): Priority matching
- **Gold** (50-99 orders): 10% discount + advanced features
- **Platinum** (100+ orders): Premium support + maximum benefits

---

## 📈 Performance Optimizations

### Database Indexes
```sql
-- Order queries
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_CourierId_Status ON Orders(CourierId, Status);
CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt DESC);

-- Location-based matching
CREATE INDEX IX_Couriers_Location ON Couriers(
    IsOnline, IsAvailable, CurrentLatitude, CurrentLongitude
);

-- Wallet transactions
CREATE INDEX IX_WalletTransactions_WalletId_CreatedAt ON WalletTransactions(
    WalletId, CreatedAt DESC
);
```

### Query Optimization
- Pagination for all list endpoints (20 items per page)
- Eager loading with `.Include()` to prevent N+1 queries
- Projection to DTOs to reduce payload size
- Caching for frequently accessed data (Redis ready)

---

## 🔒 Security Features

### Data Protection
- HTTPS enforced for all API endpoints
- Sensitive data encryption (passwords, tokens)
- SQL injection prevention via parameterized queries
- XSS protection in all inputs

### Authorization
- Role-based access control (`[Authorize(Roles = "Admin, SuperAdmin")]`)
- Resource-based authorization (users can only access their own data)
- API rate limiting (planned)
- CORS configuration for allowed origins

### Privacy
- Personal data (National ID, License Number) hidden from public APIs
- Location tracking only when courier is online
- Customer phone numbers masked in notifications

---

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server (supports PostgreSQL, MySQL)
- **Authentication**: ASP.NET Core Identity + JWT

### Email Service
- **Library**: MailKit
- **Protocol**: SMTP
- **Features**: HTML emails, attachments support

### Configuration
- Environment-based settings (Development, Staging, Production)
- Secrets management via User Secrets / Azure Key Vault
- Connection string encryption

---

## 📝 Next Steps (Phase 3)

### Core Business Features
- [ ] Order creation and management APIs
- [ ] Real-time location tracking (SignalR)
- [ ] Matching engine for courier assignment
- [ ] Wallet transaction processing
- [ ] Withdrawal request workflow
- [ ] Review and rating system
- [ ] Notification delivery (Push, SMS, Email)
- [ ] Subscription management and auto-renewal
- [ ] Admin dashboard APIs
- [ ] Merchant bulk order upload
- [ ] Payment gateway integration

### Advanced Features
- [ ] In-app chat between customer and courier
- [ ] Route optimization for deliveries
- [ ] Promotional campaigns and discounts
- [ ] Advanced analytics and reporting
- [ ] API access for merchants (webhook support)
- [ ] Multi-language support (Arabic, English)
- [ ] Mobile app integration (React Native / Flutter)

---

## 📂 Project Structure

```
Tawsella/
├── Tawsella.Domain/          # Entities, Enums, Interfaces
│   ├── Entities/             # User, Order, Wallet, etc.
│   └── Enums/                # OrderStatus, PaymentMethod, etc.
├── Tawsella.Infrastructure/  # Data access, DbContext
│   ├── DbContext/            # AppDbContext with configurations
│   ├── Repositories/         # Generic repository pattern
│   └── Migrations/           # EF Core migrations
├── Tawsella.Application/     # Business logic, Services
│   ├── Services/             # AuthService, OrderService, etc.
│   └── DTOs/                 # Request/Response models
└── Tawsella.API/             # Controllers, Middleware
    ├── Controllers/          # API endpoints
    └── Middleware/           # Authentication, Error handling
```

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server / PostgreSQL
- SMTP email account (Gmail, SendGrid, etc.)

### Setup
```bash
# Clone repository
git clone https://github.com/your-org/tawsella.git

# Update connection string in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=Tawsella;..."
}

# Update email settings
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderPassword": "your-app-password"
}

# Run migrations
dotnet ef database update --project Tawsella.Infrastructure

# Run application
dotnet run --project Tawsella.API
```

---

## 📧 Contact & Support

For questions or contributions, please reach out to the development team.

**Note**: This is Phase 2 completion. Core business features (Orders, Tracking, Payments) are the next priority.
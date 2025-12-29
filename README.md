# 🛡️ Tawsella - Identity Module (Phase 1)

## 📌 Overview

This is the **initial phase** of the project. I have completed the full **Identity & Authentication** system. All user-related logic is ready, so we can now focus on building the core business features (Orders, Tracking, etc.).

## 🏗️ What’s Implemented?

### 1. User Types (Roles)

The system is set up to handle 4 types of users:

- **Super-Admin:** Can Create admin and send email contain *'email' & 'Password'* to Admin
    
- **Admin:** Full control.
    
- **Customer:** Regular app user.
    
- **Courier:** Delivery person (needs Admin approval).
    
- **Merchant:** Shop owner (needs Admin approval).
    

### 2. Authentication Flow

- **JWT Tokens:** Secure login system.
    
- **Refresh Tokens:** Keeps the user logged in without asking for a password every hour.
    
- **Email Verification:** Registration sends a **6-digit code** via email (using MailKit). Users can't login until they verify.
    

### 3. Key Services

- `AuthService`: Handles Login, Registration, and Password Reset.
    
- `TokenService`: Manages JWT generation and Refresh Token logic.
    
- `EmailSender`: A ready-to-use service to send any email through SMTP.

# Bizonet API — Business Networking Platform (ASP.NET Core Web API)

Bizonet is a business networking platform where professionals connect through **groups**, maintain **business profiles**, and exchange **referrals** inside trusted circles.

This repository contains the backend system built using **ASP.NET Core Web API**, designed as a modular and scalable REST API for a mobile application.

---

## 🚀 Tech Stack

- **ASP.NET Core Web API**
- **Entity Framework Core** (Code First + Migrations)
- **SQL Server**
- **JWT Authentication** (Access Token + Refresh Token)
- **OTP Email Verification** (Registration OTP + Login OTP)
- **Swagger / OpenAPI**
- **Dependency Injection + Service Layer Architecture**

---

## 🧩 System Architecture

The API follows a clean, service-based structure:

- **Controllers** → handle HTTP, routing, authorization
- **Services** → contain business logic and workflows
- **Entities** → EF Core database models
- **DTOs** → request/response contracts for frontend/mobile integration
- **Seeder** → automated lookup seeding (Countries / States / Cities / Business Categories)

---

## ✅ Business Modules & Workflows

### 1) Authentication Module

#### ✅ Registration Flow
1. User registers using email, password, and personal details
2. System creates a user record
3. OTP is generated and stored in `UserOtps`
4. OTP is emailed to the user
5. User verifies OTP → account becomes verified

#### ✅ Login Flow (One Login Endpoint, Multiple Methods)
The API supports multiple login methods via a single endpoint using a `signInMethod` field.

##### A) Password Login
- Validate credentials
- Generate **Access Token + Refresh Token**

##### B) OTP Login
- Request OTP (`signInMethod = otp`)
- Validate OTP (`signInMethod = otpValidation`)
- Generate **Access Token + Refresh Token**

---

### 2) Profile Module

#### ✅ User Profile Features
- Fetch user profile
- Update user profile including:
  - first name / last name
  - address
  - designation
  - location (country/state/city)
  - profile photo upload

---

### 3) Business Profile Module

Business information is managed separately from the user’s login identity.

#### ✅ Business Info Features
Save/update business info such as:

- business category
- company name
- company address
- business description
- company website
- business email (separate contact email)
- business phone

This supports scenarios where a user registers with personal email, while the company uses a different email for business communication.

---

### 4) Groups Module

Groups are the core networking feature in Bizonet.

#### ✅ Group Creation Workflow
1. Logged-in user creates a group
2. Group enters **Pending Approval**
3. Admin approves/rejects the group
4. Approved groups become available for join requests

#### ✅ Join Group Workflow
1. User requests to join an active group
2. Membership record is created in **Pending Approval**
3. Group owner approves/rejects the request
4. Approved users become **active members** of that group

Entities involved:
- `Groups`
- `GroupUsers`

---

### 5) Referral System Module

Referrals are exchanged **inside groups**, between active members.

#### Referral Types

✅ **Self Referral**
- Referrer shares their own business/contact details

✅ **Outsider Referral**
- Referrer shares details of an external person/business not registered in the app

#### ✅ Referral Workflow
1. Active group member sends a referral to another active member
2. Referral is created in `Referrals`
3. Outsider details are stored in `ReferralOutsiders` (only for outsider referrals)
4. Referral history and tracking is stored in `ReferralStatuses`

#### ✅ Recipient Actions
Recipient can:
- Accept referral
- Reject referral (with reject reason)

---

### 6) Follow-up Workflow

After accepting a referral, the recipient can maintain follow-up history using predefined lookup data.

Lookup tables:
- `FollowupStatuses`
- `FollowupComments`

Follow-up supports:
- scheduling the next follow-up date
- marking referral as **won / lost**
- full follow-up timeline tracking

---

## 📌 Key API Endpoints (Overview)

### Authentication
- `POST /api/auth/register`
- `POST /api/auth/verify-otp`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`

### Profiles
- `GET /api/profile/me`
- `PUT /api/profile/update`

### Business
- `POST /api/business/save`
- `GET /api/business/my`

### Groups
- `POST /api/groups/create`
- `POST /api/groups/join`
- `GET /api/groups/my`
- `GET /api/groups/{groupId}/requests`
- `POST /api/groups/{groupId}/requests/{memberId}/approve`
- `POST /api/groups/{groupId}/requests/{memberId}/reject`

### Admin Group Approval
- `GET /api/admin/groups/pending`
- `POST /api/admin/groups/{groupId}/approve`
- `POST /api/admin/groups/{groupId}/reject`

### Referrals
- `POST /api/referrals/send`
- `GET /api/referrals/inbox`
- `GET /api/referrals/sent`
- `GET /api/referrals/{referralId}/timeline`
- `POST /api/referrals/{referralId}/status`
- `POST /api/referrals/{referralId}/followup`

---

## 🧠 Author

Built by **Mohamed Adunan**  
Software Developer | ASP.NET Core | SQL Server | REST APIs  

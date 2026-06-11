# 🛍️ ShopSphere — Enterprise Full-Stack E-Commerce Platform

<p align="center">
  <img src="https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D" alt="Vue 3" />
  <img src="https://img.shields.io/badge/.NET%20Core%208.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET API" />
  <img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white" alt="Tailwind CSS" />
  <img src="https://img.shields.io/badge/Supabase-3ECF8E?style=for-the-badge&logo=supabase&logoColor=white" alt="Supabase" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
  <img src="https://img.shields.io/badge/Vercel-000000?style=for-the-badge&logo=vercel&logoColor=white" alt="Vercel" />
</p>

ShopSphere is a production-ready, full-stack e-commerce solution built using a decoupled architecture. It features a high-performance **ASP.NET Core Web API** backend and a reactive, modern **Vue 3** frontend, seamlessly integrated with **Supabase PostgreSQL** and **Supabase Storage**.

---

## 🌐 Live Demo

* 🖥️ **Frontend Web App:** [https://shopsphere-red.vercel.app](https://shopsphere-red.vercel.app)
* ⚙️ **Backend API Service:** [https://shopsphere-api-2mb2.onrender.com](https://shopsphere-api-2mb2.onrender.com)

---

## 🛠️ Tech Stack

### **Frontend Infrastructure**
* **Framework:** Vue 3 (Composition API)
* **Build Tool:** Vite (Next-Gen Frontend Tooling)
* **Styling Engine:** Tailwind CSS
* **HTTP Client:** Axios (Configured for seamless JWT management)
* **Routing:** Vue Router
* **Icons:** Font Awesome

### **Backend Architecture**
* **Framework:** ASP.NET Core Web API (RESTful Design Patterns)
* **ORM:** Entity Framework Core
* **Database:** PostgreSQL
* **Security Layer:** JWT Authentication & Role-Based Authorization
* **Containerization:** Docker

### **Cloud & Deployment**
* **Database Cluster:** Supabase PostgreSQL
* **Cloud Storage:** Supabase Storage Bucket
* **Backend Hosting:** Render (Container Deployment)
* **Frontend Hosting:** Vercel

---

## 🚀 Key Features

### 🛒 Customer Features
* **Dynamic Catalog:** Browse products smoothly with fluid user interfaces.
* **Granular Filtering:** Filter available inventory items dynamically by category.
* **Cart Management:** Add products, update quantities, and calculate totals in real time.
* **Frictionless Checkout:** Streamlined Cash on Delivery (COD) processing pipeline.
* **Order History:** Complete historical tracking logs for authenticated user purchases.

### 🛡️ Admin Features
* **Executive Dashboard:** Centralized metrics and management access.
* **Product Lifecycle (CRUD):** Complete control to add, edit, and delete products.
* **Media Asset Pipeline:** Automated, secure image uploads for product variants.
* **Category Governance:** Manage categories and update associated visual assets.
* **Order Fulfillment:** Comprehensive tracking and real-time status updates for client orders.

### 👑 Master Admin Features
* **Identity & Access Management (IAM):** Complete user roster oversight.
* **Administrative Provisioning:** Create and promote secondary administrative profiles securely.
* **Access Control:** Instantly activate or deactivate specific user accounts.

---

## 👥 User Roles & Access Matrix

| Role | Badges | Authorized Scopes & Access Boundaries |
| :--- | :--- | :--- |
| **User** | `🌐 Customer` | Product discovery, active cart context, checkout routines, order logs. |
| **Admin** | `🛡️ Management` | Full catalog manipulation, category configuration, order processing. |
| **MasterAdmin** | `👑 Super User` | Complete global access, system administration, and user account lifecycle control. |

---

## 📁 Project Structure

```txt
ShopSphere/
├── backend/
│   └── ShopSphere.Api/
│       ├── Controllers/      # REST API Controllers (RBAC Guarded)
│       ├── DTOs/             # Unified Data Transfer Objects
│       ├── Data/             # DBContext & Data Migrations Layer
│       ├── Models/           # Strongly-typed Domain Entities
│       └── Program.cs        # Middleware Pipeline & Service Container
│
├── frontend/
│   ├── public/
│   ├── src/
│   │   ├── components/       # Reusable, Atomic UI Components
│   │   ├── views/            # Dashboard & Page Layouts
│   │   ├── router/           # Guarded Navigation Mapping
│   │   ├── services/         # API Layer (Axios Context Interceptors)
│   │   └── stores/           # Central Reactive State Contexts
│   ├── package.json
│   ├── vite.config.js
│   └── vercel.json           # Production SPA Routing Rules
│
├── Dockerfile                # Multi-stage Backend Build Template
├── .dockerignore
└── README.md

```

---

## ⚙️ Local Development Setup

### 🎛️ Backend Installation

1. Navigate to the API directory:
```bash
cd backend/ShopSphere.Api

```


2. Restore core dependencies via NuGet:
```bash
dotnet restore

```


3. Initialize your environment variables or local User Secrets system:
```txt
ConnectionStrings__DefaultConnection=<Your_Supabase_PostgreSQL_String>
Jwt__Key=<Your_Secure_Super_Secret_JWT_Key_Signature>
Jwt__Issuer=ShopSphere
Jwt__Audience=ShopSphereClient
Supabase__Url=[https://your-project-id.supabase.co](https://your-project-id.supabase.co)
Supabase__ServiceRoleKey=<Your_Private_Service_Role_Key>
AllowedOrigins__0=http://localhost:5173
AllowedOrigins__1=[https://shopsphere-red.vercel.app](https://shopsphere-red.vercel.app)

```


> ⚠️ **Security Warning:** Do not commit secret keys or database passwords to public repositories.


4. Launch the local runtime server:
```bash
dotnet run

```


*The server boots up locally at:* `http://localhost:5106`

---

### 💻 Frontend Installation

1. Navigate to the frontend directory:
```bash
cd frontend

```


2. Install the node package dependencies:
```bash
npm install

```


3. Create a local development configuration file `.env` in the root frontend directory:
```env
VITE_API_BASE_URL=http://localhost:5106/api
# Production Mapping: VITE_API_BASE_URL=[https://shopsphere-api-2mb2.onrender.com/api](https://shopsphere-api-2mb2.onrender.com/api)

```


4. Start the Vite hot-reloading development server:
```bash
npm run dev

```


*The application will render natively at:* `http://localhost:5173`

---

## 🗄️ Relational Database Schema

The persistent storage tier is hosted on **Supabase PostgreSQL**, driven by the following core structural tables:

* `app_users` — Normalized customer profiles, authentication records, and system roles.
* `categories` — Product classifications and metadata.
* `products` — Core catalog inventory records.
* `carts` — Active client session references.
* `cart_items` — Relational joins detailing specific item allocations inside customer carts.
* `orders` — Permanent historical invoices with stateful tracking flags.
* `order_items` — Granular mapping of products included in a completed purchase order.

---

## 📦 Cloud Storage Infrastructure

Product and category media assets are pushed asynchronously to **Supabase Storage** buckets through dedicated API handlers. Transactions utilize an isolated, server-side **Service Role Key** ensuring that sensitive storage read/write bucket privileges are completely hidden from the client browser context.

---

## 📡 API Endpoints Spec Sheet

### 🔓 Public Routes

* `GET /api/products` — Fetch full product list.
* `GET /api/products/{id}` — Specific product inspection.
* `GET /api/categories` — Fetch all product categories.
* `POST /api/auth/register` — Account creation pipeline.
* `POST /api/auth/login` — Authentication gateway (Returns valid JWT token).

### 👤 Customer Protected Routes (`User` Auth Required)

* `GET /api/cart/{userId}` — Retrieve active cart context.
* `POST /api/cart/add` — Append product item to current cart.
* `PUT /api/cart/items/{cartItemId}` — Modify item volume.
* `DELETE /api/cart/items/{cartItemId}` — Drop item from cart instance.
* `POST /api/orders/checkout` — Finalize order execution via COD.
* `GET /api/orders/user/{userId}` — Pull user specific purchase history.

### 🛡️ Admin Restricted Routes (`Admin` / `MasterAdmin` Auth Required)

* `GET /api/admin/products` | `POST /api/admin/products` — View / Create products.
* `PUT /api/admin/products/{id}` | `DELETE /api/admin/products/{id}` — Modify / Remove target products.
* `GET /api/admin/categories` | `POST /api/admin/categories` — View / Build new categories.
* `PUT /api/admin/categories/{id}` | `DELETE /api/admin/categories/{id}` — Modify / Drop categories.
* `GET /api/admin/orders` — Audit global client orders.
* `PUT /api/admin/orders/{id}/status` — Transition order fulfillment state.
* `POST /api/admin/uploads/product-image` — Stream product image binaries to Supabase.
* `POST /api/admin/uploads/category-image` — Stream category asset binaries to Supabase.

### 👑 Master Admin Overlord Routes (`MasterAdmin` Only)

* `GET /api/master/users` — Comprehensive platform user roster inspection.
* `POST /api/master/users` — Direct administrative profile provisioning.
* `PUT /api/master/users/{id}/status` — Enforce global account lockouts or activations.

---

## 🔒 Security Architecture Blueprint

* **Cryptographic Authorization:** Secure stateless session validation via JWT signatures.
* **Dual Layer Route Defenses:** Component routing hooks prevent navigation visually on the client-side, while strict controller attributes re-verify identity token signatures on every API request server-side.
* **Strict CORS Contracts:** Explicit domain whitelisting enforces strict cross-origin access blocks, accepting traffic exclusively from the secure Vercel client deployment ecosystem.

---

## 🎯 Future Enhancements Roadmap

* [ ] Integration of Online Payment Processing Gateways (Stripe, PayPal).
* [ ] Implementation of full-text indexing for advanced search and elastic queries.
* [ ] Customer Wishlist context scopes.
* [ ] Interactive User Ratings matrix and review tracking per catalog item.
* [ ] Rich Admin Analytics charts summarizing transactional velocity.
* [ ] Automated Order Confirmation and status update Email/SMS pipelines.

---

### 👤 Developer & Architect

**Argho Chakma** *Full-Stack Software Engineer focused on crafting performant, scalable digital platforms.*

```

```
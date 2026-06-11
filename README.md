# ShopSphere

ShopSphere is a full-stack e-commerce web application built with **ASP.NET Core Web API**, **Vue 3**, **Tailwind CSS**, and **Supabase PostgreSQL**. It includes customer shopping features, role-based authentication, admin product/order/category management, image upload, cart, checkout, and order tracking.

## Live Demo

**Frontend:** https://shopsphere-red.vercel.app
**Backend API:** https://shopsphere-api-2mb2.onrender.com

## Tech Stack

### Frontend

* Vue 3
* Vite
* Tailwind CSS
* Axios
* Vue Router
* Font Awesome

### Backend

* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* Role-Based Authorization
* Docker

### Cloud & Deployment

* Supabase PostgreSQL
* Supabase Storage
* Render for backend deployment
* Vercel for frontend deployment

## Key Features

### Customer Features

* Browse products
* View product details
* Filter products by category
* User registration and login
* Add products to cart
* Update cart quantity
* Checkout with Cash on Delivery
* View order history

### Admin Features

* Admin dashboard access
* Add, edit, and delete products
* Upload product images
* Manage product categories
* Upload, edit, and remove category images
* Manage customer orders
* Update order status

### Master Admin Features

* Manage users
* Create admin users
* Activate or deactivate users
* Role-based access control

## User Roles

| Role        | Access                                  |
| ----------- | --------------------------------------- |
| User        | Products, cart, checkout, order history |
| Admin       | Product, category, and order management |
| MasterAdmin | All admin features plus user management |

## Project Structure

```txt
ShopSphere/
├── backend/
│   └── ShopSphere.Api/
│       ├── Controllers/
│       ├── DTOs/
│       ├── Data/
│       ├── Models/
│       └── Program.cs
│
├── frontend/
│   ├── public/
│   ├── src/
│   │   ├── components/
│   │   ├── views/
│   │   ├── router/
│   │   ├── services/
│   │   └── stores/
│   ├── package.json
│   ├── vite.config.js
│   └── vercel.json
│
├── Dockerfile
├── .dockerignore
└── README.md
```

## Backend Setup

### 1. Navigate to backend project

```bash
cd backend/ShopSphere.Api
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Add required user secrets or environment variables

Required backend configuration:

```txt
ConnectionStrings__DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
Supabase__Url
Supabase__ServiceRoleKey
AllowedOrigins__0
AllowedOrigins__1
```

Example:

```txt
Jwt__Issuer=ShopSphere
Jwt__Audience=ShopSphereClient
Supabase__Url=https://your-project-id.supabase.co
```

Do not commit secret keys or database passwords to GitHub.

### 4. Run backend locally

```bash
dotnet run
```

Backend will run locally at:

```txt
http://localhost:5106
```

## Frontend Setup

### 1. Navigate to frontend

```bash
cd frontend
```

### 2. Install dependencies

```bash
npm install
```

### 3. Create `.env` file

```env
VITE_API_BASE_URL=http://localhost:5106/api
```

For production:

```env
VITE_API_BASE_URL=https://shopsphere-api-2mb2.onrender.com/api
```

### 4. Run frontend locally

```bash
npm run dev
```

Frontend will run locally at:

```txt
http://localhost:5173
```

## Database

The project uses **Supabase PostgreSQL**. Main tables include:

* `app_users`
* `categories`
* `products`
* `carts`
* `cart_items`
* `orders`
* `order_items`

## Storage

Product and category images are uploaded to **Supabase Storage** through the backend API using a service role key. The service role key is stored only in backend environment variables.

## API Endpoints

### Public

```txt
GET /api/products
GET /api/products/{id}
GET /api/categories
POST /api/auth/register
POST /api/auth/login
```

### User

```txt
GET /api/cart/{userId}
POST /api/cart/add
PUT /api/cart/items/{cartItemId}
DELETE /api/cart/items/{cartItemId}
POST /api/orders/checkout
GET /api/orders/user/{userId}
```

### Admin

```txt
GET /api/admin/products
POST /api/admin/products
PUT /api/admin/products/{id}
DELETE /api/admin/products/{id}

GET /api/admin/categories
POST /api/admin/categories
PUT /api/admin/categories/{id}
DELETE /api/admin/categories/{id}

GET /api/admin/orders
PUT /api/admin/orders/{id}/status

POST /api/admin/uploads/product-image
POST /api/admin/uploads/category-image
```

### Master Admin

```txt
GET /api/master/users
POST /api/master/users
PUT /api/master/users/{id}/status
```

## Deployment

### Backend Deployment

The backend is deployed on **Render** using Docker.

Backend production URL:

```txt
https://shopsphere-api-2mb2.onrender.com
```

### Frontend Deployment

The frontend is deployed on **Vercel**.

Frontend production URL:

```txt
https://shopsphere-red.vercel.app
```

### CORS Configuration

The backend allows requests from the deployed Vercel frontend:

```txt
https://shopsphere-red.vercel.app
```

## Security Notes

* JWT authentication is used for protected routes.
* Role-based authorization is implemented for Admin and MasterAdmin features.
* User cart and order routes are protected using the authenticated user ID.
* Supabase service role key is never exposed in frontend code.
* Environment variables are used for all sensitive configuration.

## Future Improvements

* Online payment gateway integration
* Product search and advanced filtering
* Wishlist feature
* Product reviews and ratings
* Admin analytics dashboard
* Email confirmation for orders
* Responsive dashboard improvements

## Author

Developed as a full-stack portfolio project using ASP.NET Core, Vue 3, Tailwind CSS, Supabase, Render, and Vercel.

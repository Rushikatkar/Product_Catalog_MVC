# Product Catalog Module

## Overview

The **Product Catalog Module** is an essential part of the ASP.NET Core MVC web application, providing users with the ability to browse, search, and filter products. Additionally, administrators have full control over product management, including creating, updating, and deleting products.

## Features

### 🛍️ Product Management
- Display a **paginated list** of products.
- **Search & filter** by category, price range, and availability.
- View detailed **product descriptions and images**.

### 🔑 Authentication & Authorization
- **Role-based authentication**: Only authenticated users can access the catalog.
- **Admin privileges**: Add, update, and delete products.

### ⚡ Database Integration
- **Entity Framework Core** with SQL Server.
- Tables: `Products`, `Categories`, `ProductImages`.

### 📌 Admin Product Management
- Secure **CRUD operations** for products.
- **Client-side and server-side validation**.


## Installation & Setup

### 🚀 Step 1: Clone the Repository
```bash
git clone <repository-url>
```

### 🛠 Step 2: Configure Database
- Run the **SQL script** above to set up the tables.
- Update the **connection string** in `appsettings.json`.

### ▶ Step 3: Run the Application
- Open in **Visual Studio** or any compatible IDE.
- **Build & Run** the project.
- Navigate to `http://localhost:<port>`.

## Future Enhancements
- **Image upload functionality** for products.
- **AJAX-powered pagination & search** for a smoother experience.
- **Caching** for optimized performance.

---
**🔗 Contribute & Feedback**  
Feel free to contribute to this module or report any issues! 🚀

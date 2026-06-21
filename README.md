# HiTech Store Backend

Backend implementation of a digital products e-commerce platform focused on computers, laptops, smartphones, smart gadgets, and accessories.

Built with **ASP.NET Core**, **Entity Framework Core**, and **PostgreSQL**, with a strong emphasis on modularity, maintainability, and extensibility.

---

## 🚀 Overview

HiTech Store is a personal learning and portfolio project designed to explore modern backend development concepts and enterprise application patterns.

The solution consists of multiple projects, each responsible for a dedicated domain within the ecosystem.

```text
HiTechStore.sln
│
├── HiTechStore
├── HiTechPay
├── HiTechStore.ApiTokenHandler
└── HiTechPay.SDK
```

---

## 🏗 Architecture

The project follows a hybrid architectural approach.

Initially, the project started with a traditional layered architecture based on SOLID principles. As development progressed and my understanding of Clean Architecture improved, newer modules were gradually redesigned to follow Clean Architecture concepts.

As a result:

- Older modules follow a layered architecture.
- Newer modules are organized around Clean Architecture boundaries.
- Authentication functionality is separated into an independent module inspired by Modular Monolith principles.

This repository therefore reflects both the project's evolution and my learning journey.

---

## 📦 Projects

### HiTechStore

Main REST API of the e-commerce platform.

**Responsibilities**

- Product management
- Category & brand management
- Shopping cart
- Order processing
- Media management
- Authentication integration
- Payment integration

---

### HiTechPay

A lightweight payment gateway simulator built with ASP.NET Core Razor Pages.

Used during development and testing to emulate a real payment provider.

Supported outcomes:

- ✅ Successful payment
- ❌ Failed payment

After selection, the simulator redirects users back to the store with the corresponding payment result.

---

### HiTechStore.ApiTokenHandler

Dedicated authentication module inspired by Modular Monolith Architecture.

**Responsibilities**

- Access token generation
- Refresh token generation
- Token validation
- Token rotation
- Authentication utilities

Separating authentication logic improves maintainability and promotes reuse across services.

---

### HiTechPay.SDK

Client SDK used for communication with HiTechPay.

**Features**

- Payment request creation
- Payment verification
- Public key exchange
- Encryption / Decryption
- Secure communication

Provides a clean abstraction layer between the store and payment provider.

---

## ✨ Key Features

### Dynamic Discount Engine

Unlike traditional fixed discount implementations, HiTech Store supports script-based discount rules.

Discount codes can execute custom business logic at runtime using the Roslyn Compiler Platform.

Examples include:

- First purchase discounts
- VIP customer campaigns
- Product-specific discounts
- Time-limited promotions
- Cart-value discounts
- Custom business rules

---

### Product Media Management

Supports:

- Multiple product images
- Video uploads
- Automatic video thumbnail generation

---

### Category & Brand Management

Flexible management of product categories and brands with hierarchical organization support.

---

### Shopping Cart

Complete shopping cart workflow including:

- Item management
- Quantity updates
- Price calculations
- Discount application

---

### JWT Authentication

Authentication is implemented using:

- JWT Access Tokens
- Refresh Tokens
- Token rotation

for improved security and user experience.

---

### Payment Gateway Abstraction

Payment processing is isolated through HiTechPay.SDK, making it possible to replace the payment provider with minimal changes to business logic.

---

### Docker Support

Docker images are available for executable services, simplifying local development and deployment.

---

## 🛠 Technologies

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Razor Pages
- Roslyn Compiler Platform
- JWT Authentication
- Refresh Token Authentication
- Docker

---

## 🗄 Database

The application uses:

- PostgreSQL as the primary relational database
- Entity Framework Core as the ORM

EF Core migrations are fully supported.

---

## ⚙ Configuration

### Storage Strategies

```csharp
public enum StorageStrategy
{
    Local,
    Supabase
}
```

| Configuration Key            | Required      | Description                              |
| ---------------------------- | ------------- | ---------------------------------------- |
| FormLimit                    | No            | Maximum upload size for multipart forms  |
| StorageStrategy              | Yes           | Storage provider (`Local` or `Supabase`) |
| Supabase:BaseUrl             | Supabase only | Supabase project URL                     |
| Supabase:SecretKey           | Supabase only | Secret API key used by backend           |
| Supabase:BucketName          | Supabase only | Storage bucket name                      |
| PublicAccessUrl              | Yes           | Public API URL                           |
| PaymentServer:Url            | Payment only  | Payment server address                   |
| PaymentServer:KeyStoragePath | Payment only  | Path used for public keys                |

### Supported Providers

#### Local Storage

Stores files directly within the application storage.

#### Supabase Storage

Stores files in a Supabase Storage bucket and serves them through public URLs.

---

## 🐳 Docker Images

Pre-built Docker images are available on Docker Hub.

### HiTechStore API

```bash
docker run -d \
  --name hitechstore-api \
  pourianof/hitechstore-api:latest
```

### HiTechPay

```bash
docker run -d \
  --name hitechpay \
  pourianof/hitechpay:latest
```

---

## 🔗 Related Repositories

### Frontend

The frontend is implemented separately using Next.js.

GitHub Repository:

https://github.com/Pourianof/hi_tech_store_next

### Docker Hub

https://hub.docker.com/repositories/pourianof

---

## 🎯 Project Goals

This project serves as a platform for exploring and applying modern backend development concepts, including:

- Clean Architecture
- Modular Monolith Architecture
- Domain Separation
- Secure Authentication
- Payment Integration
- Docker & CI/CD
- Dynamic Scripting with Roslyn
- Media Processing

The architecture and implementation continue to evolve as new concepts and improvements are introduced.

---

## 🤝 Contributing

Although this project is primarily a personal learning and portfolio project, feedback, suggestions, and pull requests are always welcome.

If you discover a bug or have an idea for improvement, feel free to open an Issue.

---

## ⚠ Disclaimer

This project is intended for educational and portfolio purposes.

It should not be considered production-ready without additional security reviews, testing, monitoring, and performance optimization.

---

## 📄 License

Released for educational and portfolio purposes.

# HiTech Store Backend

HiTech Store is the backend implementation of a digital products e-commerce platform, focused on smart devices such as computers, laptops, mobile phones, smart gadgets and their accessories.

The project is developed using **ASP.NET Core**, **Entity Framework Core**, and **PostgreSQL**, with an emphasis on maintainability, modularity, and extensibility.

---

## ✨ Overview

This repository contains the backend services required to run the HiTech Store ecosystem.

It consists of multiple projects, each responsible for a dedicated part of the system.

```
HiTechStore.sln
│
├── HiTechStore
├── HiTechPay
├── HiTechStore.ApiTokenHandler
└── HiTechPay.SDK
```

---

# Projects

## HiTechStore

The main REST API of the online store.

This project is implemented with an architecture inspired by **Clean Architecture** principles.

> **Note**
>
> The project was initially started before I became familiar with Clean Architecture. After learning its concepts during development, I gradually migrated newer sections toward this architecture.
>
> As a result, some older parts of the project still follow a more traditional **SOLID-oriented layered design**, while newer modules try to respect Clean Architecture boundaries.

---

## HiTechPay

A standalone payment simulator service built with **ASP.NET Core Razor Pages**.

Its purpose is to simulate an external payment gateway during development and testing.

The payment page simply allows the user to select between:

- ✅ Success
- ❌ Failed

and redirects back to the store with the corresponding payment result.

---

## HiTechStore.ApiTokenHandler

A separate authentication module designed according to **Modular Monolithic Architecture** concepts.

Its responsibility is handling:

- Refresh Token generation
- Access Token generation
- Token validation
- Token rotation
- Authentication-related utilities

Separating this module makes the authentication subsystem more maintainable and reusable.

---

## HiTechPay.SDK

A client SDK for communicating with **HiTechPay**.

It provides a standardized integration layer for:

- Secure communication
- Public key exchange
- Encryption/Decryption
- Payment request creation
- Payment verification

allowing the store API to interact with the payment simulator in a clean and reusable way.

---

# Technologies

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Razor Pages
- Roslyn Compiler Platform
- JWT Authentication
- Refresh Token Authentication
- Docker Support

---

# Database

The project uses **PostgreSQL** as its primary relational database and **Entity Framework Core** as the ORM.

Migrations are fully supported through EF Core.

---

# Interesting Features

## Script-Based Discount Engine

The discount system supports **dynamic script-based discount codes** instead of fixed discount types.

Discount rules can be written as scripts and interpreted at runtime, providing a highly flexible way to implement promotional campaigns.

The scripting engine is powered by **Roslyn**, allowing C# scripts to be compiled and executed dynamically.

Examples of possible rules:

- First purchase discounts
- VIP customer discounts
- Time-limited campaigns
- Product-specific discounts
- Cart total based discounts
- Complex business rules

---

## Video Media Support

Products can contain video media.

The system automatically generates thumbnails for uploaded videos, making media management easier for the frontend.

---

## Product Gallery

Support for multiple images and media files for each product.

---

## Category & Brand Management

Flexible management of product categories and brands with hierarchical organization.

---

## Shopping Cart

Complete shopping cart workflow including quantity management and price calculation.

---

## User Authentication

Authentication based on **JWT Access Tokens** and **Refresh Tokens** for improved security and user experience.

---

## Payment Gateway Abstraction

The payment process is abstracted from the main store logic through the **HiTechPay.SDK**, making it easy to replace the payment provider in the future.

---

## Docker Support

The project includes Docker support for the executable services, making local development and deployment significantly easier.

---

# Purpose

This project is primarily intended as a personal learning and portfolio project while exploring modern backend development concepts including:

- Clean Architecture
- Modular Monolith Architecture
- Domain separation
- Secure authentication
- Payment integration
- Docker & CI/CD
- Dynamic scripting with Roslyn
- Media processing

The architecture and implementation continue to evolve as new concepts and improvements are introduced.

---

## Frontend

The frontend of this project is developed separately using **Next.js** and is available in the following repository:

**GitHub Repository**

[hi_tech_store_next](https://github.com/Pourianof/hi_tech_store_next?utm_source=chatgpt.com)

---

# Docker Images

Pre-built Docker images for the executable services are published on Docker Hub and can be pulled directly without building the source code.

**Docker Hub Repository**

[Pourianof Docker Hub Repository](https://hub.docker.com/repositories/pourianof?utm_source=chatgpt.com)

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

# Deployment

Docker Compose configurations supporting multiple deployment scenarios (development, production, and other environments) are planned and will be published in a separate repository in the near future.

The goal is to provide a simple one-command deployment experience for the complete ecosystem.

---

## Configuration

The application supports multiple storage strategies for public assets. The following configuration keys are available:

| Configuration Key     | Required            | Description                                                                                                     | Example                            |
| --------------------- | ------------------- | --------------------------------------------------------------------------------------------------------------- | ---------------------------------- |
| `StorageStrategy`     | Yes                 | Specifies the storage strategy for public assets. Supported values are `Local` and `Supabase`.                  | `Supabase`                         |
| `Supabase:BaseUrl`    | Only for `Supabase` | Base URL of the Supabase project.                                                                               | `https://your-project.supabase.co` |
| `Supabase:SecretKey`  | Only for `Supabase` | Supabase Secret API Key used by the backend to access the Storage API. **Never expose this key to the client.** | `sb_secret_xxxxxxxxxxxxxxxxx`      |
| `Supabase:BucketName` | Only for `Supabase` | Name of the Storage Bucket used for storing public assets.                                                      | `hitechstore-assets`               |
| `PublicAccessUrl`     | Yes                 | Public Url which out-world can used to access this api                                                          | `https://my-api-server.com`        |

### Storage Strategies

```csharp
public enum StorageStrategy
{
    Local,
    Supabase
}
```

- `Local`: Stores uploaded files in the application's local storage (e.g. `wwwroot`).
- `Supabase`: Stores uploaded files in a Supabase Storage bucket and serves them through public URLs.

---

# Contributing

This project is primarily developed as a learning and portfolio project, but suggestions, ideas, and constructive feedback are always welcome.

If you discover a bug or have an idea for improvement, feel free to open an Issue or submit a Pull Request.

---

# Disclaimer

This project is intended for educational and portfolio purposes and is **not intended for production use without further hardening, security review, and optimization**.

# License

This project is released for educational and portfolio purposes.

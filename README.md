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

# License

This project is released for educational and portfolio purposes.

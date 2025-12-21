# HiTech Solution

A full-stack e-commerce platform built with .NET, featuring a product store (HiTechStore) and an integrated payment system simulator (HiTechPay) along with an SDK for payment integration.

## Features

- **HiTechStore**: Complete e-commerce backend with product management, categories, brands, shopping cart, orders, and user authentication.
- **HiTechPay**: A dummy payment system simulator
- **HiTechPay.Sdk**: SDK for easy integration of payment services into other applications.
- **Database**: Entity Framework Core with migrations for data persistence.
- **Docker Support**: Containerized deployment options.

## Architecture

The solution consists of three main projects:

- **HiTechStore**: ASP.NET Core Web API for the e-commerce store.
- **HiTechPay**: ASP.NET Core Web API for payment processing.
- **HiTechPay.Sdk**: Class library providing payment integration utilities.

## Prerequisites

- .NET 8.0 or later (check `global.json` for exact version)
- Node.js 16+ (for payment simulator frontend development)
- Docker (optional, for containerized deployment)
- PostgreSQL or compatible database (configured in `appsettings.json`)

## Installation

1. Clone the repository:

   ```bash
   git clone <repository-url>
   cd HiTechSolution
   ```

2. Restore .NET dependencies:

   ```bash
   dotnet restore
   ```

3. Install frontend dependencies:

   ```bash
   cd HiTechPay/frontend
   npm install
   cd ../..
   ```

4. Update database connection strings in `appsettings.json` files if needed.
5. Run database migrations:
   ```bash
   dotnet ef database update --project ./HiTechStore
   ```

## Running the Application

For both `HiTechStore` and `HiTechPay` projects to run, run them in two seperate bash:

```bash
dotnet run --project ./HiTechStore
```

and

```bash
dotnet run --project ./HiTechPay
```

### Development Mode

````bash
# Terminal 1: HiTechStore
cd HiTechStore
dotnet watch run

# Terminal 2: HiTechPay
cd HiTechPay
dotnet watch run


### Production Mode

```bash
# Build and run HiTechStore
cd HiTechStore
dotnet build --configuration Release
dotnet run --configuration Release

# Build and run HiTechPay
cd HiTechPay
dotnet build --configuration Release
dotnet run --configuration Release
````

### Docker (if configured)

```bash
docker-compose up --build
```

## API Documentation

### HiTechStore Endpoints

- `/api/products` - Product management
- `/api/categories` - Category management
- `/api/brands` - Brand management
- `/api/carts` - Shopping cart operations
- `/api/orders` - Order management
- `/api/auth` - Authentication

## SDK Usage

To use the HiTechPay SDK in your project:

```csharp
using HiTechPay.Sdk;

// Register services
builder.Services.UseHiTechPaySdk();
```

## Configuration

- **HiTechStore**: Configure database and authentication in `appsettings.json`
- **HiTechPay**: Configure payment keys and RSA settings in `appsettings.json` and key files
- **Frontend**: Configure API endpoints in frontend configuration

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please open an issue in this repository.

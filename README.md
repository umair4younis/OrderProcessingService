# OrderProcessingService Solution

This repository contains a .NET 9 solution for an order processing system, including both the main API project and its associated test project.

## Projects

### 1. OrderProcessingService

A minimal, modern ASP.NET Core Web API for processing product orders. It supports order placement, confirmation, shipping, and status tracking, with a simple API key authentication mechanism.

**Key Features:**
- Place, confirm, ship, and check status of orders
- Product and order management using Entity Framework Core (SQLite by default)
- API documentation via Swagger (OpenAPI)
- Simple API key authentication for demonstration

**Tech Stack:**
- .NET 9
- ASP.NET Core Minimal APIs
- Entity Framework Core (SQLite)
- Swagger/OpenAPI

**Getting Started:**

1. **Clone the repository:**

2. **Run the API:**


3. **Access Swagger UI:**
   - Navigate to `https://localhost:5001/swagger` in your browser.

4. **Authentication:**
   - Use the header `X-API-KEY: local-dev-key` for all API requests.

**Default Products Seeded:**
- Keyboard (10 in stock)
- Mouse (20 in stock)

**Example Endpoints:**
- `POST /orders` - Place a new order
- `GET /orders/{id}` - Get order details
- `GET /orders/{id}/status` - Get order status
- `POST /orders/{id}/confirm` - Confirm an order
- `POST /orders/{id}/ship` - Ship an order

---

### 2. OrderProcessingService.Tests

A test project using xUnit for unit and integration testing of the API.

**Key Features:**
- xUnit-based tests
- Integration tests using WebApplicationFactory
- Code coverage via Coverlet

**Running Tests:**


---

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- (Optional) SQLite (default, in-memory for dev)

## Contributing

Pull requests are welcome. For major changes, please open an issue first.

## License

This project is licensed under the MIT License.
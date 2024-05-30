# Minimal API with Entity Framework Core and SQLite

This project is a Minimal API built with ASP.NET Core that uses Entity Framework Core for data access and SQLite as the database. The API manages `User` and `Order` entities and includes basic CRUD operations.

## Features

- Minimal API structure using ASP.NET Core.
- Entity Framework Core for data access.
- SQLite as the database.
- CRUD operations for `User` and `Order` entities.
- Swagger for API documentation and testing.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/download.html)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

### Setup

1. **Clone the repository:**

    ```bash
    git clone https://github.com/yourusername/minimal-api-database.git
    cd minimal-api-database
    ```

2. **Restore the dependencies:**

    ```bash
    dotnet restore
    ```

3. **Apply migrations to create the database:**

    ```bash
    dotnet ef database update
    ```

4. **Run the application:**

    ```bash
    dotnet run
    ```

### Project Structure

- `Models`: Contains the `User` and `Order` entity classes.
- `Data`: Contains the `ApplicationDbContext` class for EF Core.
- `Migrations`: Contains the EF Core migration files.
- `Program.cs`: Configures the services and middleware, and defines the endpoints.

### Endpoints

#### Users

- **GET /users**: Retrieve all users.
- **GET /users/{id}**: Retrieve a user by ID.
- **POST /users**: Create a new user.
- **PUT /users/{id}**: Update an existing user.
- **DELETE /users/{id}**: Delete a user by ID.

#### Orders

- **GET /orders**: Retrieve all orders.
- **GET /orders/{id}**: Retrieve an order by ID.
- **POST /orders**: Create a new order.
- **PUT /orders/{id}**: Update an existing order.
- **DELETE /orders/{id}**: Delete an order by ID.

### Using Swagger

Once the application is running, you can use Swagger to explore and test the API endpoints. Swagger will be available at:


### Database Management

The application uses SQLite for the database. The connection string is configured in `Program.cs`:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={AppDomain.CurrentDomain.BaseDirectory}app.db");
});

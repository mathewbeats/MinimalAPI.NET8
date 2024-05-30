using Microsoft.EntityFrameworkCore;
using MinimalApiDatabase;
using MinimalApiDatabase.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configurar el DbContext con SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={AppDomain.CurrentDomain.BaseDirectory}app.db");
});

// Agregar servicios para Endpoints y Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar servicios de autorización
builder.Services.AddAuthorization();

// Configurar JSON para manejar ciclos de referencia
//builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
//{
//    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//});

var app = builder.Build();

// Configurar Swagger para documentación de API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar autorización
app.UseAuthorization();

// Definir endpoints para `User`
app.MapGet("/users", async (ApplicationDbContext db) =>
    await db.Users.Include(u => u.Orders).ToListAsync())
    .WithName("GetAllUsers")
    .WithTags("User");

app.MapGet("/users/{id}", async (int id, ApplicationDbContext db) =>
    await db.Users.Include(u => u.Orders).FirstOrDefaultAsync(u => u.Id == id)
        is User user
            ? Results.Ok(new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                BirthDate = user.BirthDate,
                Orders = user.Orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    UserId = o.UserId
                }).ToList()
            })
            : Results.NotFound())
    .WithName("GetUserById")
    .WithTags("User");

app.MapPost("/users", async (UserDto userDto, ApplicationDbContext db) =>
{
    var user = new User
    {
        FirstName = userDto.FirstName,
        LastName = userDto.LastName,
        Email = userDto.Email,
        BirthDate = userDto.BirthDate,
        Orders = userDto.Orders.Select(o => new Order
        {
            OrderDate = o.OrderDate,
            UserId = o.UserId
        }).ToList()
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    userDto.Id = user.Id;
    return Results.Created($"/users/{user.Id}", userDto);
})
.WithName("CreateUser")
.WithTags("User");

app.MapPut("/users/{id}", async (int id, UserDto updatedUser, ApplicationDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    user.FirstName = updatedUser.FirstName;
    user.LastName = updatedUser.LastName;
    user.Email = updatedUser.Email;
    user.BirthDate = updatedUser.BirthDate;
    user.Orders = updatedUser.Orders.Select(o => new Order
    {
        OrderDate = o.OrderDate,
        UserId = o.UserId
    }).ToList();

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("UpdateUser")
.WithTags("User");

app.MapDelete("/users/{id}", async (int id, ApplicationDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is not null)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
})
.WithName("DeleteUser")
.WithTags("User");

// Definir endpoints para `Order`
app.MapGet("/orders", async (ApplicationDbContext db) =>
    await db.Orders.Include(o => o.User).ToListAsync())
    .WithName("GetAllOrders")
    .WithTags("Order");

app.MapGet("/orders/{id}", async (int id, ApplicationDbContext db) =>
    await db.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id)
        is Order order
            ? Results.Ok(order)
            : Results.NotFound())
    .WithName("GetOrderById")
    .WithTags("Order");

app.MapPost("/orders", async (OrderDto orderDto, ApplicationDbContext db) =>
{
    var order = new Order
    {
        OrderDate = orderDto.OrderDate,
        UserId = orderDto.UserId
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    orderDto.Id = order.Id;
    return Results.Created($"/orders/{order.Id}", orderDto);
})
.WithName("CreateOrder")
.WithTags("Order");

app.MapPut("/orders/{id}", async (int id, OrderDto updatedOrder, ApplicationDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    if (order is null) return Results.NotFound();

    order.OrderDate = updatedOrder.OrderDate;
    order.UserId = updatedOrder.UserId;
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("UpdateOrder")
.WithTags("Order");

app.MapDelete("/orders/{id}", async (int id, ApplicationDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    if (order is not null)
    {
        db.Orders.Remove(order);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
})
.WithName("DeleteOrder")
.WithTags("Order");

// Aplicar migraciones y crear la base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();

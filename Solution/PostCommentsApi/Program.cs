using Microsoft.EntityFrameworkCore;
using PostCommentsApi.Services;
using System.Text.Json.Serialization;
using PostCommentsApi.Data; // Add this if it's missing (for AppDbContext)

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

// Add Scoped services for your PostService
builder.Services.AddScoped<IPostService, PostService>();

// Register Swagger UI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Replace with the origin of your frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure MySQL with Entity Framework (Choose one of the below based on your preference)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23)) // specify the MySQL version here
    ));

// OR, if you want to use an in-memory database (comment out the MySQL part and use the following):
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseInMemoryDatabase("PostCommentsDb"));

var app = builder.Build();

// Enable Swagger in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS policy
app.UseCors("AllowSpecificOrigin");

// Enable HTTPS Redirection and Authorization middleware
app.UseHttpsRedirection();
app.UseAuthorization();

// Map Controllers to handle requests
app.MapControllers();

// Start the application
app.Run();

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

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

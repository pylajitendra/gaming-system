using GameService.API.Controllers;
using GameService.API.Data;
using GameService.API.interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================= CONTROLLERS + SWAGGER =================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ================= CORS =================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ================= INTERNAL API CLIENT =================

builder.Services.AddHttpClient<
    IInternalApiClient,
    GameController.InternalApiClient>();

// ================= DATABASE =================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// ================= BUILD APP =================

var app = builder.Build();

// ================= MIDDLEWARE =================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

// Redirect root to swagger

app.MapGet("/",
    () => Results.Redirect("/swagger"));

// HTTPS

app.UseHttpsRedirection();

// ================= ENABLE CORS =================

app.UseCors("AngularPolicy");

// ================= AUTHORIZATION =================

app.UseAuthorization();

// ================= MAP CONTROLLERS =================

app.MapControllers();

// ================= RUN APP =================

app.Run();
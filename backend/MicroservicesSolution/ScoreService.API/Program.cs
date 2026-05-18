using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ScoreService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// ================= CORS =================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// ================= RABBITMQ =================

builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory()
    {
        HostName = "localhost"
    };

    return factory.CreateConnection();
});

builder.Services.AddSingleton<IModel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();

    var channel = connection.CreateModel();

    channel.QueueDeclare(
        queue: "score-queue",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    return channel;
});

builder.Services.AddScoped<IMessagePublisher>(sp =>
{
    var channel = sp.GetRequiredService<IModel>();

    return new MessagePublisher(channel);
});

var app = builder.Build();


// ================= USE CORS =================

app.UseCors("AllowAngular");

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

var scores = app.MapGroup("/scores");


// ================= GET ALL =================

scores.MapGet("/", async (AppDbContext db) =>
{
    var result = await db.Scores.ToListAsync();

    return Results.Ok(result);
});


// ================= GET BY ID =================

scores.MapGet("/{id}", async (int id, AppDbContext db) =>
{
    var score = await db.Scores.FindAsync(id);

    if (score is null)
        return Results.NotFound();

    return Results.Ok(score);
});


// ================= POST =================

scores.MapPost("/", async (
    Score score,
    AppDbContext db,
    IMessagePublisher publisher) =>
{
    score.CreatedAt = DateTime.UtcNow;

    db.Scores.Add(score);

    await db.SaveChangesAsync();

    await publisher.PublishAsync("score-queue", score);

    return Results.Ok(score);
});


// ================= UPDATE =================

scores.MapPut("/{id}", async (
    int id,
    Score updatedScore,
    AppDbContext db) =>
{
    var score = await db.Scores.FindAsync(id);

    if (score is null)
        return Results.NotFound();

    score.PlayerId = updatedScore.PlayerId;
    score.GameId = updatedScore.GameId;
    score.Points = updatedScore.Points;

    await db.SaveChangesAsync();

    return Results.Ok(score);
});


// ================= DELETE =================

scores.MapDelete("/{id}", async (
    int id,
    AppDbContext db) =>
{
    var score = await db.Scores.FindAsync(id);

    if (score is null)
        return Results.NotFound();

    db.Scores.Remove(score);

    await db.SaveChangesAsync();

    return Results.Ok("Deleted");
});

app.Run();


// ================= PUBLISHER =================

class MessagePublisher : IMessagePublisher
{
    private readonly IModel _channel;

    public MessagePublisher(IModel channel)
    {
        _channel = channel;
    }

    public Task PublishAsync<T>(string queueName, T message)
    {
        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        var props = _channel.CreateBasicProperties();

        props.Persistent = true;

        _channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }
}


// ================= MODEL =================

public class Score
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public int Points { get; set; }

    public DateTime CreatedAt { get; set; }
}


// ================= DB CONTEXT =================

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Score> Scores => Set<Score>();
}
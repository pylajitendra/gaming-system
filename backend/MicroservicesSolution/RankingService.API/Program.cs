using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using RankingService.API.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlServer(
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


builder.Services.AddSingleton<IMessageConsumer, MessageConsumer>();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();


// ================= USE CORS =================

app.UseCors("AllowAngular");

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/rankings/{gameId}", (int gameId, AppDbContext db) =>
{
    var data = db.Rankings
        .Where(x => x.GameId == gameId)
        .OrderBy(x => x.Rank)
        .Take(10)
        .ToList();

    return Results.Ok(data);
});

app.Run();


// ================= CONSUMER =================

class MessageConsumer : IMessageConsumer
{
    private readonly IModel _channel;

    public MessageConsumer()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        var connection = factory.CreateConnection();

        _channel = connection.CreateModel();

        _channel.QueueDeclare("score-queue", true, false, false);
    }

    public void Consume<T>(string queue, Action<T> onReceive)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                var msg = JsonSerializer.Deserialize<T>(json);

                if (msg != null)
                    onReceive(msg);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch
            {
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue, false, consumer);
    }
}


// ================= WORKER =================

class Worker : BackgroundService
{
    private readonly IMessageConsumer _consumer;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory)
    {
        _consumer = consumer;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Consume<Score>("score-queue", score =>
        {
            using var scope = _scopeFactory.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var ranking = db.Rankings.FirstOrDefault(x =>
                x.PlayerId == score.PlayerId &&
                x.GameId == score.GameId);

            if (ranking == null)
            {
                db.Rankings.Add(new Ranking
                {
                    PlayerId = score.PlayerId,
                    GameId = score.GameId,
                    Points = score.Points
                });
            }
            else
            {
                ranking.Points = Math.Max(
                    ranking.Points,
                    score.Points);
            }

            db.SaveChanges();

            // ================= RANK CALCULATION =================

            var rankings = db.Rankings
                .Where(x => x.GameId == score.GameId)
                .OrderByDescending(x => x.Points)
                .ToList();

            for (int i = 0; i < rankings.Count; i++)
            {
                rankings[i].Rank = i + 1;
            }

            db.SaveChanges();
        });

        return Task.CompletedTask;
    }
}


// ================= MODELS =================

public class Score
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public int Points { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class Ranking
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public int Points { get; set; }

    public int Rank { get; set; }
}


// ================= DB =================

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ranking> Rankings => Set<Ranking>();

    public DbSet<Score> Scores => Set<Score>();
}
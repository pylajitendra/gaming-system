using GameService.API.Data;
using GameService.API.interfaces;
using GameService.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GameService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IInternalApiClient _apiClient;

    public GameController(
        AppDbContext context,
        IInternalApiClient apiClient)
    {
        _context = context;
        _apiClient = apiClient;
    }

    // ================= CREATE GAME =================

    [HttpPost]
    public async Task<IActionResult> Create(Game game)
    {
        game.CreatedAt = DateTime.UtcNow;

        _context.Games.Add(game);

        await _context.SaveChangesAsync();

        return Ok(game);
    }

    // ================= GET ALL GAMES =================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var games = await _context.Games.ToListAsync();

        return Ok(games);
    }

    // ================= GET GAME BY ID =================

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        // Get game from database
        var game = await _context.Games.FindAsync(id);

        if (game == null)
            return NotFound("Game not found");

        object? player = null;

        // Fetch player dynamically using PlayerId
        if (game.PlayerId.HasValue)
        {
            player = await _apiClient.GetAsync<object>(
                $"https://localhost:64124/players/{game.PlayerId}"
            );
        }

        return Ok(new
        {
            game,
            player = player ?? "Player not assigned"
        });
    }

    // ================= INTERNAL API CLIENT =================

    public class InternalApiClient : IInternalApiClient
    {
        private readonly HttpClient _http;

        public InternalApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return default;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
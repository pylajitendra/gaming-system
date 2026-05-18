using Microsoft.AspNetCore.Mvc;
using PlayerService.API.Data;
using PlayerService.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlayerService.API.Controllers;

[ApiController]
[Route("players")]
public class PlayerController : ControllerBase
{
    private readonly AppDbContext _context;

    public PlayerController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Player player)
    {
        player.CreatedAt = DateTime.UtcNow;

        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return Ok(player);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Players.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var player = _context.Players.Find(id);
        if (player == null) return NotFound();
        return Ok(player);
    }
}
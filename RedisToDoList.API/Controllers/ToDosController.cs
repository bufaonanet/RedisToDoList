using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisToDoList.API.Core.Entities;
using RedisToDoList.API.Infrastructure.Caching;
using RedisToDoList.API.Infrastructure.Persistence;
using RedisToDoList.API.Models;

namespace RedisToDoList.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ToDosController : ControllerBase
{
    private readonly ToDoListDbContext _context;
    private readonly ICachingService _cache;

    public ToDosController(ToDoListDbContext context, ICachingService cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var todoCache = await _cache.GetAsync(id.ToString());
        ToDo? todo;

        if (!string.IsNullOrWhiteSpace(todoCache))
        {
            todo = JsonSerializer.Deserialize<ToDo>(todoCache);

            Console.WriteLine("Loaded from cache.");

            return Ok(todo);
        }

        todo = await _context.ToDos.SingleOrDefaultAsync(t => t.Id == id);

        if (todo == null)
            return NotFound();

        await _cache.SetAsync(id.ToString(), JsonSerializer.Serialize(todo));

        return Ok(todo);
    }

    [HttpPost]
    public async Task<IActionResult> Post(ToDoInputModel model)
    {
        var todo = new ToDo(0, model.Title, model.Description);

        await _context.ToDos.AddAsync(todo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, model);
    }
}
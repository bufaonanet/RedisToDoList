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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var todo = await _cache.GetAsync<ToDo>(id.ToString());
        if (todo is not null)
        {
            Console.WriteLine("Loaded from cache.");

            return Ok(todo);
        }

        todo = await _context.ToDos.SingleOrDefaultAsync(t => t.Id == id);

        if (todo == null)
            return NotFound();

        await _cache.SetAsync(id.ToString(), todo);

        return Ok(todo);
    }

    [HttpPost]
    public async Task<IActionResult> Post(ToDoInputModel model)
    {
        var todo = new ToDo(0, model.Title, model.Description);

        await _context.ToDos.AddAsync(todo);
        await _context.SaveChangesAsync();

        await _cache.SetAsync(todo.Id.ToString(), todo);

        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, ToDoInputModel model)
    {
        var todo = await _context.ToDos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }

        todo.UpdateTitle(model.Title)
            .UpdateDescription(model.Description);

        await _context.SaveChangesAsync();
        
        await _cache.RemoveAsync(id.ToString());

        return Ok(todo);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _context.ToDos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }

        _context.ToDos.Remove(todo);
        await _context.SaveChangesAsync();
        
        await _cache.RemoveAsync(id.ToString());

        return NoContent();
    }
}
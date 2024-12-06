
using Database;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly MyDbContext _context;
        // private readonly ILogger _logger;

        public UserController(MyDbContext context, ILogger<UserController> logger) : base(logger)
        {
            _context = context;
            // _logger = logger;

        }

        // public IActionResult UserIndex()  index
        // {
        //     return View(); UserIndex
        // }
    // Create
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
    }
     [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers() // added comment additionally
    {
        var users = await _context.User.ToListAsync();
       return users;
    }

    // Read
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.User.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // Update
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(string id, User user)
    {
        if (id != user.UserId)
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    }
}


















































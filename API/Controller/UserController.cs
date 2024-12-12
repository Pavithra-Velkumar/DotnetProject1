
using System.Net;
using Database;
using Domain;
using Microsoft.AspNetCore.Authorization;
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
        // private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        // public UserController(MyDbContext context, ILogger<UserController> logger) : base(logger)
        // {
        //     _context = context;
        //     // _logger = logger;

        // }



        public UserController(IUserService userService, MyDbContext context, ILogger<UserController> logger) : base(logger)
        {
            _userService = userService;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(user);
                _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.UserId);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers() // added comment additionally
        {
            var users = await _context.User.ToListAsync();
            return users;
        }

        // Example GetUser method
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userService.GetUserById(id);
            // Implementation for getting a user by id
            return Ok(user); // Placeholder
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(new { message = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
            }

        }





        // Create
        // [HttpPost]
        // public async Task<ActionResult<User>> PostUser(User user)
        // {
        //     _context.User.Add(user);
        //     await _context.SaveChangesAsync();
        //     return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        // }



        //  [HttpGet("items")]
        //     public async Task<ActionResult<List<Items>>> GetItems() // added comment additionally
        //     {
        //         var items = await _context.Items.ToListAsync();
        //        return items;
        //     }
        // Read




        // [HttpGet("{id}")]
        // public async Task<ActionResult<User>> GetUser(string id)
        // {
        //     try
        //     {
        //         var user = await _context.User.FindAsync(id);

        //         if (user == null)
        //         {
        //             return NotFound(new { message = "User not found" });
        //         }

        //         return user!;
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred", details = ex.Message });
        //     }
        // }






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

        // // Delete
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUser(string id)
        // {
        //     var user = await _context.User.FindAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.User.Remove(user);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

    }
}


















































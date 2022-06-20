using MongoToken.Model;
using MongoToken.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;
using FoodWebsite.Models;

namespace MongoToken.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService) =>
        _userService = userService;

    [HttpGet]
    public async Task<List<User>> Get() =>
        await _userService.GetAsync();

    [AllowAnonymous]
    [Route("authenticate")]
    [HttpPost]
    public ActionResult Login([FromBody] User user)
    {
        var token = _userService.Authenticate(user.Email, user.Password);

        if (token == null)  
            return Unauthorized();

        return Ok(new { token, user });
    }
    //public async Task<IActionResult> Post(User newUser)
    //{
    //    newUser.Id = ObjectId.GenerateNewId().ToString();
    //    await _userService.CreateAsync(newUser);

    //    return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
    //}

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post(FWModel newOrder)
    {
        await _userService.CreateAsync(newOrder);

        return CreatedAtAction(nameof(Get), new { id = newOrder.Id }, newOrder);
    }

    [HttpGet("{email}")]
    public async Task<ActionResult<FWModel>> Get(string email)
    {
        var order = await _userService.GetAsync(email);

        if (order is null)
        {
            return NotFound();
        }

        return order;
    }
}
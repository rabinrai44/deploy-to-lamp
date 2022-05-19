using API.Entities;
using API.Helpers;
using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Isopoh.Cryptography.Argon2;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{

    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(ILogger<UsersController> logger, IUserService userService, IMapper mapper)
    {
        _mapper = mapper;
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var users = await _userService.GetAll();

        return Ok(users);
    }

    [HttpPost]
    public IActionResult Create(CreateRequest model)
    {
        // validate user email if alredy exist
        if (_userService.CheckEmailExists(model.Email))
        {
            return BadRequest(new AppException("User with the email '" + model.Email + "' already exists"));
        }

        // // map model to new user object
        var user = _mapper.Map<User>(model);

        // hash password
        user.PasswordHash = Argon2.Hash(model.Password);
        _userService.Create(user);
        return Ok(new { message = "User created" });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateRequest model)
    {
        var user = _userService.GetById(id);

        // validate
        if (model.Email != user.Email && _userService.CheckEmailExists(user.Email))
        {
            return BadRequest(new AppException("User with the email '" + model.Email + "' already exists"));
        }


        // validate
        if (user.Email != user.Email && _userService.CheckEmailExists(user.Email))
        {
            return BadRequest(new AppException("User with the email '" + user.Email + "' already exists"));
        }

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
        {
            user.PasswordHash = Argon2.Hash(model.Password);
        }
        // copy model to user and save
        _mapper.Map(model, user);
        _userService.UpdateAsync(user);
        return Ok(new { message = "User updated" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = _userService.GetById(id);
        if (user == null)
        {
            return NotFound(new ApplicationException("User was not found with id: " + id));
        }

        _userService.DeleteAsync(user);
        return Ok(new { message = "User deleted" });
    }
}

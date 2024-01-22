using System;
using System.Collections.Generic;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;

        public UserController(IConfiguration configuration, IRepository<User> userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<User> users = _userRepository.GetAll();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] User newUser)
        {
            try
            {
                Console.WriteLine("Received registration request: " + newUser.ToString());

                
                bool added = _userRepository.Add(newUser);
                if (!added)
                {
                    Console.WriteLine("Failed to add user");
                    return BadRequest("Failed to add user");
                }

                Console.WriteLine("User added successfully");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during user registration: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{userId}")]
        public IActionResult Put(User updatedUser)
        {
            bool updated = _userRepository.Update(updatedUser);
            if (updated)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{userId}")]
        public IActionResult Delete(int userId)
        {
            bool deleted = _userRepository.Delete(userId);
            if (deleted)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginRequest loginRequest)
        {
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Email == loginRequest.Email);

            if (user == null || !user.CheckPassword(loginRequest.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var jwtService = new JwtService(_configuration);
            var token = jwtService.GenerateJwtToken(user);

            return Ok(new { Token = token });
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] User newUser)
        {
            try
            {
                Console.WriteLine("Received registration request: " + newUser.ToString());

                if (_userRepository.GetAll().Any(u => u.Email == newUser.Email))
                {
                    Console.WriteLine("User with the same email already exists");
                    return BadRequest("User with the same email already exists");
                }

                bool added = _userRepository.Add(newUser);
                if (!added)
                {
                    Console.WriteLine("Failed to add user");
                    return BadRequest("Failed to add user");
                }

                Console.WriteLine("User added successfully");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during user registration: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }



    }
}

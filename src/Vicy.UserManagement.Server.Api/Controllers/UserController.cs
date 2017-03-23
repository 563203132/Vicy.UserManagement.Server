using Mehdime.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Vicy.UserManagement.Server.Api.Models;
using Vicy.UserManagement.Server.Domain;
using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.Api.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IReadDbFacade _readDb;
        private readonly IUserService _userService;
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public UserController(
            IReadDbFacade readDb,
            IUserService userService,
            IDbContextScopeFactory dbContextScopeFactory)
        {
            _readDb = readDb;
            _userService = userService;
            _dbContextScopeFactory = dbContextScopeFactory;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _readDb.Users.FirstOrDefault(u => u.Id == id);

            return Ok(user);
        }

        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(UserModel), 200)]
        [ProducesResponseType(typeof(UserModel), 404)]
        public IActionResult Post([FromBody]UserModel userModel)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var user = _userService.Create(
                    userModel.FirstName,
                    userModel.LastName,
                    userModel.Email,
                    userModel.PhoneNumber);

                dbContextScope.SaveChanges();

                return Ok(user.Id);
            }
        }

        [Route("edit")]
        [HttpPut]
        [ProducesResponseType(typeof(UserModel), 200)]
        [ProducesResponseType(typeof(UserModel), 404)]
        public IActionResult Edit([FromBody]UserModel userModel)
        {
            if (userModel.Id == 0)
                throw new ArgumentException($"user id is {userModel.Id}");

            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var user = _userService.Edit(
                    userModel.Id,
                    userModel.FirstName,
                    userModel.LastName,
                    userModel.Email,
                    userModel.PhoneNumber);

                if (user == null)
                    return NotFound();

                dbContextScope.SaveChanges();

                return Ok(user.Id);
            }
        }
    }
}

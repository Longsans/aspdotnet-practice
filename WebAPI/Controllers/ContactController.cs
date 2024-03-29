﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Common.Models;
using Common.Services;
using Common.Validators;
using Practice.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<Contact> _contactValidator;

        public ContactController(
            IUserService userService, 
            IValidator<Contact> contactValidator)
        {
            _userService = userService;
            _contactValidator = contactValidator;
        }

        [HttpGet("{username}")]
        public ActionResult<Contact?> GetContact(string username)
        {
            return Ok(_userService.FindContactByUsername(username));
        }

        [HttpPost]
        public async Task<ActionResult<Contact?>> CreateContact(Contact contact)
        {
            var validationResult = await _contactValidator.ValidateAsync(contact);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToErrorDictionary());
            }

            var existing = _userService.FindContactByUsername(contact.UserUsername);
            if (existing != null)
            {
                return Conflict(
                    new Dictionary<string, string>
                    {
                        { "username", $"Contact with username {contact.UserUsername} already exists" }
                    });
            }

            await _userService.AddContact(contact);
            return CreatedAtAction(
                nameof(GetContact), 
                new { username = contact.UserUsername }, 
                contact);
        }

        [HttpPut("{username}")]
        public async Task<IActionResult> UpdateContact(string username, Contact update)
        {
            var validationResult = await _contactValidator.ValidateAsync(update);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToErrorDictionary());
            }
            if (username != update.UserUsername)
            {
                return BadRequest(
                    new Dictionary<string, string>
                    {
                        { "username", "Route value username must be the same as contact.UserUsername" },
                    });
            }

            var existing = _userService.FindContactByUsername(update.UserUsername);
            if (existing == null)
            {
                return NotFound(update);
            }

            await _userService.UpdateContact(update);
            return NoContent();
        }
    }
}

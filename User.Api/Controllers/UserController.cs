using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using User.Application.Ports;
using User.Application.DTOs;
using User.Domain.Entities;
using User.Application.Services;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service) => _service = service;

        
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserCreateDto dto, [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            try
            {
                var actorId = ParseActorId(actorHeader);
                var created = await _service.RegisterAsync(dto, actorId);
                return CreatedAtAction(nameof(GetById), new { id = created.id }, ToCompleteView(created));
            }
            catch (ValidationException ve) { return BadRequest(new { message = ve.Message, errors = ve.Errors }); }
            catch (DomainException de) { return BadRequest(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        // GET api/user/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var u = await _service.GetByIdAsync(id);
                if (u is null) return NotFound();
                return Ok(ToCompleteView(u));
            }
            catch (Exception) { return StatusCode(500); }
        }

        // GET api/user
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var list = await _service.ListAsync();
                var view = list.Select(ToView);
                return Ok(view);
            }
            catch (Exception) { return StatusCode(500); }
        }

        // PUT api/user/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto, [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            try
            {
                var actorId = ParseActorId(actorHeader);
                await _service.UpdateAsync(id, dto, actorId);
                return NoContent();
            }
            catch (ValidationException ve) { return BadRequest(new { message = ve.Message, errors = ve.Errors }); }
            catch (NotFoundException nf) { return NotFound(new { message = nf.Message }); }
            catch (DomainException de) { return BadRequest(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        // DELETE api/user/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id, [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            try
            {
                var actorId = ParseActorId(actorHeader);
                await _service.SoftDeleteAsync(id, actorId);
                return NoContent();
            }
            catch (NotFoundException nf) { return NotFound(new { message = nf.Message }); }
            catch (DomainException de) { return BadRequest(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        // POST api/user/{id}/change-password
        [HttpPost("{id:int}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] UserChangePasswordDto dto)
        {
            try
            {
                await _service.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);
                return NoContent();
            }
            catch (ValidationException ve) { return BadRequest(new { message = ve.Message, errors = ve.Errors }); }
            catch (NotFoundException nf) { return NotFound(new { message = nf.Message }); }
            catch (DomainException de) { return BadRequest(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        // POST api/user/authenticate
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest req)
        {
            try
            {
                var user = await _service.AuthenticateAsync(req.Username, req.Password);
                return Ok(ToView(user));
            }
            catch (DomainException de) { return Unauthorized(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        // Helpers / mappers
        private static int ParseActorId(string? header)
        {
            if (!string.IsNullOrWhiteSpace(header) && int.TryParse(header, out var id)) return id;
            return 0; // fallback actor id
        }

        private static UserViewDto ToView(UserEntity u)
            => new(u.id, u.username, u.last_first_name, u.last_second_name, u.mail, u.phone, u.ci, u.role);

        private static UserCompleteViewDto ToCompleteView(UserEntity u)
            => new(u.id, u.username, u.first_name, u.last_first_name, u.last_second_name, u.mail, u.phone, u.ci, u.role, u.has_changed_password, u.password_version, u.last_password_changed_at);

        // Small request model for authentication body
        public record AuthenticateRequest(string Username, string Password);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using User.Domain.Entities;
using User.Domain.Enums;
using User.Domain.Ports;
using User.Domain.Exceptions;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserFacade _facade;
        public UserController(IUserFacade facade) => _facade = facade;

        // Request / Response models locales al proyecto API (no dependen de User.Application.DTOs)
        public record CreateUserRequest(
            string FirstName,
            string LastFirstName,
            string? LastSecondName,
            string Mail,
            string Phone,
            string Ci,
            int Role
        );

        public record UpdateUserRequest(
            string? FirstName,
            string? LastFirstName,
            string? LastSecondName,
            string? Mail,
            string? Phone,
            string? Ci,
            int? Role
        );

        public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

        public record AuthenticateRequest(string Username, string Password);

        public record UserListItemResponse(int Id, string Username, string LastFirstName, string? LastSecondName, string? Mail, string Phone, string Ci, UserRole Role);

        public record UserCompleteResponse(int Id, string Username, string FirstName, string LastFirstName, string? LastSecondName, string? Mail, string Phone, string Ci, UserRole Role, bool HasChangedPassword, int PasswordVersion, DateTime? LastPasswordChangedAt);

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest req, [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!Enum.IsDefined(typeof(UserRole), req.Role)) return BadRequest(new { message = "Role inválido." });

            try
            {
                var actorId = ParseActorId(actorHeader);
                var entity = new UserEntity
                {
                    first_name = req.FirstName?.Trim() ?? "",
                    last_first_name = req.LastFirstName?.Trim() ?? "",
                    last_second_name = string.IsNullOrWhiteSpace(req.LastSecondName) ? null : req.LastSecondName!.Trim(),
                    mail = req.Mail?.Trim() ?? "",
                    phone = req.Phone?.Trim() ?? "",
                    ci = req.Ci?.Trim() ?? "",
                    role = (UserRole)req.Role
                };

                var created = await _facade.RegisterAsync(entity, actorId);
                return CreatedAtAction(nameof(GetById), new { id = created.id }, ToCompleteResponse(created));
            }
            catch (ValidationException ve) { return BadRequest(new { message = ve.Message, errors = ve.Errors }); }
            catch (DomainException de) { return BadRequest(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var u = await _facade.GetByIdAsync(id);
                if (u is null) return NotFound();
                return Ok(ToCompleteResponse(u));
            }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var list = await _facade.ListAsync();
                var view = list.Select(ToListItemResponse);
                return Ok(view);
            }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest req, [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var actorId = ParseActorId(actorHeader);
                var current = await _facade.GetByIdAsync(id) ?? throw new NotFoundException("Usuario no encontrado.");

                if (req.FirstName is not null) current.first_name = req.FirstName.Trim();
                if (req.LastFirstName is not null) current.last_first_name = req.LastFirstName.Trim();
                if (req.LastSecondName is not null) current.last_second_name = req.LastSecondName.Trim();
                if (req.Mail is not null) current.mail = req.Mail.Trim();
                if (req.Phone is not null) current.phone = req.Phone.Trim();
                if (req.Ci is not null) current.ci = req.Ci.Trim();
                if (req.Role is not null)
                {
                    if (!Enum.IsDefined(typeof(UserRole), req.Role.Value)) return BadRequest(new { message = "Role inválido." });
                    current.role = (UserRole)req.Role.Value;
                }

                await _facade.UpdateAsync(current, actorId);
                return NoContent();
            }
            catch (ValidationException ve) { return BadRequest(new { message = ve.Message, errors = ve.Errors }); }
            catch (NotFoundException nf) { return NotFound(new { message = nf.Message }); }
            catch (DomainException de) { return BadRequest(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> SoftDelete(int id, [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            try
            {
                var actorId = ParseActorId(actorHeader);
                await _facade.SoftDeleteAsync(id, actorId);
                return NoContent();
            }
            catch (NotFoundException nf) { return NotFound(new { message = nf.Message }); }
            catch (DomainException de) { return BadRequest(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("{id:int}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _facade.ChangePasswordAsync(id, req.CurrentPassword, req.NewPassword);
                return NoContent();
            }
            catch (ValidationException ve) { return BadRequest(new { message = ve.Message, errors = ve.Errors }); }
            catch (NotFoundException nf) { return NotFound(new { message = nf.Message }); }
            catch (DomainException de) { return BadRequest(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _facade.AuthenticateAsync(req.Username, req.Password);
                return Ok(ToListItemResponse(user));
            }
            catch (DomainException de) { return Unauthorized(new { message = de.Message }); }
            catch (Exception) { return StatusCode(500); }
        }

        // Helpers / mappers
        private static int ParseActorId(string? header)
        {
            if (!string.IsNullOrWhiteSpace(header) && int.TryParse(header, out var id)) return id;
            return 0;
        }

        private static UserListItemResponse ToListItemResponse(UserEntity u)
            => new(u.id, u.username, u.last_first_name, u.last_second_name, u.mail, u.phone, u.ci, u.role);

        private static UserCompleteResponse ToCompleteResponse(UserEntity u)
            => new(u.id, u.username, u.first_name, u.last_first_name, u.last_second_name,
                   u.mail, u.phone, u.ci, u.role, u.has_changed_password, u.password_version,
                   u.last_password_changed_at);
    }
}

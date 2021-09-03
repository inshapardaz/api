using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Domain.Models;
using System.Threading;
using Inshapardaz.Domain.Ports.Handlers.Account;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Views.Accounts;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : BaseController
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IMapper _mapper;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderAccount _accountRenderer;

        public AccountsController(
            IAmACommandProcessor commandProcessor,
            IQueryProcessor queryProcessor,
            IMapper mapper,
            IRenderAccount accountRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _accountRenderer = accountRenderer;
            _mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticateResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(void))]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model, CancellationToken cancellationToken)
        {
            var command = new AuthenticateCommand(model.Email, model.Password);
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            setTokenCookie(command.Response.RefreshToken);

            return Ok(_accountRenderer.Render(command.Response));
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticateResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("refresh-token")]
        //TODO : Can be POST /token
        public async Task<ActionResult<AuthenticateResponse>> RefreshToken(RefreshTokenRequest model, CancellationToken cancellationToken)
        {
            var refreshToken = model.RefreshToken ?? Request.Cookies["refreshToken"];
            var command = new RefreshTokenCommand(refreshToken);
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            setTokenCookie(command.Response.RefreshToken);
            return Ok(_accountRenderer.Render(command.Response));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Role.Admin)]
        [HttpPost("revoke-token")]
        //TODO : Can be DELETE /token
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest model, CancellationToken cancellationToken)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];
            var command = new RevokeTokenCommand(model.Token) { Revoker = Account };
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [HttpGet("invitation/{id}", Name = nameof(CheckInvitationCode))]
        public async Task<IActionResult> CheckInvitationCode(string id, CancellationToken cancellationToken)
        {
            var validityStatus = await _queryProcessor.ExecuteAsync(new GetInvitationStatusQuery(id), cancellationToken: cancellationToken);

            if (validityStatus == InvitationStatuses.NotFound)
            {
                return NotFound();
            }

            if (validityStatus == InvitationStatuses.Expired)
            {
                return StatusCode(410);
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("invitations", Name = nameof(ResendInvitationCode))]
        public async Task<IActionResult> ResendInvitationCode([FromBody] ResendInvitationCodeRequest request, CancellationToken cancellationToken)
        {
            await _commandProcessor.SendAsync(new ResendInvitationCodeCommand(request.Email), cancellationToken: cancellationToken);

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Authorize(Role.Admin, Role.LibraryAdmin)]
        [HttpPost("invite")]
        [HttpPost("invite/library/{libraryId}")]
        public async Task<IActionResult> InviteUser(int libraryId, [FromBody] InviteUserRequest model, CancellationToken cancellationToken)
        {
            var command = new InviteUserCommand()
            {
                Email = model.Email,
                Name = model.Name,
                LibraryId = libraryId,
                Role = model.Role
            };
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("register/{invitationCode}")]
        public async Task<IActionResult> Register(string invitationCode, [FromBody] RegisterRequest model, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand()
            {
                Name = model.Name,
                Password = model.Password,
                AcceptTerms = model.AcceptTerms,
                InvitationCode = invitationCode
            };

            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model, CancellationToken cancellationToken)
        {
            var command = new PasswordResetCommand(model.Email);

            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model, CancellationToken cancellationToken)
        {
            var command = new ResetPasswordCommand()
            {
                Token = model.Token,
                Password = model.Password
            };

            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest model, CancellationToken cancellationToken)
        {
            var command = new ChangePasswordCommand()
            {
                Email = Account.Email,
                Password = model.Password,
                OldPassword = model.OldPassword
            };

            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok();
        }

        [Authorize(Role.Admin)]
        [HttpGet(Name = nameof(AccountsController.GetAll))]
        public async Task<IActionResult> GetAll(string query, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
        {
            var seriesQuery = new GetAccountsQuery(pageNumber, pageSize) { Query = query };
            var series = await _queryProcessor.ExecuteAsync(seriesQuery, cancellationToken: token);

            var args = new PageRendererArgs<AccountModel>
            {
                Page = series,
                RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
            };

            return new OkObjectResult(_accountRenderer.Render(args));
        }

        [Authorize(Role.Admin, Role.LibraryAdmin)]
        [HttpGet("/libraries/{libraryId}/writers", Name = nameof(AccountsController.GetWriters))]
        public async Task<IActionResult> GetWriters(int libraryId, string query, CancellationToken token = default(CancellationToken))
        {
            var writersQuery = new GetWritersQuery(libraryId, query);
            var writers = await _queryProcessor.ExecuteAsync(writersQuery, cancellationToken: token);

            return new OkObjectResult(_accountRenderer.RenderLookup(writers));
        }

        [Authorize]
        [HttpGet("{id:int}", Name = nameof(AccountsController.GetById))]
        public async Task<ActionResult<AccountView>> GetById(int id, CancellationToken cancellationToken)
        {
            // users can get their own account and admins can get any account
            if (id != Account.Id && Account.IsSuperAdmin)
                return Unauthorized(new { message = "Unauthorized" });

            var account = await _queryProcessor.ExecuteAsync(new GetAccountByIdQuery(id), cancellationToken);
            return Ok(account);
        }

        [Authorize(Role.Admin)]
        [HttpPost(Name = nameof(AccountsController.Create))]
        public ActionResult<AccountView> Create(CreateRequest model)
        {
            //TODO : Implement
            //var account = _accountService.Create(model);
            //return Ok(account);
            return NotFound();
        }

        [Authorize]
        [HttpPut("{id:int}", Name = nameof(AccountsController.Update))]
        public ActionResult<AccountView> Update(int id, UpdateRequest model)
        {
            // users can update their own account and admins can update any account
            //if (id != Account.Id && Account.Role != Role.Admin)
            //   return Unauthorized(new { message = "Unauthorized" });

            // only admins can update role
            //if (Account.Role != Role.Admin)
            //    model.Role = null;

            //var account = _accountService.Update(id, model);
            //return Ok(account);
            return NotFound();
        }

        [Authorize]
        [HttpDelete("{id:int}", Name = nameof(AccountsController.Delete))]
        public IActionResult Delete(int id)
        {
            // users can delete their own account and admins can delete any account
            //if (id != Account.Id && Account.Role != Role.Admin)
            //    return Unauthorized(new { message = "Unauthorized" });

            //_accountService.Delete(id);
            //return Ok(new { message = "Account deleted successfully" });
            return NotFound();
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
#if DEBUG
                SameSite = SameSiteMode.Lax
#else
                SameSite = SameSiteMode.None,
                Secure = true
#endif
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}

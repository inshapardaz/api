using Microsoft.AspNetCore.Mvc;
using Inshapardaz.Domain.Models;
using Paramore.Darker;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Views.Accounts;
using Paramore.Brighter;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Ports.Command.Account;
using Inshapardaz.Domain.Ports.Query.Account;
using Microsoft.Extensions.Options;

namespace Inshapardaz.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IRenderAccount _accountRenderer;

    public AccountsController(
        IAmACommandProcessor commandProcessor,
        IQueryProcessor queryProcessor,
        IRenderAccount accountRenderer)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
        _accountRenderer = accountRenderer;
    }


    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticateResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(void))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(void))]
    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model, CancellationToken cancellationToken)
    {
        var command = new AuthenticateCommand(model.Email, model.Password);
        await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);

        return Ok(_accountRenderer.Render(command.Response));
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticateResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthenticateResponse>> RefreshToken([FromBody] RefreshTokenRequest model, CancellationToken cancellationToken)
    {
        var refreshToken = model.RefreshToken ?? Request.Cookies["refreshToken"];
        var command = new RefreshTokenCommand(refreshToken);
        await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
        return Ok(_accountRenderer.Render(command.Response));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model, CancellationToken cancellationToken)
    {
        var token = model.Token ?? Request.Cookies["refreshToken"];
        var command = new RevokeTokenCommand(token);
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
    [HttpPost("invite")]
    [HttpPost("invite/library/{libraryId}", Name = nameof(AccountsController.InviteUser))]
    public async Task<IActionResult> InviteUser(int libraryId, [FromBody] InviteUserRequest model, CancellationToken cancellationToken)
    {
        var command = new InviteUserCommand(libraryId)
        {
            Email = model.Email,
            Name = model.Name,
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
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest model, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand()
        {
            Password = model.Password,
            OldPassword = model.OldPassword
        };

        await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
        return Ok();
    }

    [HttpGet(Name = nameof(GetAll))]
    public async Task<IActionResult> GetAll(string query, int pageNumber = 1, int pageSize = 10, CancellationToken token = default(CancellationToken))
    {
        var accountsQuery = new GetAccountsQuery(pageNumber, pageSize) { Query = query };
        var accounts = await _queryProcessor.ExecuteAsync(accountsQuery, cancellationToken: token);

        var args = new PageRendererArgs<AccountModel>
        {
            Page = accounts,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
        };

        return new OkObjectResult(_accountRenderer.Render(args));
    }

    [HttpGet("libraries/{libraryId}/users", Name = nameof(AccountsController.GetLibraryUsers))]
    [Produces(typeof(PageView<AccountView>))]
    public async Task<IActionResult> GetLibraryUsers(int libraryId, string query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken))
    {
        var accountsQuery = new GetAccountsByLibraryQuery(libraryId, pageNumber, pageSize) { Query = query };
        var accounts = await _queryProcessor.ExecuteAsync(accountsQuery, cancellationToken: cancellationToken);

        var args = new PageRendererArgs<AccountModel>
        {
            Page = accounts,
            RouteArguments = new PagedRouteArgs { PageNumber = pageNumber, PageSize = pageSize, Query = query },
        };

        return new OkObjectResult(_accountRenderer.Render(args, libraryId));
    }

    [HttpGet("/libraries/{libraryId}/writers", Name = nameof(AccountsController.GetWriters))]
    public async Task<IActionResult> GetWriters(int libraryId, string query, CancellationToken token = default(CancellationToken))
    {
        var writersQuery = new GetWritersQuery(libraryId, query);
        var writers = await _queryProcessor.ExecuteAsync(writersQuery, cancellationToken: token);

        return new OkObjectResult(_accountRenderer.RenderLookup(writers));
    }

    [HttpGet("{id:int}", Name = nameof(AccountsController.GetById))]
    public async Task<ActionResult<AccountView>> GetById(int id, CancellationToken cancellationToken)
    {
        var account = await _queryProcessor.ExecuteAsync(new GetAccountByIdQuery(id), cancellationToken);
        return Ok(_accountRenderer.Render(account));
    }

    [HttpGet("/libraries/{libraryId}/users/{id:int}", Name = nameof(AccountsController.GetLibraryUserById))]
    public async Task<ActionResult<AccountView>> GetLibraryUserById(int libraryId, int id, CancellationToken cancellationToken)
    {
        var query = new GetAccountByIdQuery(id) { LibraryId = libraryId };
        var account = await _queryProcessor.ExecuteAsync(query, cancellationToken);
        return Ok(_accountRenderer.Render(account, libraryId));
    }

    [HttpPut("{id:int}", Name = nameof(AccountsController.Update))]
    public async Task<ActionResult> Update(int id, UpdateRequest model, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(id)
        {
            Email = model.Email,
            LibraryId = null,
            Name = model.Name,
            Role = model.Role,
        };
        await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
        return Ok(model);
    }

    [HttpPut("/libraries/{libraryId}/users/{id:int}", Name = nameof(AccountsController.UpdateLibraryUser))]
    public async Task<ActionResult> UpdateLibraryUser(int libraryId, int id, UpdateRequest model, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(id)
        {
            Email = model.Email,
            LibraryId = libraryId,
            Name = model.Name,
            Role = model.Role,
        };

        await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
        return Ok(model);
    }

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
}

using AutoMapper;
using FluentValidation;
using Holocron.App.Api.Data;
using Holocron.App.Api.Data.Entities;
using Holocron.App.Api.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Holocron.App.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // keep base clean; explicit routes per action
public class CommentsController(DataContext dataContext, IMapper mapper, IValidator<NewCommentRequest> validator) : ControllerBase
{
    [HttpGet("{name}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetComments(string name)
    {
        var comments = await dataContext.Comments
            .Where(c => c.Name == name)
            .ToListAsync();

        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment(NewCommentRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var commentEntity = mapper.Map<CommentEntity>(request);
        dataContext.Comments.Add(commentEntity);
        await dataContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetComments), new { name = commentEntity.Name }, commentEntity);
    }

    [HttpGet("{name}/count")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCommentCount(string name)
    {
        var count = await dataContext.Comments.CountAsync(c => c.Name == name);
        return Ok(new { name, count });
    }

    [HttpGet("{name}/me")]
    public async Task<IActionResult> HasUserCommented(string name)
    {
        var tenantId = HttpContext.User?.Identity?.IsAuthenticated == true
            ? HttpContext.User.FindFirst("sub")?.Value
            : null;

        if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

        var hasCommented = await dataContext.Comments
            .AnyAsync(c => c.Name == name && c.TenantId == tenantId);

        return Ok(hasCommented);
    }
}


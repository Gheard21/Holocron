using System.Security.Claims;
using Holocron.App.Api.Data;
using Holocron.App.Api.Data.Entities;
using Holocron.App.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Holocron.App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LikesController(DataContext dataContext, ILogger<LikesController> logger, IIdentityProvider identityProvider) : ControllerBase
    {


        [HttpGet("{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLikesForPerson(string name)
        {
            try
            {
                var likes = await dataContext.Likes.Where(x => x.Name == name).CountAsync();
                return Ok(likes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured attempting to access the database.");
                return StatusCode(500);
            }
        }

        [HttpGet("{name}/me")]
        public async Task<IActionResult> HasUserLiked(string name)
        {
            try
            {
                var tenantId = identityProvider.GetCurrentUserId();
                var hasLiked = await dataContext.Likes.AnyAsync(x => x.Name == name && x.TenantId == tenantId);
                return Ok(hasLiked);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured attempting to access the database.");
                return StatusCode(500);
            }
        }

        [HttpPost("{name}")]
        public async Task<IActionResult> PostNewLike(string name)
        {
            try
            {
                var tenantId = identityProvider.GetCurrentUserId();
                var existingLike = await dataContext.Likes.FirstOrDefaultAsync(x => x.Name == name && x.TenantId == tenantId);

                if (existingLike != null)
                    return BadRequest("You have already liked this person.");

                await dataContext.Likes.AddAsync(new LikeEntity(name));
                await dataContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured attempting to access the database.");
                return StatusCode(500);
            }
        }

        [HttpDelete("{name}")]
        [Authorize]
        public async Task<IActionResult> DeleteLike(string name)
        {
            try
            {
                var tenantId = identityProvider.GetCurrentUserId();
                var existingLike = await dataContext.Likes.FirstOrDefaultAsync(x => x.Name == name && x.TenantId == tenantId);

                if (existingLike == null)
                    return NotFound("You have not liked this person.");

                dataContext.Likes.Remove(existingLike);
                await dataContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured attempting to access the database.");
                return StatusCode(500);
            }
        }
    }
}

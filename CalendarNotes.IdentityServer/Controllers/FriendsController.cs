using CalendarNotes.IdentityServer.Models;
using CalendarNotes.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarNotes.IdentityServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;

        public FriendsController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpPost]
        public async Task<ActionResult<Friendship>> SendRequest([FromQuery] string requesterId, [FromQuery] string addresseeEmail)
        {
            var result = await _friendshipService.SendRequestAsync(requesterId, addresseeEmail);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Accept([FromQuery] int friendshipId)
        {
            await _friendshipService.AcceptAsync(friendshipId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Reject([FromQuery] int friendshipId)
        {
            await _friendshipService.RejectAsync(friendshipId);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> GetFriends([FromQuery] string userId)
        {
            var result = await _friendshipService.GetFriendsAsync(userId);
            return Ok(result.Select(u => new { u.Id, u.Email, FullName = (u.FirstName + " " + u.LastName).Trim() }));
        }

        [HttpGet]
        public async Task<ActionResult<List<Friendship>>> GetPending([FromQuery] string userId)
        {
            var result = await _friendshipService.GetPendingAsync(userId);
            return Ok(result);
        }
    }
}


using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]

    public class LikesController : ControllerBase
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;

        public LikesController(ILikesRepository likesRepository, IUserRepository userRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            int sourceUserId = User.GetUserId();
            var targetUser = await _userRepository.GetUserByUserNameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if (targetUser == null) return NotFound();
            if (sourceUser.UserName.Equals(username)) return BadRequest("Cannot Like Yourself!");
            var userLike = await _likesRepository.GetUserLike(sourceUserId, targetUser.Id);
            if (userLike != null) return BadRequest("You Already like this user");

            UserLike likeUser = new()
            {
                SourceUserId = sourceUser.Id,
                TargetUserId = targetUser.Id
            };
            sourceUser.LikedUsers.Add(likeUser);
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to like user");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParam likesParam)
        {
            if (likesParam.Predicate.Equals("likedBy") || likesParam.Predicate.Equals("liked"))

            {
                likesParam.UserId = User.GetUserId();
                var users = await _likesRepository.GetUserLikes(likesParam);
                Response.AddPaginationHeader(new PaginationHeader
                (users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPages));
                return Ok(users);
            }
            return NotFound();
        }
    }
}
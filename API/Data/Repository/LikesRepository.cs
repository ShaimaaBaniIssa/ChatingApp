
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParam likesParam)
        {

            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable(); //nothing happend in DB
            var likes = _context.Likes.AsQueryable();
            if (likesParam.Predicate.Equals("liked"))
            {
                likes = likes.Where(u => u.SourceUserId == likesParam.UserId);
                users = likes.Select(t => t.TargetUser); // return the liked users
            }
            else if (likesParam.Predicate.Equals("likedBy"))
            {
                likes = likes.Where(u => u.TargetUserId == likesParam.UserId);
                users = likes.Select(t => t.SourceUser); // return the liked by users
            }
            var likedUsers = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain == true).Url,
                City = user.City,
                Id = user.Id,
                KnownAs = user.KnownAs
            });
            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParam.PageNumber, likesParam.PageSize);


        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
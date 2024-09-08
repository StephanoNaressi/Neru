using Microsoft.EntityFrameworkCore;
using Neru.Context;
using Neru.Models;

namespace Neru.Utils
{
    public class CommonDB
    {
        public async Task<UserRemembrance> FindByIdAsync(SqliteContext lite, string userId)
            => await lite.UserRemembrances.FirstOrDefaultAsync(f => f.UserId == userId);
        public async Task AddLove(float amount,  string userId)
        {
            using SqliteContext lite = new SqliteContext();
            var friend = await FindByIdAsync(lite, userId);
            friend.UserLove += amount;
            await lite.SaveChangesAsync();
        }
    }
}

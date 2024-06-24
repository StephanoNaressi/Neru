using Microsoft.EntityFrameworkCore;
using Neru.Context;
using Neru.Models;

namespace Neru.Utils
{
    public class CommonDB
    {
        public async Task<UserRemembrance> FindByIdAsync(SqliteContext lite, string userId)
            => await lite.UserRemembrances.FirstOrDefaultAsync(f => f.UserId == userId);

    }
}

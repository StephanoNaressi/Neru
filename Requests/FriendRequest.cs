using Discord.WebSocket;
using Discord;
using Neru.Context;
using Neru.Models;

public class FriendRequest(UserRemembrance friend, SocketSlashCommand cmd) : IPendingRequest
{
    public async Task<bool> TreatMessageAsync(string msg, IUser userCurrent, IChannel channelCurrent)
    {
        if (cmd.User == userCurrent && cmd.Channel == channelCurrent)
        {
            using SqliteContext lite = new SqliteContext();
            var newFriend = new UserRemembrance();
            newFriend.UserId = userCurrent.Id.ToString();
            newFriend.UserNickname = msg;
            newFriend.UserLove = 0f;
            lite.UserRemembrances.Add(newFriend);
            await lite.SaveChangesAsync();
            await cmd.FollowupAsync($"Awesome! Added you to my friends :bubbles:! {newFriend.UserNickname}");
            return true;
        }
        return false;
    }
}
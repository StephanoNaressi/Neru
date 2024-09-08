using Discord.WebSocket;
using Discord;
using Neru.Context;
using Neru.Models;
using System;
using Neru.Utils;

namespace Neru.Requests
{
    public class KanaRequest(UserRemembrance friend, SocketSlashCommand cmd, string valuesChosen, CommonDB commonDB) : IPendingRequest
    {
        
        public async Task<bool> TreatMessageAsync(string msg, IUser user, IChannel channel)
        {
            if (cmd.User == user && cmd.Channel == channel)
            {
                if (valuesChosen.ToUpperInvariant().Replace(" ", "") == msg.ToUpperInvariant().Replace(" ", ""))
                {
                    await cmd.FollowupAsync("Well done! :bubbles:");
                    await commonDB.AddLove(0.02f, user.Id.ToString());
                }
                else
                {
                    await cmd.FollowupAsync($"Oops! Wrong! :x: the correct answer was \n### {valuesChosen}");
                    await commonDB.AddLove(-0.01f, user.Id.ToString());
                }
                return true;
            }
            return false;
        }
    }
}

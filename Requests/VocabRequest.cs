using Discord;
using Discord.WebSocket;
using Neru.JsonClasses;
using Neru.Models;
using Neru.Utils;
using System.Text.RegularExpressions;

namespace Neru.Requests
{
    public class VocabRequest(UserRemembrance friend, SocketSlashCommand cmd, Vocab valuesChosen, CommonDB commonDB) : IPendingRequest
    {
        public async Task<bool> TreatMessageAsync(string msg, IUser user, IChannel channel)
        {
            if (cmd.User == user && cmd.Channel == channel)
            {
                if (valuesChosen.meaning.Any(x => Regex.Replace(x.ToUpperInvariant().Replace(" ", ""), "\\([^\\)]+\\)", string.Empty) == msg.ToUpperInvariant().Replace(" ", "")))
                {
                    await cmd.FollowupAsync("Wow Impressive! :bubbles:");
                    await commonDB.AddLove(0.02f, user.Id.ToString());
                }
                else
                {
                    await cmd.FollowupAsync($"Oops! Wrong! :x: the correct answer was \n## {valuesChosen.meaning[0]} || {valuesChosen.reading}");
                    await commonDB.AddLove(-0.01f, user.Id.ToString());
                }
                return true;
           }
           return false;
        }
    }
}

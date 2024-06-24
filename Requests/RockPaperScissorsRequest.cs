using Discord;
using Discord.WebSocket;
using Neru.Context;
using Neru.Models;

namespace Neru.Requests
{
    public class RockPaperScissorsRequest(SocketSlashCommand cmd, Random r, UserRemembrance friend) : IPendingRequest
    {
        public enum RpsType
        {
            Rock,
            Paper,
            Scissors
        }
        public async Task<bool> TreatMessageAsync(string msg, IUser user, IChannel channel)
        {

            if (cmd.User == user && cmd.Channel == channel)
            {
                var types = new[] { "ROCK", "PAPER", "SCISSORS" };
                RpsType myChoice = (RpsType)Array.IndexOf(types, msg.ToUpperInvariant());

                if (myChoice == (RpsType)(-1))
                {
                    await cmd.FollowupAsync("Oop! You had a typo, make sure to reply with Rock, Paper or Scissors :)");
                    return true;
                }

                RpsType neruChoice = (RpsType)r.Next(0, 2);
                bool neruWins = false;
                bool isTie = false;
                string neruWeapon = string.Empty;

                if (neruChoice == myChoice) isTie = true;
                else
                {
                    if(neruChoice == RpsType.Rock)
                    {
                        if (myChoice == RpsType.Paper) neruWins = false;
                        else if (myChoice == RpsType.Scissors) neruWins = true;
                    }
                    else if (neruChoice == RpsType.Paper)
                    {
                        if (myChoice == RpsType.Rock) neruWins = true;
                        else if (myChoice == RpsType.Scissors) neruWins = false;
                    }
                    else if (neruChoice == RpsType.Scissors)
                    {
                        if (myChoice == RpsType.Paper) neruWins = true;
                        else if (myChoice == RpsType.Rock) neruWins = false;
                    }
                }
                switch (neruChoice)
                {
                    case RpsType.Rock:
                        neruWeapon = ":fist:";
                        break;
                    case RpsType.Paper:
                        neruWeapon = ":newspaper2:";
                            break;
                    case RpsType.Scissors:
                        neruWeapon = ":scissors:";
                        break;
                }
                if (isTie) 
                {
                    await cmd.FollowupAsync("Aww Darnit! Haha it's a tie!");
                    return true;
                } 

                else
                {
                    if (neruWins)
                    {
                        await cmd.FollowupAsync($"Haha! I chose {neruWeapon} so I win! :bubbles:");
                        if(friend.UserLove - 0.2f  > 0)
                        {
                            friend.UserLove -= 0.2f;
                        }
                        return true;
                    }

                    else 
                    {
                        await cmd.FollowupAsync($"Oh no! I chose {neruWeapon} so {friend.UserNickname} wins! :bubbles:");
                        friend.UserLove += 0.10f;
                        return true;
                    } 

                }
            }
            return false;
        }
    }
}

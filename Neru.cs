using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Neru.Context;
using System.Diagnostics;
using Discord.Interactions;
using Neru.Models;
using Neru.Utils;
namespace Neru
{

    public sealed class Neru
    {
        DiscordSocketClient _client;
        List<IPendingRequest> _requests = new();
        ulong _testingGuild = 1169565317920456705;
        Random r = new Random();
        CommonDB _commonDB = new();
        public static async Task Main()
        {
            var program = new Neru();
            await program.StartAsync();
        }
        private async Task StartAsync()
        {
            _client = new(new()
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.GuildVoiceStates | GatewayIntents.Guilds | GatewayIntents.MessageContent | GatewayIntents.GuildMessages
            });
            _client.Log += Log;
            _client.Ready += ReadyAsync;
            _client.SlashCommandExecuted += SlashCommandExecutedAsync;
            _client.MessageReceived += ReceiveMessageAsync;
            _client.MessageReceived += RespondInteractAsync;
            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("token.txt"));
            await _client.StartAsync();
            await Task.Delay(-1); // Wait forever 
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
        //Set Commands
        private async Task SlashCommandExecutedAsync(SocketSlashCommand cmd)
        {
            var cmdName = cmd.CommandName.ToUpperInvariant();
            if (cmdName == "BEEP")
            {
                await cmd.RespondAsync("boop!");
            }
            else if (cmdName == "ROLL")
            {
                await cmd.DeferAsync();
                _ = Task.Run(async () => 
                {
                    var numberParam = cmd.Data.Options.FirstOrDefault(option => option.Name == "number");
                    if (numberParam.Value is long number)
                    {
                        if (number == 0) await cmd.FollowupAsync("Hey, rolling a 0 faces dice is not that fun! But anyway, you got a 0 hehe");
                        else
                        {
                            var result = r.Next(1, (int)number + 1);
                            var percentage = ((double)result / number) * 100;
                            var percentageInt = (int)Math.Round(percentage);
                            switch (percentageInt)
                            {
                                case 0:
                                    await cmd.FollowupAsync($"Yikes!! You rolled a **{result}** that's a *{percentageInt}%* from the maximum value! :hushed:");
                                    break;
                                case < 25:
                                    await cmd.FollowupAsync($"Beep Boop! You rolled a **{result}** that's a *{percentageInt}%* ! Better luck next time! :bubbles:");
                                    break;
                                case < 50:
                                    await cmd.FollowupAsync($"Nice! You rolled a **{result}** that's a *{percentageInt}%* ! Not so bad! Tehe :bubbles:");
                                    break;
                                case < 75:
                                    await cmd.FollowupAsync($"Wow!! You rolled a **{result}** that's a *{percentageInt}%* ! That's really good!:bubbles:");
                                    break;
                                case < 100:
                                    await cmd.FollowupAsync($":0!! You rolled a **{result}** that's a *{percentageInt}%* ! You're really lucky! :bubbles:");
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else await cmd.FollowupAsync("*bubbles come out* H-hey I don't think that's a number! D:");
                });

            }
            else if (cmdName == "MIGRATE")
            {
                await cmd.RespondAsync("Booting up! Give me a sec! :bubbles:");
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (cmd.User.Id == 298907835251687424)
                        {
                            await Console.Out.WriteLineAsync("Beep Boop! Migrating DB!");
                            await using SqliteContext lite = new SqliteContext();

                            var pendingMigrations = await lite.Database.GetPendingMigrationsAsync();
                            if (pendingMigrations.Any())
                            {
                                await lite.Database.MigrateAsync();
                            }
                            await cmd.FollowupAsync(":heart:");
                            await Console.Out.WriteLineAsync("Migration Complete!");
                        }
                        else
                        {
                            await cmd.RespondAsync("O..Oh Sorry you're not allowed to use that! :hushed:");
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"Error during migration: {ex.Message}");
                        await cmd.FollowupAsync(":broken_heart:");
                    }
                });
            }
            else if (cmdName == "BEFRIEND")
            {
                using SqliteContext lite = new SqliteContext();
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if (friend != null)
                { 
                    await cmd.RespondAsync($"Oh! But we're already friends! {friend.UserNickname}!! :bubbles: :heart:");
                }
                else
                {
                    await cmd.RespondAsync("Oh! Let's be friends! :heart: :bubbles: What do you want me to call you?");
                    _requests.Add(new FriendRequest(friend, cmd));
                }
            }
            else if (cmdName == "POKE")
            {
                using SqliteContext lite = new SqliteContext();
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if (friend != null)
                {
                    await cmd.RespondAsync($"Oh hehe :bubbles: *pokes {friend.UserNickname} back* :heart:");
                    friend.UserLove += 0.1f;
                    await lite.SaveChangesAsync();
                }
                else
                {
                    await cmd.RespondAsync("*stares at you* :eyes:");
                }
            }
            else if(cmdName == "UNFRIEND")
            {
                using SqliteContext lite = new SqliteContext();
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if (friend != null)
                {
                    lite.UserRemembrances.Remove(friend);
                    await lite.SaveChangesAsync();
                    await cmd.RespondAsync("Oh :( Alright, hope we can become friends again soon :broken_heart:");
                }
                else
                {
                    await cmd.RespondAsync("Haha silly, but we're not friends yet! :bubbles:");
                }
            }
            else if (cmdName == "LOVE")
            {
                using SqliteContext lite = new SqliteContext();
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if (friend != null)
                {
                    var love = (friend.UserLove / 1000f) * 100;
                    await cmd.RespondAsync($"Hi! {friend.UserNickname} My love towards you is... {(int)Math.Round(love)}% :heart: :bubbles:");
                }
                else
                {
                    await cmd.RespondAsync("Maybe after we're friends we can talk about love :hushed:");
                }
            }
            else if (cmdName == "HEADPAT")
            {
                using SqliteContext lite = new SqliteContext();
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if (friend != null)
                {
                    await cmd.RespondAsync($"Oh hehe :bubbles: Thanks :heart: {friend.UserNickname} :heart:");
                    friend.UserLove += 0.3f;
                    await lite.SaveChangesAsync();
                }
                else
                {
                    await cmd.RespondAsync("*stares at you* :eyes:");
                }
            }
        }
        private async Task RespondInteractAsync(SocketMessage msg)
        {
            var randomNumber = r.Next(100);
            if (randomNumber < 15)
            {
                if (msg.Content.ToUpperInvariant().Contains("YUZU") && !msg.Author.IsBot)
                {
                    await msg.Channel.SendMessageAsync("Love Yuzu :heart:");
                }
            }
            if(randomNumber < 35)
            {
                if(msg.Content.ToUpperInvariant().Contains("WOW") && !msg.Author.IsBot)
                {
                    await msg.Channel.SendMessageAsync("wow");
                }
                if (msg.Content.ToUpperInvariant() == "WHAT" && !msg.Author.IsBot)
                {
                    await msg.Channel.SendMessageAsync("what");
                }
            }
            if(randomNumber < 60)
            {
                if (msg.Content.ToUpperInvariant() == "HEADPAT" && !msg.Author.IsBot)
                {
                    await msg.Channel.SendMessageAsync("Can I have pats too :eyes: ? :bubbles:");
                }
            }
        }
        private async Task ReceiveMessageAsync(SocketMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.Content)) return;
            for (int i = _requests.Count -1; i >= 0; i--)
            {
                if(await _requests[i].TreatMessageAsync(msg.Content, msg.Author, msg.Channel)) _requests.RemoveAt(i);
            }
        }
        
        //Create slash commands
        private Task ReadyAsync()
        {
            // _ means variable is not gonna be used, so compiler stops whining
            _ = Task.Run(async () => {
                var builder = new[]
                {
                    new SlashCommandBuilder()
                    {
                        Name = "beep",
                        Description = "Beeps Neru"
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "roll",
                        Description = "Roll a number"
                    }.AddOption("number", ApplicationCommandOptionType.Integer, "Number to be rolled", isRequired: true),
                    new SlashCommandBuilder()
                    {
                        Name = "migrate",
                        Description = "Migrate SqliteDB"
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "befriend",
                        Description = "Become friends with Neru"
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "poke",
                        Description = "Poke Neru"
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "unfriend",
                        Description = "Are you sure :(?"
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "love",
                        Description = "Check your relationship with Neru"
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "headpat",
                        Description = "Give Neru Headpats"
                    }

                }.Select(x => x.Build()).ToArray();

                foreach (var command in builder)
                {
                    if (Debugger.IsAttached)
                    {
                        await _client.GetGuild(_testingGuild).CreateApplicationCommandAsync(command);
                    }
                    else
                    {
                        await _client.CreateGlobalApplicationCommandAsync(command);
                    }
                }
                if (Debugger.IsAttached)
                {
                    await _client.GetGuild(_testingGuild).BulkOverwriteApplicationCommandAsync(builder);
                }
                else
                {
                    await _client.GetGuild(_testingGuild).DeleteApplicationCommandsAsync();
                    await _client.BulkOverwriteGlobalApplicationCommandsAsync(builder);
                }
            });
            return Task.CompletedTask;
        }
    }

}
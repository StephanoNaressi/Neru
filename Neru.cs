using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Neru.Context;
using System.Diagnostics;
using Neru.Utils;
using Neru.Requests;
using System.Text.RegularExpressions;
using Neru.JsonClasses;
using Newtonsoft.Json;
using Neru.Models;
namespace Neru
{

    public sealed class Neru
    {
        DiscordSocketClient _client;
        List<IPendingRequest> _requests = new();
        ulong _testingGuild = 1169565317920456705;
        Random r = new Random();
        CommonDB _commonDB = new();
        InteractiveResponsesMain _interactiveResponses;
        public static Kana KanaDict {get;set;}
        public static Vocab[] VocabDict { get;set;}
        public static async Task Main()
        {
            var program = new Neru();
            await program.StartAsync();
        }
        private async Task StartAsync()
        {
            _interactiveResponses = JsonConvert.DeserializeObject<InteractiveResponsesMain>(File.ReadAllText("Data/InteractiveResponses.json"));
            KanaDict = JsonConvert.DeserializeObject<Kana>(File.ReadAllText("Data/Kana.json"));
            VocabDict = JsonConvert.DeserializeObject<Vocab[]>(File.ReadAllText("Data/Vocab5.json"));
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
            _client.SelectMenuExecuted += SelectMenuExecuted;
            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("token.txt"));
            await _client.StartAsync();
            await _client.SetGameAsync("Blowing Bubbles~");
            await Task.Delay(-1); // Wait forever 
        }

        private async Task SelectMenuExecuted(SocketMessageComponent arg)
        {
            if (arg.Data.CustomId.StartsWith("roles-"))
            {
                var id = ulong.Parse(arg.Data.Values.First());
                await ((IGuildUser)arg.User).AddRoleAsync(id);
            }
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
            if (cmdName == "INIT")
            {
                if (cmd.User.Id == 298907835251687424 || cmd.User.Id == 144851584478740481)
                {
                    await cmd.DeferAsync();
                    var chan = await (cmd.Channel as ITextChannel).Guild.GetTextChannelAsync(1204049269258977370);

                    _ = Task.Run(async () =>
                    {
                        var ints = new ComponentBuilder()
                            .WithSelectMenu("roles-color",
                            [
                                new("🔴 Red", "1204056730720542720"),
                                new("🟣 Purple", "1204056731114803253"),
                                new("🟢 Green", "1204056732486205492"),
                                new("🩷 Pink", "1204056734008606781"),
                                new("🟠 Orange", "1204056735250260019"),
                                new("🟡 Yellow", "1204056736135249970"),
                                new("🔵 Blue", "1204056739545354291")
                            ])
                            .WithSelectMenu("roles-continent",
                            [
                                new("🌍 Europe", "1204056844239376405"),
                                new("🌎 North America", "1204056844931309600"),
                                new("🌎 South America", "1204056845648535584"),
                                new("🌍 Africa", "1204056848307724298"),
                                new("🌏 Asia", "1204056846617284618"),
                                new("🌏 Oceania", "1204056847309480016")
                            ])
                            .WithSelectMenu("roles-pronouns",
                            [
                                new("🧡 he/him", "1204056886392135731"),
                                new("💛 she/her", "1204056887339909140"),
                                new("💜 they/them", "1204056889218826260"),
                                new("💚 ask", "1204056890003165296"),
                            ]);

                        await chan.SendMessageAsync(embed: new EmbedBuilder
                        {
                            Title = "Roles",
                            Color = Color.Blue,
                        }.Build(), components: ints.Build());
                        await cmd.RespondAsync("Ok", ephemeral: true);
                    });
                }
                else
                {
                    await cmd.RespondAsync("O..Oh Sorry you're not allowed to use that! :hushed:");
                }
            }
            else if (cmdName == "BEEP")
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

                        if (number < 0) await cmd.FollowupAsync("Woah there! Now how am I supposed to roll a negative die?");
                        else if (number == 0) await cmd.FollowupAsync("Hey, rolling a die with 0 faces is not that fun! But anyway, you got a 0 hehe");
                        else if (number == 1) await cmd.FollowupAsync("Wow!!!! You got a *drumroll* :drum: 1!!! Incredible");
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
                    await cmd.RespondAsync($"Hi! {friend.UserNickname} My love towards you is... {(int)Math.Round(love)}% ({Math.Round(friend.UserLove, 1)}) :heart: :bubbles:");
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
                    await cmd.RespondAsync($":3 :bubbles: :heart: {friend.UserNickname} :heart:");
                    friend.UserLove += 0.6f;
                    await lite.SaveChangesAsync();
                }
                else
                {
                    await cmd.RespondAsync("*stares at you* :eyes:");
                }
            }
            else if (cmdName == "RPS")
            {
                using SqliteContext lite = new SqliteContext();
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if (friend != null)
                {
                    await cmd.RespondAsync("Alright! Rock...Paper...Scissors! . . . :fist: :newspaper2: :scissors:");
                    _requests.Add(new RockPaperScissorsRequest(cmd, r, friend));
                }
                else
                {
                    await cmd.RespondAsync("I only play games with my friends :eyes:");
                }
            }
            else if (cmdName == "SETHEALTH")
            {

                using SqliteContext lite = new SqliteContext();
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if (friend != null)
                {
                    string valueParam = cmd.Data.Options.FirstOrDefault(option => option.Name == "health").Value.ToString();

                    try
                    {
                        if (valueParam != null && Regex.IsMatch(valueParam, @"^[+-]?\d+$"))
                        {
                            if (valueParam.StartsWith("+") || (valueParam.StartsWith("-")))
                            {
                                if (Regex.Count(valueParam, @"^[+-]\d+$") == 1)
                                {
                                    friend.UserHP += int.Parse(valueParam);
                                    await lite.SaveChangesAsync();
                                    await cmd.RespondAsync($"Done calculating! Your HP now is {friend.UserHP}");
                                }
                                else
                                {
                                    await cmd.RespondAsync($"Please input a proper number! :bubbles:");
                                }
                            }
                            else
                            {
                                friend.UserHP = int.Parse(valueParam);
                                await lite.SaveChangesAsync();
                                await cmd.RespondAsync($"Done! Your HP now is {friend.UserHP}");
                            }
                        }
                        else
                        {
                            await cmd.RespondAsync($"Please input a proper number! :bubbles:");

                        }
                    }
                    catch (Exception e)
                    {
                        await cmd.RespondAsync($"Wow! That number was a bit difficult to handle, try again with a simpler one >n<! :bubbles:");
                    }
                    
                }
                        
                else
                {
                    await cmd.RespondAsync("You're not my friend so I dont keep track of your health, sorry! :bubbles:");
                }
            }
            else if (cmdName == "LISTFRIENDS")
            {
                cmd.DeferAsync();
                using SqliteContext lite = new SqliteContext();
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if (cmd.User.Id == 298907835251687424)
                {
                    var friends = "My friends are: ";
                    foreach (var item in lite.UserRemembrances)
                    {
                        friends += $" {item.UserNickname} With love: {MathF.Round(item.UserLove,2)} ";
                    }
                    
                    await cmd.FollowupAsync(friends);
                }
                else
                {
                    await cmd.FollowupAsync("Sorry! Only Nano can use that command!"); 
                }
            }
            else if(cmdName == "ROLEASSIGNMENT")
            {
                if (cmd.User.Id == 298907835251687424)
                {

                }
                else
                {
                    await cmd.FollowupAsync("Sorry! Only Nano can use that command!");
                }
            }
            else if (cmdName == "WIKI")
            {
                cmd.DeferAsync();
                using SqliteContext lite = new SqliteContext();
                Console.WriteLine(cmd.Data.Options.First().Name);
                try
                {
                    switch (cmd.Data.Options.First().Name)
                    {
                        case "add":
                            if (cmd.User.Id != 298907835251687424) await cmd.RespondAsync("Sorry! Only Nano can add to the database :bubbles:");
                            else
                            {
                                string fieldName = cmd.Data.Options.First().Options.FirstOrDefault(option => option.Name == "field_name").Value.ToString();
                                string fieldDescription = cmd.Data.Options.First().Options.FirstOrDefault(option => option.Name == "description").Value.ToString();
                                var wiki = new MyWiki();
                                wiki.Title = fieldName;
                                wiki.Description = fieldDescription;
                                wiki.EntryID = fieldName.ToUpperInvariant();
                                lite.MyWikis.Add(wiki);
                                await lite.SaveChangesAsync();
                                await cmd.FollowupAsync($"Added {fieldName} to the wiki! :bubbles: ");
                            }
                            break;
                        case "get":
                            string query = cmd.Data.Options.First().Options.FirstOrDefault(option => option.Name == "query").Value.ToString();
                            var result = lite.MyWikis.FirstOrDefault(x => x.EntryID.Equals(query.ToUpperInvariant()));
                            if (result != null)
                            {
                                await cmd.FollowupAsync($"## {result.Title} \n```{result.Description}```");
                            }
                            else await cmd.FollowupAsync("Oops, couldn't find your query, maybe ask Nano about it! :bubbles:");
                            break;
                        case "list":
                            var titleFields = String.Join(", ",lite.MyWikis.Select(x => x.Title));
                            if (titleFields != null) await cmd.FollowupAsync($"```{titleFields}```");
                            else await cmd.FollowupAsync("Oop! Something went wrong! :bubbles:");
                            break;
                        case "remove":
                            if (cmd.User.Id != 298907835251687424) await cmd.RespondAsync("Sorry! Only Nano can add to the database :bubbles:");
                            else
                            {
                                string entryId = cmd.Data.Options.First().Options.FirstOrDefault(option => option.Name == "field_id").Value.ToString();
                                var fieldSearch = lite.MyWikis.FirstOrDefault(x => x.EntryID.Equals(entryId.ToUpperInvariant()));
                                if (fieldSearch != null)
                                {
                                    lite.MyWikis.Remove(fieldSearch);
                                    await lite.SaveChangesAsync();
                                    await cmd.FollowupAsync($"{fieldSearch.Title} has been removed! o7 :bubbles:");
                                }
                                else await cmd.FollowupAsync("Oops, couldn't find your query, maybe ask Nano about it! :bubbles:");
                            };
                            break;
                        case null: Console.WriteLine("Reached default"); break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e}");
                    await cmd.FollowupAsync($"Wow! Something went wrong, try again! >n<! :bubbles:");
                }

            }
            else if(cmdName == "JP")
            {
                await cmd.DeferAsync();
                using SqliteContext lite = new SqliteContext();
                Console.WriteLine(cmd.Data.Options.First().Name);
                var friend = await _commonDB.FindByIdAsync(lite, cmd.User.Id.ToString());
                if(friend != null)
                {
                    try
                    {

                        switch (cmd.Data.Options.First().Name)
                        {
                            case "hiragana":
                                await cmd.FollowupAsync($"Okay! Let me choose some hiraganas for you {friend.UserNickname}");
                                Dictionary<string, string> Hresponse = new();
                                var Hrn = r.Next(1, 10);
                                //5 random kanas
                                for (int i = 0; i < Hrn; i++)
                                {
                                    var randomIndex = r.Next(0, Neru.KanaDict.Hiragana.Count);
                                    var randomElement = Neru.KanaDict.Hiragana.ElementAt(randomIndex);
                                    if (!Hresponse.ContainsKey(randomElement.Key)) Hresponse.Add(randomElement.Key, randomElement.Value);
                                }

                                var HkanasChosen = string.Join(" ", Hresponse.Keys);
                                var HvaluesChosen = string.Join(" ", Hresponse.Values);

                                await cmd.FollowupAsync($"The Kanas are: \n# {HkanasChosen}");
                                _requests.Add(new KanaRequest(friend, cmd, HvaluesChosen, _commonDB));
                                break;
                            case "katakana":
                                await cmd.FollowupAsync($"Okay! Let me choose some katakanas for you {friend.UserNickname}");
                                //5 random kanas
                                Dictionary<string, string> Kresponse = new();
                                var Krn = r.Next(1, 10);

                                for (int i = 0; i < Krn; i++)
                                {
                                    var randomIndex = r.Next(0, Neru.KanaDict.Hiragana.Count);
                                    var randomElement = Neru.KanaDict.Katakana.ElementAt(randomIndex);
                                    if (!Kresponse.ContainsKey(randomElement.Key)) Kresponse.Add(randomElement.Key, randomElement.Value);
                                }

                                var KkanasChosen = string.Join(" ", Kresponse.Keys);
                                var KvaluesChosen = string.Join(" ", Kresponse.Values);

                                await cmd.FollowupAsync($"The Kanas are: \n# {KkanasChosen}");
                                _requests.Add(new KanaRequest(friend, cmd, KvaluesChosen, _commonDB));

                                break;
                            case "vocab":
                                await cmd.FollowupAsync($"Okay! Let me choose a word for you {friend.UserNickname}");
                                var randomVocab = VocabDict[r.Next(0, VocabDict.Length)];

                                await cmd.FollowupAsync($"The word is: \n# {randomVocab.word}");
                                _requests.Add(new VocabRequest(friend, cmd, randomVocab, _commonDB));
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e}");
                        await cmd.FollowupAsync($"Wow! Something went wrong, try again! >n<! :bubbles:");
                    }
                }
                else
                {
                    await cmd.FollowupAsync("You're not my friend but we can become friends if you do /befriend :), sorry! :bubbles:");
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
                    await _commonDB.AddLove(0.30f, msg.Author.Id.ToString());
                    await msg.Channel.SendMessageAsync(_interactiveResponses.Responses.Yuzu[r.Next(0, _interactiveResponses.Responses.Yuzu.Count)]);
                }
            }
            if(randomNumber < 35)
            {
                if(msg.Content.ToUpperInvariant().Contains("WOW") && !msg.Author.IsBot)
                {
                    await _commonDB.AddLove(0.15f, msg.Author.Id.ToString());
                    await msg.Channel.SendMessageAsync("wow");
                }
                if (msg.Content.ToUpperInvariant() == "WHAT" && !msg.Author.IsBot)
                {
                    await _commonDB.AddLove(0.25f, msg.Author.Id.ToString());
                    await msg.Channel.SendMessageAsync("what");
                }
            }
            if(randomNumber < 60)
            {
                if (msg.Content.ToUpperInvariant() == "PAT" && !msg.Author.IsBot)
                {
                    await _commonDB.AddLove(0.45f, msg.Author.Id.ToString());
                    await msg.Channel.SendMessageAsync(_interactiveResponses.Responses.Pat[r.Next(0,_interactiveResponses.Responses.Pat.Count)]);
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
                        Name = "init",
                        Description = "Init server stuff"
                    },
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
                        Name = "roleassignment",
                        Description = "Setup role assignment"
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
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "rps",
                        Description = "Rock Paper Scissors!"
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "sethealth",
                        Description = "Set your health value"
                    }.AddOption("health", ApplicationCommandOptionType.String, "New Health value or operator", isRequired: true),
                    new SlashCommandBuilder()
                    {
                        Name = "listfriends",
                        Description = "Check Neru friends"
                    },
                    new SlashCommandBuilder()
                    {
                        Name = "wiki",
                        Description = "Ask Neru about the wiki"
                    }
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("add")
                        .WithDescription("Add a term to the wiki")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("field_name", ApplicationCommandOptionType.String, "Name of your field", isRequired: true)
                        .AddOption("description", ApplicationCommandOptionType.String, "Description of your field", isRequired: true)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Get a description from the wiki")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("query",ApplicationCommandOptionType.String, "What do you want to know?", isRequired:true)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("list")
                        .WithDescription("Get a list of all values from the wiki")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("remove")
                        .WithDescription("remove a term to the wiki")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("field_id", ApplicationCommandOptionType.String, "id of your field", isRequired: true)
                    ),
                    new SlashCommandBuilder()
                    {
                        Name = "jp",
                        Description = "Study some Japanese"
                    }
                    .AddOption (new SlashCommandOptionBuilder()
                        .WithName("hiragana")
                        .WithDescription("translate hiragana")
                        .WithType (ApplicationCommandOptionType.SubCommand)
                    )
                    .AddOption (new SlashCommandOptionBuilder()
                        .WithName("katakana")
                        .WithDescription("translate katakana")
                        .WithType (ApplicationCommandOptionType.SubCommand)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("vocab")
                        .WithDescription("translate vocabulary")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                    )
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
using Discord;
public interface IPendingRequest
{
    Task<bool> TreatMessageAsync(string msg, IUser user, IChannel channel);
}
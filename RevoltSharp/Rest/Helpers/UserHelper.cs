﻿using RevoltSharp.Rest;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for users.
/// </summary>
public static class UserHelper
{
    /// <inheritdoc cref="GetUserAsync(RevoltRestClient, string)" />
    public static Task<User?> GetUserAsync(this Server server, string userId)
        => GetUserAsync(server.Client.Rest, userId);

    /// <summary>
    /// Test
    /// </summary>
    /// <returns><see cref="User" /> or <see langword="null" /> if no mutual servers, groups or dms.</returns>
    public static async Task<User?> GetUserAsync(this RevoltRestClient rest, string userId)
    {
        Conditions.UserIdEmpty(userId, "GetUserAsync");

        if (rest.Client.WebSocket != null && rest.Client.WebSocket.UserCache.TryGetValue(userId, out User User))
            return User;

        UserJson? Data = await rest.GetAsync<UserJson>($"users/{userId}");
        if (Data == null)
            return null;

        User user = new User(rest.Client, Data);
        if (rest.Client.WebSocket != null)
            rest.Client.WebSocket.UserCache.TryAdd(userId, user);
        return user;
    }
    /// <inheritdoc cref="GetProfileAsync(RevoltRestClient, string)" />
    public static Task<Profile?> GetProfileAsync(this User user)
        => GetProfileAsync(user.Client.Rest, user.Id);

    public static async Task<Profile?> GetProfileAsync(this RevoltRestClient rest, string userId)
    {
        Conditions.UserIdEmpty(userId, "GetProfileAsync");

        ProfileJson? Data = await rest.GetAsync<ProfileJson>($"users/{userId}/profile");
        if (Data == null)
            return null;

        return new Profile(rest.Client, Data);
    }

    /// <inheritdoc cref="GetUserDMChannelAsync(RevoltRestClient, string)" />
    public static Task<DMChannel?> GetDMChannelAsync(this User user)
        => GetUserDMChannelAsync(user.Client.Rest, user.Id);

    public static async Task<DMChannel?> GetUserDMChannelAsync(this RevoltRestClient rest, string userId)
    {
        Conditions.UserIdEmpty(userId, "GetUserDMChannel");

        ChannelJson? Data = await rest.GetAsync<ChannelJson>($"users/{userId}/dm");
        if (Data == null)
            return null;
        return Channel.Create(rest.Client, Data) as DMChannel;
    }

    /// <inheritdoc cref="BlockUserAsync(RevoltRestClient, string)" />
    public static Task<User> BlockAsync(this User user)
        => BlockUserAsync(user.Client.Rest, user.Id);

    public static async Task<User> BlockUserAsync(this RevoltRestClient rest, string userId)
    {
        Conditions.UserIdEmpty(userId, "BlockUserAsync");

        UserJson Data = await rest.PutAsync<UserJson>($"users/{userId}/block");
        return new User(rest.Client, Data);
    }

    /// <inheritdoc cref="UnBlockUserAsync(RevoltRestClient, string)" />
    public static Task UnBlockAsync(this User user)
        => UnBlockUserAsync(user.Client.Rest, user.Id);

    public static async Task UnBlockUserAsync(this RevoltRestClient rest, string userId)
    {
        Conditions.UserIdEmpty(userId, "UnBlockUserAsync");

        await rest.DeleteAsync($"users/{userId}/block");
    }
}

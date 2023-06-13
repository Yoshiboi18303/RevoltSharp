﻿using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp;

/// <summary>
/// Private group channel with a list of user
/// </summary>
public class GroupChannel : Channel
{
    internal GroupChannel(RevoltClient client, ChannelJson model) : base(client, model)
    {
        Type = ChannelType.Group;
        Recipents = model.Recipients != null ? model.Recipients.ToHashSet() : new HashSet<string>();
        if (client.WebSocket != null)
        {
            foreach (string u in Recipents)
            {
                if (client.WebSocket.UserCache.TryGetValue(u, out User user))
                {
                    user.InternalMutualGroups.TryAdd(Id, this);
                    CachedUsers.TryAdd(user.Id, user);
                }
            }
        }
        Description = model.Description;
        LastMessageId = model.LastMessageId;
        Name = model.Name;
        OwnerId = model.Owner;
        Permissions = new ChannelPermissions(null, model.Permissions, 0);
        Icon = Attachment.Create(client, model.Icon);
        IsNsfw = model.Nsfw;
    }

    /// <summary>
    /// Default permissions for all users
    /// </summary>
    public ChannelPermissions Permissions { get; internal set; }

    internal HashSet<string> Recipents { get; set; }

    internal ConcurrentDictionary<string, User> CachedUsers { get; set; } = new ConcurrentDictionary<string, User>();

    /// <summary>
    /// List of users in the channel
    /// </summary>
    [JsonIgnore]
    public IReadOnlyCollection<User> Users
        => (IReadOnlyCollection<User>)CachedUsers.Values;


    /// <summary>
    /// The last message id sent in this Group channel.
    /// </summary>
    public string LastMessageId { get; internal set; }

    /// <summary>
    /// Owner of the channel
    /// </summary>
    public string OwnerId { get; internal set; }

    public User? Owner => Client.GetUser(OwnerId);

    /// <summary>
    /// Name of the channel
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// Description of the channel
    /// </summary>
    public string Description { get; internal set; }

    /// <summary>
    /// Icon attachment of the channel
    /// </summary>
    /// <remarks>
    /// This may be <see langword="null" />
    /// </remarks>
    public Attachment? Icon { get; internal set; }

    /// <summary>
    /// Channel has nsfw content
    /// </summary>
    public bool IsNsfw { get; internal set; }


    internal override void Update(PartialChannelJson json)
    {
        if (json.Name.HasValue)
            Name = json.Name.Value;

        if (json.Icon.HasValue)
            Icon = Attachment.Create(Client, json.Icon.Value);

        if (json.Description.HasValue)
            Description = json.Description.Value;

        if (json.DefaultPermissions.HasValue)
            Permissions.RawAllowed = json.DefaultPermissions.Value.Allowed;

        if (json.Permissions.HasValue)
            Permissions.RawAllowed = json.Permissions.Value;

        if (json.IsNsfw.HasValue)
            IsNsfw = json.IsNsfw.Value;

        if (json.OwnerId.HasValue)
            OwnerId = json.OwnerId.Value;

    }

    internal void AddUser(User user)
    {
        try
        {
            Recipents.Add(user.Id);
        }
        catch { }
        user.InternalMutualGroups.TryAdd(Id, this);
        CachedUsers.TryAdd(user.Id, user);
    }

    internal void RemoveUser(User user, bool delete)
    {
        if (!delete)
        {
            CachedUsers.TryRemove(user.Id, out _);
            try
            {
                Recipents.Remove(user.Id);
            }
            catch { }
        }
        user.InternalMutualGroups.TryRemove(Id, out _);
        if (user.Id != user.Client.CurrentUser.Id && !user.HasMutuals)
        {
            user.Client.WebSocket.UserCache.TryRemove(user.Id, out _);
        }
    }

    internal void RemoveUser(RevoltClient client, string userId)
    {
        CachedUsers.TryRemove(userId, out _);
        try
        {
            Recipents.Remove(userId);
        }
        catch { }

        if (userId != client.CurrentUser.Id)
        {
            client.WebSocket.UserCache.TryRemove(userId, out _);
        }
    }
}

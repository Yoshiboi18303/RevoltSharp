﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;

public class Server : CreatedEntity
{
    internal Server(RevoltClient client, ServerJson model) : base(client, model.Id)
    {
        Name = model.Name;
        DefaultPermissions = new ServerPermissions(this, model.DefaultPermissions);
        Description = model.Description;
        Banner = Attachment.Create(client, model.Banner);
        Icon = Attachment.Create(client, model.Icon);
        ChannelIds = model.Channels != null ? model.Channels.ToHashSet() : new HashSet<string>();
        OwnerId = model.Owner;
        InternalRoles = model.Roles != null
            ? new ConcurrentDictionary<string, Role>(model.Roles.ToDictionary(x => x.Key, x => new Role(client, x.Value, model.Id, x.Key)))
            : new ConcurrentDictionary<string, Role>();
        HasAnalytics = model.Analytics;
        IsDiscoverable = model.Discoverable;
        IsNsfw = model.Nsfw;
        SystemMessages = new ServerSystemMessages(client, model.SystemMessages);
    }

    /// <summary>
    /// Id of the server.
    /// </summary>
    public new string Id => base.Id;

    /// <summary>
    /// Date of when the server was created.
    /// </summary>
    public new DateTimeOffset CreatedAt => base.CreatedAt;

    public string OwnerId { get; internal set; }

    public async Task<ServerMember> GetOwnerAsync()
    {
        if (InternalMembers.TryGetValue(OwnerId, out ServerMember SM))
            return SM;
        return await Client.Rest.GetMemberAsync(Id, OwnerId);
    }

    public string Name { get; internal set; }

    public string Description { get; internal set; }

    internal HashSet<string> ChannelIds { get; set; }

    //public ServerCategory[] Categories;
    public ServerSystemMessages SystemMessages;

    internal ConcurrentDictionary<string, Role> InternalRoles { get; set; }

    internal ConcurrentDictionary<string, Emoji> InternalEmojis { get; set; } = new ConcurrentDictionary<string, Emoji>();

    internal ConcurrentDictionary<string, ServerMember> InternalMembers { get; set; } = new ConcurrentDictionary<string, ServerMember>();

    public ServerPermissions DefaultPermissions { get; internal set; }

    public Attachment? Icon { get; internal set; }

    public string GetIconUrl() => Icon != null ? Icon.GetUrl() : string.Empty;

    public Attachment? Banner { get; internal set; }

    public string GetBannerUrl() => Banner != null ? Banner.GetUrl() : string.Empty;

    public bool HasAnalytics { get; internal set; }

    public bool IsDiscoverable { get; internal set; }

    public bool IsNsfw { get; internal set; }

    public ServerMember? GetCachedMember(string userId)
    {
        if (InternalMembers.TryGetValue(userId, out ServerMember member))
            return member;
        return null;
    }

    [JsonIgnore]
    public IReadOnlyCollection<ServerMember> CachedMembers
        => (IReadOnlyCollection<ServerMember>)InternalMembers.Values;


    public Role? GetRole(string roleId)
    {
        if (InternalRoles.TryGetValue(roleId, out Role role))
            return role;
        return null;
    }

    [JsonIgnore]
    public IReadOnlyCollection<Role> Roles
        => (IReadOnlyCollection<Role>)InternalRoles.Values;

    public Emoji? GetEmoji(string emojiId)
    {
        if (InternalEmojis.TryGetValue(emojiId, out Emoji emoji))
            return emoji;
        return null;
    }

    [JsonIgnore]
    public IReadOnlyCollection<Emoji> Emojis
        => (IReadOnlyCollection<Emoji>)InternalEmojis.Values;

    public TextChannel? GetTextChannel(string channelId)
    {
        if (!Client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel chan))
            return null;

        if (chan is not TextChannel textChannel)
            return null;

        if (textChannel.ServerId != Id)
            return null;

        return textChannel;
    }

    public VoiceChannel? GetVoiceChannel(string channelId)
    {
        if (!Client.WebSocket.ChannelCache.TryGetValue(channelId, out Channel chan))
            return null;

        if (chan is not VoiceChannel voiceChannel)
            return null;

        if (voiceChannel.ServerId != Id)
            return null;

        return voiceChannel;
    }

    internal void Update(PartialServerJson json)
    {
        if (json.Name.HasValue)
            Name = json.Name.Value;

        if (json.Icon.HasValue)
            Icon = Attachment.Create(Client, json.Icon.Value);

        if (json.Banner.HasValue)
            Banner = Attachment.Create(Client, json.Icon.Value);

        if (json.DefaultPermissions.HasValue)
            DefaultPermissions = new ServerPermissions(this, json.DefaultPermissions.Value);

        if (json.Description.HasValue)
            Description = json.Description.Value;

        if (json.Analytics.HasValue)
            HasAnalytics = json.Analytics.Value;

        if (json.Discoverable.HasValue)
            IsDiscoverable = json.Discoverable.Value;

        if (json.Nsfw.HasValue)
            IsNsfw = json.Nsfw.Value;

        if (json.Owner.HasValue)
            OwnerId = json.Owner.Value;

        if (json.SystemMessages.HasValue)
        {
            SystemMessages = new ServerSystemMessages(Client, json.SystemMessages.Value);
        }
    }

    internal Server Clone()
    {
        return (Server)this.MemberwiseClone();
    }

    internal void AddMember(ServerMember member)
    {
        InternalMembers.TryAdd(member.Id, member);
        member.User.InternalMutualServers.TryAdd(Id, this);
    }

    internal void RemoveMember(User user)
    {
        InternalMembers.TryRemove(user.Id, out _);

        user.InternalMutualServers.TryRemove(Id, out _);
        if (user.Id != user.Client.CurrentUser.Id && !user.HasMutuals)
        {
            user.Client.WebSocket.UserCache.TryRemove(user.Id, out _);
        }
    }
}

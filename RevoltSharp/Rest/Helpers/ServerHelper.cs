﻿using RevoltSharp.Core.Servers;
using RevoltSharp.Rest;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for servers.
/// </summary>
public static class ServerHelper
{
    public static async Task<Server?> GetServerAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdEmpty(serverId, "GetServerAsync");

        if (rest.Client.WebSocket != null && rest.Client.WebSocket.ServerCache.TryGetValue(serverId, out Server server))
            return server;

        ServerJson? Server = await rest.GetAsync<ServerJson>($"/servers/{serverId}");
        if (Server == null)
            return null;

        return new Server(rest.Client, Server);
    }

    public static Task<IReadOnlyCollection<ServerBan>> GetBansAsync(this Server server)
        => GetBansAsync(server.Client.Rest, server.Id);

    public static async Task<IReadOnlyCollection<ServerBan>> GetBansAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdEmpty(serverId, "GetBansAsync");

        ServerBansJson? Bans = await rest.GetAsync<ServerBansJson>($"/servers/{serverId}/bans");
        if (Bans == null)
            return System.Array.Empty<ServerBan>();

        return Bans.Users.Select(x => new ServerBan(rest.Client, x, Bans.Bans.Where(b => b.Ids.UserId == x.Id).FirstOrDefault())).ToImmutableArray();
        
    }

    public static Task LeaveAsync(this Server server)
        => LeaveServerAsync(server.Client.Rest, server.Id);

    public static async Task LeaveServerAsync(this RevoltRestClient rest, string serverId)
    {
        Conditions.ServerIdEmpty(serverId, "LeaveServerAsync");

        await rest.DeleteAsync($"/servers/{serverId}");
    }
}

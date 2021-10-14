﻿namespace RevoltSharp
{
    /// <summary>
    /// Do not use this class! only used for <see cref="RevoltClient"/>
    /// </summary>
    public class ClientEvents
    {
        public delegate void RevoltEvent<TValue>(TValue value);
        public delegate void RevoltEvent<TValue, TValue2>(TValue value, TValue2 value2);
        public delegate void RevoltEvent<TValue, TValue2, TValue3>(TValue values, TValue2 values2, TValue3 value3);

        /// <summary>
        /// Receive message events from websocket in a <see cref="TextChannel"/> or <seealso cref="GroupChannel"/>
        /// </summary>
        public event RevoltEvent<Message> OnMessageRecieved;
        internal void InvokeMessageRecieved(Message msg)
        {
            OnMessageRecieved?.Invoke(msg);
        }

        /// <summary>
        /// Event used when the <see cref="RevoltClient"/> WebSocket has fully loaded with cached data and <see cref="RevoltClient.CurrentUser"/> is set.
        /// </summary>
        public event RevoltEvent<SelfUser> OnReady;
        internal void InvokeReady(SelfUser user)
        {
            OnReady?.Invoke(user);
        }

        public event RevoltEvent<Channel, string, string> OnMessageUpdated;
        internal void InvokeMessageUpdated(Channel chan, string message, string content)
        {
            OnMessageUpdated?.Invoke(chan, message, content);
        }

        public event RevoltEvent<Channel, string> OnMessageDeleted;
        internal void InvokeMessageDeleted(Channel chan, string msg)
        {
            OnMessageDeleted?.Invoke(chan, msg);
        }

        public event RevoltEvent<Channel> OnChannelCreated;
        internal void InvokeChannelCreated(Channel chan)
        {
            OnChannelCreated?.Invoke(chan);
        }

        public event RevoltEvent<Channel, Channel> OnChannelUpdated;
        internal void InvokeChannelUpdated(Channel old, Channel newc)
        {
            OnChannelUpdated?.Invoke(old, newc);
        }

        public event RevoltEvent<Channel> OnChannelDeleted;
        internal void InvokeChannelDeleted(Channel chan)
        {
            OnChannelDeleted?.Invoke(chan);
        }


        public event RevoltEvent<GroupChannel, SelfUser> OnGroupJoined;
        internal void InvokeGroupJoined(GroupChannel chan, SelfUser user)
        {
            OnGroupJoined?.Invoke(chan, user);
        }

        public event RevoltEvent<GroupChannel, SelfUser> OnGroupLeft;
        internal void InvokeGroupLeft(GroupChannel chan, SelfUser user)
        {
            OnGroupLeft?.Invoke(chan, user);
        }

        public event RevoltEvent<GroupChannel, User> OnGroupUserJoined;
        internal void InvokeGroupUserJoined(GroupChannel chan, User user)
        {
            OnGroupUserJoined?.Invoke(chan, user);
        }

        public event RevoltEvent<GroupChannel, User> OnGroupUserLeft;
        internal void InvokeGroupUserLeft(GroupChannel chan, User user)
        {
            OnGroupUserLeft?.Invoke(chan, user);
        }


        public event RevoltEvent<Server, Server> OnServerUpdated;
        internal void InvokeServerUpdated(Server old, Server news)
        {
            OnServerUpdated?.Invoke(old, news);
        }

        public event RevoltEvent<Server, SelfUser> OnServerJoined;
        internal void InvokeServerJoined(Server server, SelfUser user)
        {
            OnServerJoined?.Invoke(server, user);
        }

        public event RevoltEvent<Server> OnServerLeft;
        internal void InvokeServerLeft(Server server)
        {
            OnServerLeft?.Invoke(server);
        }

        public event RevoltEvent<Server, User> OnMemberJoined;
        internal void InvokeMemberJoined(Server server, User user)
        {
            OnMemberJoined?.Invoke(server, user);
        }

        public event RevoltEvent<Server, User> OnMemberLeft;
        internal void InvokeMemberLeft(Server server, User user)
        {
            OnMemberLeft?.Invoke(server, user);
        }

        public event RevoltEvent<Role> OnRoleCreated;
        internal void InvokeRoleCreated(Role role)
        {
            OnRoleCreated?.Invoke(role);
        }

        public event RevoltEvent<Role> OnRoleDeleted;
        internal void InvokeRoleDeleted(Role role)
        {
            OnRoleDeleted?.Invoke(role);
        }

        public event RevoltEvent<Role, Role> OnRoleUpdated;
        internal void InvokeRoleUpdated(Role old, Role newr)
        {
            OnRoleUpdated?.Invoke(old, newr);
        }

        public event RevoltEvent<WebSocketError> OnWebSocketError;
        internal void InvokeWebSocketError(WebSocketError error)
        {
            OnWebSocketError?.Invoke(error);
        }

        public event RevoltEvent<User, User> OnUserUpdated;
        internal void InvokeUserUpdated(User old, User newu)
        {
            OnUserUpdated?.Invoke(old, newu);
        }

        public event RevoltEvent<SelfUser, SelfUser> OnCurrentUserUpdated;
        internal void InvokeCurrentUserUpdated(SelfUser old, SelfUser newu)
        {
            OnCurrentUserUpdated?.Invoke(old, newu);
        }
    }
}

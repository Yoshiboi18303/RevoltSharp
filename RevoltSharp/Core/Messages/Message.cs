﻿using System;

namespace RevoltSharp
{

    /// <summary>
    /// Base chat messages that can be casted to <see cref="UserMessage"/> or <see cref="SystemMessage{Type}"/>
    /// </summary>
    public abstract class Message : CreatedEntity
    {
        internal Message(RevoltClient client, MessageJson model)
            : base(client, model.MessageId)
        {
            ChannelId = model.ChannelId;
            ServerId = null!;
            if (model.Webhook != null)
            {
                Type = MessageType.Webhook;
                AuthorId = model.Webhook.Id;
                Author = new User(Client, model.Webhook);
            }
            else
            {
                AuthorId = model.AuthorId;
                Author = client.GetUser(model.AuthorId);
                if (Author != null)
                    Type = Author.IsBot ? MessageType.Bot : MessageType.User;

            }
            Channel = client.GetChannel(model.ChannelId);

            if (Channel != null && Channel is ServerChannel SC)
                ServerId = SC.ServerId;
        }

        internal static Message Create(RevoltClient client, MessageJson model)
        {
            if (model.AuthorId == "00000000000000000000000000")
            {
                if (model.System != null)
                {
                    switch (model.System.SystemType)
                    {
                        case "text":
                            return new SystemMessage<SystemText>(client, model, new SystemText());
                        case "user_added":
                            return new SystemMessage<SystemUserAdded>(client, model, new SystemUserAdded());
                        case "user_remove":
                            return new SystemMessage<SystemUserRemoved>(client, model, new SystemUserRemoved());
                        case "user_joined":
                            return new SystemMessage<SystemUserJoined>(client, model, new SystemUserJoined());
                        case "user_left":
                            return new SystemMessage<SystemUserLeft>(client, model, new SystemUserLeft());
                        case "user_kicked":
                            return new SystemMessage<SystemUserKicked>(client, model, new SystemUserKicked());
                        case "user_banned":
                            return new SystemMessage<SystemUserBanned>(client, model, new SystemUserBanned());
                        case "channel_renamed":
                            return new SystemMessage<SystemChannelRenamed>(client, model, new SystemChannelRenamed());
                        case "channel_description_changed":
                            return new SystemMessage<SystemChannelDescriptionChanged>(client, model, new SystemChannelDescriptionChanged());
                        case "channel_icon_changed":
                            return new SystemMessage<SystemChannelIconChanged>(client, model, new SystemChannelIconChanged());
                        case "channel_ownership_changed":
                            return new SystemMessage<SystemChannelOwnershipChanged>(client, model, new SystemChannelOwnershipChanged());
                    }
                }
                return new SystemMessage<SystemUnknown>(client, model, new SystemUnknown());
            }

            return new UserMessage(client, model);
        }

        /// <summary>
        /// Id of the message.
        /// </summary>
        public new string Id => base.Id;

        /// <summary>
        /// Date of when the message was created.
        /// </summary>
        public new DateTimeOffset CreatedAt => base.CreatedAt;

        /// <summary>
        /// Parent channel id of the message
        /// </summary>
        public string ChannelId { get; internal set; }

        /// <summary>
        /// Channel that the Message is from.
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/>.
        /// </remarks>
        public Channel? Channel { get; internal set; }

        public string? ServerId { get; internal set; }

        /// <summary>
        /// Server that the Message is from.
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null" /> if using <see cref="ClientMode.Http"/> or invalid channel context.
        /// </remarks>
        public Server? Server => Client.GetServer(ServerId);

        /// <summary>
        /// Id of the user who posted the message
        /// </summary>
        public string AuthorId { get; internal set; }

        /// <summary>
        /// User who posted the message
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null" /> for system/webhook messages.
        /// </remarks>
        public User? Author { get; internal set; }

        /// <summary>
        /// Get the type of message this is.
        /// </summary>
        public MessageType Type { get; internal set; } = MessageType.User;

        /// <summary> Returns a string that represents the current object.</summary>
        /// <returns> User/bot/system message </returns>
        public override string ToString()
        {
            return Type.ToString() + " message";
        }
    }
    public enum MessageType
    {
        User, Bot, System, Webhook
    }
}
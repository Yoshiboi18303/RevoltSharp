﻿using Optionals;

namespace RevoltSharp
{
    public class MessageMasquerade
    {
        public MessageMasquerade(string name, string avatar = "", RevoltColor color = null)
        {
            Name = name;
            Avatar = avatar;
            Color = color == null ? new RevoltColor("") : color;
        }
        internal MessageMasquerade(MessageMasqueradeJson model)
        {
            Name = model.Name;
            Avatar = model.Avatar;
            if (model.Color.HasValue && model.Color.Value != null)
                Color = new RevoltColor(model.Color.Value);
        }
        public string Name;
        public string Avatar;
        public RevoltColor? Color;

        internal MessageMasqueradeJson ToJson()
        {
            MessageMasqueradeJson Json = new MessageMasqueradeJson();
            if (!string.IsNullOrEmpty(Name))
                Json.Name = Name;

            if (!string.IsNullOrEmpty(Avatar))
                Json.Avatar = Avatar;

            if (Color != null && !Color.IsEmpty)
                Json.Color = Optional.Some(Color.Hex);

            return Json;
        }
    }
}

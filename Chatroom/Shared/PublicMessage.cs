using System;

namespace Chatroom.Shared
{
    public class PublicMessage
    {
        public User User { get; set; }
        public string Content { get; set; }
        public bool IsOwn { get; set; } // nullable for client-to-server
        public DateTime Time { get; set; }  // nullable for client-to-server

        public PublicMessage() {}

        public PublicMessage(PublicMessage o)
        {
            User = o.User;
            Content = o.Content;
            IsOwn = o.IsOwn;
            Time = o.Time;
        }
    }
}

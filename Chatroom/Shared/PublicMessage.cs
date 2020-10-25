using System;

namespace Chatroom.Shared
{
    public class PublicMessage
    {
        public User User { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }  // nullable for client-to-server
    }
}

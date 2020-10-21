using System;

namespace Chatroom.Shared
{
    public class PublicMessage
    {
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
    }
}
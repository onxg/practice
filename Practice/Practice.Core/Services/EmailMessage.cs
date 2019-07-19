using System;
namespace Practice.Core.Services
{
    public class EmailMessage
    {
        public string Destination { get; }
        public string Subject { get; }
        public string Body { get; }

        public EmailMessage(string destination, string subject, string body)
        {
            Destination = destination;
            Subject = subject;
            Body = body;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public class Server
    {
        public string Id { get; set; }
        public string Data { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
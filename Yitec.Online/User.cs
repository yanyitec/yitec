using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yitec.Online
{
    public class User
    {
        public string SessionId { get;  set; }

        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Nick { get; set; }
        public int Flags { get; set; }
        public DateTime ActiveTime { get; set; }
    }
}

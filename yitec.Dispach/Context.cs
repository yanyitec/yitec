﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public class Context
    {
        public Context(object raw,IRequest request, IResponse response) {
            this.Raw = raw;
            this.Request = request;
            this.Response = response;
        }
        public object Raw { get; private set; }
        public IRequest Request { get; set; }

        public RouteData RouteData { get; set; }

        public IResponse Response { get; set; }

        public IArguments Arguments { get; set; }
    }
}

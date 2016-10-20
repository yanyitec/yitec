using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    public class Request : IRequest
    {
        public Request(Uri uri) {
            this.Uri = uri;
        }
        public Request(Uri uri,string controllerName,string actionName,HttpMethods method= HttpMethods.All)
        {
            this.Uri = uri;
            this.ActionName = actionName;
            this.ControllerName = controllerName;
            this.Method = method;
        }
        public HttpMethods Method { get; private set; }
        public Uri Uri { get; private set; }
        public string this[string key]
        {
            get {
                string val = null;
                this.Parameters.TryGetValue(key, out val);
                return val;
            }
        }

        public string ActionName
        {
            get;private set;
        }

        public string ControllerName
        {
            get;private set;
        }

        Dictionary<string, string> _Parameters;
        public IDictionary<string, string> Parameters
        {
            get
            {
                if (_Parameters != null) return this._Parameters;
                lock (this) {
                    if (_Parameters != null) return this._Parameters;
                    _Parameters = new Dictionary<string, string>();
                    var s = this.Uri.Query.Split('&');
                    foreach (var s1 in s) {
                        var kv = s1.Split('=');
                        if (kv.Length == 1) _Parameters.Add(kv[0], string.Empty);
                        else _Parameters.Add(kv[0],kv[1]);
                    }
                    return _Parameters;
                }
                
            }
        }

        public string Text
        {
            get
            {
                return Uri.OriginalString;
            }
        }
    }
}

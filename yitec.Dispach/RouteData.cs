using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public class RouteData:IEnumerable<KeyValuePair<string,string>>
    {
        Dictionary<string, string> _Data = new Dictionary<string, string>();
        public RouteData() {
        }
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string this[string key] {
            get {
                string value = null;
                _Data.TryGetValue(key, out value);
                return value;
            }
            set {
                if (_Data.ContainsKey(key)) _Data[key] = value;
                else _Data.Add(key,value);
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Data.GetEnumerator();
        }
    }
}

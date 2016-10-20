using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec
{
    public class ContentType
    {
        public ContentType(string contentType) {
            this.Text = contentType;
        }

        public string Text { get; private set; }

        public override string ToString()
        {
            return this.Text;
        }

        public static implicit operator ContentType(string text) {
            return new ContentType(text);
        }

        public static implicit operator string(ContentType contentType)
        {
            return contentType.Text;
        }


        bool? _IsJson;
        public bool IsJson {
            get {
                if (_IsJson == null) {
                    lock (this) {
                        if (_IsJson == null) {
                            var t = this.Text.ToLower();
                            var ts = t.Split('/');
                            foreach (var x in ts) {
                                if (x == "json") { _IsJson = true; return true; }
                            }
                            _IsJson = false;
                            return false;
                        }
                    }
                }
                return _IsJson.Value;
            }
        }

        public bool IsNameValue { get; set; }
    }
}

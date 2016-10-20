using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Dispaching.Binders
{
    public class DictionaryBinder : IParameterBinder
    {
        public IJsonConverter JsonConverter { get; private set; }
        public object GetValue(string name, Type type, IArguments arguments, IRequest request, Context context)
        {
            if (arguments.ContentType.IsJson) {
                var jsonText = arguments.GetContent();
                return JsonConverter.Parse<Dictionary<string, string>>(jsonText);
            }
            if (arguments.ContentType.IsNameValue) {
                var result = new Dictionary<string, string>();
                foreach (var pair in arguments) {
                    result.Add(pair.Key,pair.Value);
                }
                return result;
            }
            throw new ArgumentException("Cannot Bind value for " + type.FullName);
        }
    }
}

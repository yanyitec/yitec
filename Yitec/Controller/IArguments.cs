using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    /// <summary>
    /// Command.Invoke的参数抽象。
    /// 真实的参数绑定将使用该对象作为输入
    /// </summary>
    public interface IArguments:IEnumerable<KeyValuePair<string,string>>
    {
        bool ContainsKey(string key);

        ContentType ContentType { get; }
        string GetContent();
        int Read(byte[] buffer, int start,int size);
        Task<int> ReadAsync(byte[] buffer, int start, int size);
        string this[string key] { get; set; }
    }
}

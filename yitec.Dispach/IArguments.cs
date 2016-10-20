using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    /// <summary>
    /// Command.Invoke的参数抽象。
    /// 真实的参数绑定将使用该对象作为输入
    /// </summary>
    public interface IArguments
    {
        bool ContainsKey(string key);
        string this[string key] { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    /// <summary>
    /// 命令模式的命令，
    /// 主要是Execute函数
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="request"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        Task<object> ExecuteAsync(IRequest request, IArguments arguments,Context context);
    }
}

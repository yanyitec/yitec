using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    /// <summary>
    /// 命令模式的命令，
    /// 主要是Execute函数
    /// </summary>
    public interface ICommand
    {
        Type ControllerType { get; }

        MethodInfo MethodInfo { get; }
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="request"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        Task<object> ExecuteAsync(object controllerInstance , IArguments arguments, IRequest request, Context context);
    }
}

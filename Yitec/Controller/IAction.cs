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
    public interface IAction
    {
        MethodInfo MethodInfo { get; }

        string CommandText { get; }

        bool HasReturnValue { get; }

        bool IsAsync { get; }
        object ExecuteWithResult(object instance, IArguments arguments, IRequest request, Context context);

        Task<object> ExecuteAsyncWithResult(object instance, IArguments arguments, IRequest request, Context context);

        void ExecuteWithoutResult(object instance, IArguments arguments, IRequest request, Context context);

        Task ExecuteAsyncWithoutResult(object instance, IArguments arguments, IRequest request, Context context);

    }
}

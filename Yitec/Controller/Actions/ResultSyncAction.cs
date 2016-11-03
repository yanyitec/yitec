using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    class ResultSyncAction : IAction
    {
        public ResultSyncAction(MethodInfo methodInfo, Func<object, IArguments, IRequest, Context, object> execution)
        {
            this.MethodInfo = methodInfo;
            this.Execution = execution;
        }

        Func<object, IArguments, IRequest, Context, object> Execution;
        public MethodInfo MethodInfo
        {
            get; private set;
        }

        public string CommandText { get; set; }

        public bool HasReturnValue { get { return true; } }

        public bool IsAsync { get { return true; } }

        public async Task ExecuteAsyncWithoutResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            await Task.Run(()=> Execution(instance, arguments, request, context));
        }

        public async Task<object> ExecuteAsyncWithResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            return await Task.Run<object>(() => Execution(instance, arguments, request, context));
        }

        public void ExecuteWithoutResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            Execution(instance, arguments, request, context);
        }

        public object ExecuteWithResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            return Execution(instance, arguments, request, context);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    class NoresultSyncAction : IAction
    {
        public NoresultSyncAction(MethodInfo methodInfo, Action<object, IArguments, IRequest, Context> execution)
        {
            this.MethodInfo = methodInfo;
            this.Execution = execution;
        }

        Action<object, IArguments, IRequest, Context> Execution;
        public MethodInfo MethodInfo
        {
            get; private set;
        }

        public string CommandText { get; set; }

        public bool HasReturnValue { get { return false; } }

        public bool IsAsync { get { return false; } }

        public async Task ExecuteAsyncWithoutResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            await Task.Run(() => Execution(instance, arguments, request, context));
        }

        public async Task<object> ExecuteAsyncWithResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            await Task.Run(() => Execution(instance, arguments, request, context));
            return null;
        }

        public void ExecuteWithoutResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            Execution(instance, arguments, request, context);
        }

        public object ExecuteWithResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            Execution(instance, arguments, request, context);
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller.Actions
{
    class ResultAsyncAction : IAction
    {
        public ResultAsyncAction(MethodInfo methodInfo, Func<object, IArguments, IRequest, Context, Task<object>> execution) {
            this.MethodInfo = methodInfo;
            this.Execution = execution;
        }

        Func<object, IArguments, IRequest, Context, Task<object>> Execution;
        public MethodInfo MethodInfo
        {
            get;private set;
        }

        public string CommandText { get; set; }

        public bool HasReturnValue { get { return true; } }

        public bool IsAsync { get { return true; } }

        public async Task ExecuteAsyncWithoutResult(object instance, IArguments arguments, IRequest request, Context context)
        {
             await Execution(instance,arguments,request,context);
        }

        public async Task<object> ExecuteAsyncWithResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            return await Execution(instance, arguments, request, context);
        }

        public void ExecuteWithoutResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            var waitor = Execution(instance, arguments, request, context);
            waitor.Wait();
        }

        public object ExecuteWithResult(object instance, IArguments arguments, IRequest request, Context context)
        {
            var waitor = Execution(instance, arguments, request, context);
            waitor.Wait();
            return waitor.Result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public abstract class Dispacher : IDispacher
    {
        public Dispacher(ICommandLoader commandLoader) {
            this.CommandLoader = commandLoader;
        }
        ICommandFactory _Commands;
        public ICommandFactory Commands {
            get {
                if (_Commands != null) return _Commands;
                lock (this) {
                    if (_Commands == null) return _Commands;
                    return _Commands = this.CommandLoader.Load();
                }
            }
        }

        public ICommandLoader CommandLoader { get;private set; }
        public async Task<object> DispachAsync(object rawContext)
        {
            //创建上下文
            var context = this.CreateContext(rawContext);
            //路由
            var routeData = await this.RouteAsync(context.Request,context);
            context.RouteData = routeData;
            //创建绑定参数
            var cmdArgs = await this.CreateArgumentsAsync(context.Request,context.RouteData, context);

            //找出命令
            var cmd = this.Commands.FindCommand(context.RouteData,context.Request.Method,context);
            return await cmd.ExecuteAsync(context.Request,context.Arguments,context);
        }

        protected virtual Context CreateContext(object rawContext) {
            
            var request = CreateRequest(rawContext);
            var response = CreateResponse(rawContext);
            var context = new Context(rawContext,request,response);
            return context;
        }
        protected abstract IRequest CreateRequest(object rawContext);
        protected abstract IResponse CreateResponse(object rawContext);
        

        protected abstract Task<RouteData> RouteAsync(IRequest request, Context context);

        protected abstract Task<IArguments> CreateArgumentsAsync(IRequest request,RouteData routeData, Context context);

        public event Action<IRequest, Exception> OnException;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    public abstract class Dispacher : IDispacher
    {
        public Dispacher(ICommandFactory commandFactory, IControllerFactoryFactory controllerFactory) {
            this.CommandFactory = commandFactory;
            //this.BinderFactory = binderFactory;
        }

        public ICommandFactory CommandFactory
        {
            get; protected set;
        }

        public IControllerFactoryFactory ControllerFactory {
            get;protected set;
        }

        

        

        protected virtual Context CreateContext(object rawContext)
        {

            var request = CreateRequest(rawContext);
            var response = CreateResponse(rawContext);
            var context = new Context(rawContext, request, response);
            return context;
        }
        protected abstract IRequest CreateRequest(object rawContext);
        protected abstract IResponse CreateResponse(object rawContext);

        protected abstract Task<RouteData> RouteAsync(IRequest request, Context context);
        protected abstract Task<IArguments> CreateArgumentsAsync(IRequest request, RouteData routeData, Context context);

        protected virtual IAction GetOrCreateCommand(RouteData routeData,HttpMethods method,Context context) {
            return this.CommandFactory.GetOrCreateCommand(routeData,method,context);
        }

        protected virtual object GetOrCreateController(IAction cmd,RouteData routeData,Context context) {
            return null;
            //return this.ControllerFactory(cmd, routeData, context);
        }
        
    

        
        public async Task<object> DispachAsync(object rawContext)
        {
            //创建上下文
            var context = this.CreateContext(rawContext);
            //路由
            var routeData = await this.RouteAsync(context.Request,context);
            context.RouteData = routeData;
            
            //找出命令
            var cmd = context.Command = this.CommandFactory.GetOrCreateCommand(context.RouteData,context.Request.Method,context);
            //创建执行主体
            var controllerInstance = context.ControllerInstance = this.GetOrCreateController(cmd, routeData, context);

            //创建参数
            var args = context.Arguments = await this.CreateArgumentsAsync(context.Request, context.RouteData, context);
            return null;
            //执行并返回结果
            //return context.Result = await context.Command.ExecuteAsync(context.ControllerInstance , context.Arguments,context.Request, context);
        }

        
        public event Action<IRequest, Exception> OnException;
    }
}

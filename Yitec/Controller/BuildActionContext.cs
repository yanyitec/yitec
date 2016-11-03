using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    /// <summary>
    /// 构建命令所需要的上下文信息
    /// </summary>
    public class BuildActionContext
    {
        public BuildActionContext(MethodInfo actionMethodInfo, Func<ParameterInfo , string , HttpMethods , Func<ParameterInfo, IArguments, IRequest, Context, object>> binderFactory=null) {
            this.ActionMethodInfo = actionMethodInfo;
            ArgumentsExpression = Expression.Parameter(typeof(IArguments),"_ARGUMENTS");
            RequestExpression = Expression.Parameter(typeof(IRequest),"_REQUEST");
            ContextExpression = Expression.Parameter(typeof(Context),"_CONTEXT");
            this.CodeExpressions = new List<Expression>();
            this.LocalVarExpressions = new List<ParameterExpression>();
            this.ControllerInstanceExpression = Expression.Parameter(typeof(object),"_INSTANCE");
            this.BinderFactory = binderFactory;
        }
        protected BuildActionContext(BuildActionContext context) {
            this.ActionMethodInfo = context.ActionMethodInfo;
            
            this.ArgumentsExpression = context.ArgumentsExpression;
            this.RequestExpression = context.RequestExpression;
            this.ContextExpression = context.ContextExpression;
            this.CodeExpressions = context.CodeExpressions;
            this.LocalVarExpressions = context.LocalVarExpressions;

            this.BinderFactory = context.BinderFactory;
        }
        public string CommandText { get; private set; }

        public HttpMethods HttpMethod { get; private set; }

        public ParameterExpression ControllerInstanceExpression { get; private set; }

        public IList<Expression> CodeExpressions { get; private set; }
        public IList<ParameterExpression> LocalVarExpressions { get; private set; }
        public MethodInfo ActionMethodInfo { get; private set; }
        public ParameterExpression ArgumentsExpression { get; private set; }
        
        public ParameterExpression RequestExpression { get; private set; }
        
        public ParameterExpression ContextExpression { get; private set; }

        public Func<ParameterInfo, string, HttpMethods, Func<ParameterInfo, IArguments, IRequest, Context, object>> BinderFactory { get; private set; }
    }
}

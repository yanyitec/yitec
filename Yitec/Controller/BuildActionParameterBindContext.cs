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
    /// 构建参数绑定所需要的上下文信息
    /// </summary>
    public class BuildActionParameterBindContext : BuildActionContext
    {
        MethodInfo ArgumentGetItemMethodInfo = typeof(IArguments).GetMethod("get_Item");

        public BuildActionParameterBindContext(BuildActionContext context, ParameterInfo par):base(context) {
            this.ParameterInfo = par;
            this.NullableChecker = new NullableTypes(par.ParameterType);
            this.LocalExpression = Expression.Parameter(this.ParameterType,par.Name);
            this.GetItemExpression = Expression.Call(this.ArgumentsExpression, ArgumentGetItemMethodInfo, Expression.Constant(par.Name));
        }
        
        public Expression GetItemExpression { get; private set; }
        public ParameterExpression LocalExpression { get; private set; }

        public ParameterInfo ParameterInfo { get; private set; }

        public Type ParameterType { get { return this.ParameterInfo.ParameterType; } }

        public NullableTypes NullableChecker { get; private set; }
    }
}

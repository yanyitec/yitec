using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public class CommandBuilder : ICommandBuilder
    {
        static MethodInfo GetOrCreateFactoryMethodInfo = typeof(IControllerFactory).GetRuntimeMethod("GetOrCreateController",new Type[] { typeof(Type)});
        public Func<IRequest,IArguments, Context,object> BuildExecuteFunc(MethodInfo methodInfo, IControllerFactory controllerFactory, IBinderFactory binderFactory)
        {
            var parameters = methodInfo.GetParameters();
            var localVars = new List<ParameterExpression>();
            var typedVars = new Dictionary<int, ParameterExpression>();
            var paramVars = new List< ParameterExpression>();
            var codeExprs = new List<Expression>();
            foreach (ParameterInfo paramInfo in parameters) {
               // BuildAssign
            }

            var getControllerInstance = Expression.Call(Expression.Constant(controllerFactory), GetOrCreateFactoryMethodInfo);
            var callAction = Expression.Call(getControllerInstance, methodInfo, paramVars);

            
            var labelTarget = Expression.Label(typeof(object));
            var returnValueExpr = Expression.Convert(callAction, typeof(object));
            var retExpr = Expression.Return(labelTarget, returnValueExpr);
            var labelExpr = Expression.Label(labelTarget, Expression.Constant(null,typeof(object)));
            codeExprs.Add(retExpr);
            codeExprs.Add(labelExpr);

            Expression block = Expression.Block(localVars,codeExprs);
            if (block.CanReduce)
            {
                block = block.ReduceAndCheck();
            }
            var lamda = Expression.Lambda<Func<IRequest, IArguments, Context,object>>(block);

            var result = lamda.Compile();
            return result;


        }

        static void BuildAssign(ParameterInfo parameterInfo, List<ParameterExpression> localVars, List<Expression> codeExprs) {

        }
    }
}

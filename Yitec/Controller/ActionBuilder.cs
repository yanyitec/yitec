using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yitec.Compiling;

namespace Yitec.Controller
{
    public class ActionBuilder 
    {
        public ActionBuilder() {
            //this.defaultBinders = InitDefaultBinders();
        }

        //readonly static MethodInfo ParseNullableEnumMethodInfo = 
        public Func<ParameterInfo, string, HttpMethods, Func<ParameterInfo, IArguments, IRequest, Context, object>> BinderFactory { get; private set; }

        
        //Task<object> ExecuteAsync(object controllerInstance , IArguments arguments, IRequest request, Context context);
        ParameterExpression ControllerInstanceParameter = Expression.Parameter(typeof(object), "controllerInstance");
        ParameterExpression ArgumentsParameter = Expression.Parameter(typeof(IArguments),"arguments");
        MethodInfo ParseNullableEnumMethodInfo = typeof(NullableTypes).GetMethod("ParseEnum");

        public IList<IAction> Generate(IList<Type> controllerTypes)
        {
            List<IAction> result = new List<IAction>();
            foreach (var controllerType in controllerTypes) {
                var methods = controllerType.GetMethods();
                foreach (var method in methods) {
                    //跳过非公开方法
                    if (!method.IsPublic) continue;
                    //标记了NoAction的方法跳过
                    var noAction = method.GetCustomAttribute<NonActionAttribute>();
                    if (noAction != null) continue;
                    var buildContext = new BuildActionContext(method, this.BinderFactory);
                    var action = BuildAction(buildContext);
                    result.Add(action);
                }
            }
            return result;
        }

        public IAction BuildAction(BuildActionContext cmdContext)
        {
            
            var parameters = cmdContext.ActionMethodInfo.GetParameters();
            var argExprs = new List<ParameterExpression>();
            foreach (var par in parameters) {
                var parContext = new BuildActionParameterBindContext(cmdContext,par);
                cmdContext.LocalVarExpressions.Add(parContext.LocalExpression);
                argExprs.Add(parContext.LocalExpression);
                // _ARGUMENTS["parameter_name"]
                if (parContext.NullableChecker.IsNullable)
                {
                    var assignExpr = GenerateNullableBindCodes(parContext);
                    parContext.CodeExpressions.Add(assignExpr);
                    var hasValueExpr = Expression.Call(parContext.LocalExpression,
                        parContext.ParameterType.GetMethod("get_HasValue")
                        );
                    var ckExpr = Expression.Condition(Expression.Not(hasValueExpr)
                        ,GenerateInitCodes(parContext),null
                        );
                    parContext.CodeExpressions.Add(ckExpr);
                } 
                else {
                    GenerateNonullableBindCodes(parContext);
                }
            }
            var instanceExpr = cmdContext.ControllerInstanceExpression;
            var instanceConvertExpr = Expression.Convert(instanceExpr,cmdContext.ActionMethodInfo.DeclaringType);
            var callExpr = Expression.Call(instanceConvertExpr, cmdContext.ActionMethodInfo, argExprs);
            cmdContext.CodeExpressions.Add(callExpr);

            return GenerateAction(cmdContext,callExpr);
            
        }

        IAction GenerateAction(BuildActionContext cmdContext,MethodCallExpression callExpr) {
            var retType = cmdContext.ActionMethodInfo.ReturnType;
            var labelTarget = Expression.Label(retType);
            
            if (cmdContext.ActionMethodInfo.ReturnType.FullName.StartsWith("System.Task"))
            {

            }
            else {
                var retExpr = Expression.Return(labelTarget, callExpr);
                var labelExpr = retType==typeof(void)
                    ? Expression.Label(labelTarget, Expression.Constant(null,typeof(void)))
                    : Expression.Label(labelTarget, Expression.Constant(null,retType));

                cmdContext.CodeExpressions.Add(labelExpr);
                Expression blockExpr = Expression.Block(cmdContext.LocalVarExpressions, cmdContext.CodeExpressions);
                if (blockExpr.CanReduce) blockExpr.ReduceAndCheck();
                if (cmdContext.ActionMethodInfo.ReturnType == typeof(void))
                {
                    var lamda = Expression.Lambda<Action<object, IArguments, IRequest, Context>>(blockExpr, cmdContext.ControllerInstanceExpression, cmdContext.ArgumentsExpression, cmdContext.RequestExpression, cmdContext.ContextExpression);
                    var func = lamda.Compile();
                    return new NoresultSyncAction(cmdContext.ActionMethodInfo, func);
                }
                else {
                    var lamda = Expression.Lambda<Func<object, IArguments, IRequest, Context,object>>(blockExpr, cmdContext.ControllerInstanceExpression, cmdContext.ArgumentsExpression, cmdContext.RequestExpression, cmdContext.ContextExpression);
                    var func = lamda.Compile();
                    return new ResultSyncAction(cmdContext.ActionMethodInfo, func);
                }
                
            }
            return null;
            //var returnValueExpr = Expression.Convert(resultExpr, typeof(object));

        }

        Expression GenerateInitCodes(BuildActionParameterBindContext parContext) {
            Expression initParExpr = null;

            if (parContext.ParameterInfo.HasDefaultValue)
            {
                initParExpr = Expression.Assign(parContext.LocalExpression, Expression.Constant(parContext.ParameterInfo.DefaultValue, parContext.ParameterType));
            }
            else
            {
                var defaultValue = ValueTypes.GetDefaultValue(parContext.ParameterType);
                var defaultValueExpr = Expression.Convert(Expression.Constant(defaultValue, typeof(object)), parContext.ParameterType);
                initParExpr = Expression.Assign(parContext.LocalExpression, defaultValueExpr);
            }
            return initParExpr;
        }

        Expression GenerateNullableBindCodes(BuildActionParameterBindContext parContext) {
           
            if (parContext.NullableChecker.ActualType.IsEnum)
            {
                var method = ParseNullableEnumMethodInfo.MakeGenericMethod(parContext.NullableChecker.ActualType);
                // ParseEnum<T>(_ARGUMENTS['par_name']);
                var parseExpr = Expression.Call(null, method, parContext.GetItemExpression);
                return Expression.Assign(parContext.LocalExpression, parseExpr);
            }
            else
            {
                var binder = NullableTypes.GetParser(parContext.NullableChecker);
                if (binder == null) throw new Exception(parContext.NullableChecker.ActualType.FullName + " is not supported to bind as nullable<>");
                var getValueExpr = Expression.Invoke(Expression.Constant(binder), parContext.GetItemExpression);
                var convertExpr = Expression.Convert(getValueExpr, parContext.ParameterType);
                // local = (T?)NullableTypes.GetParser(typeof(T))(_ARGUMENTS["parameter_name"]);
                return Expression.Assign(parContext.LocalExpression, convertExpr);
            }
        }
        void GenerateNonullableBindCodes(BuildActionParameterBindContext parContext) {
           

            Func<ParameterInfo, IArguments, IRequest, Context, object> binder =
                parContext.BinderFactory == null ?null: parContext.BinderFactory(parContext.ParameterInfo,parContext.CommandText,parContext.HttpMethod);
            if (binder == null)
            {
                GenerateDefaultNonullableBindCodes(parContext);
            }
            else {
                var tmpVarExp = Expression.Parameter(typeof(object), "TMP_" + parContext.ParameterInfo.Name);
                parContext.LocalVarExpressions.Add(tmpVarExp);

                var getValueExpr = Expression.Invoke(Expression.Constant(binder), Expression.Constant(parContext.ParameterInfo),parContext.ArgumentsExpression,parContext.RequestExpression,parContext.ContextExpression);
                var tmpAssignExpr = Expression.Assign(tmpVarExp, getValueExpr);
                var initExpr = GenerateInitCodes(parContext);
                var convertExpr = Expression.Convert(getValueExpr, parContext.ParameterType);
                var code = Expression.Condition(Expression.Equal(tmpVarExp, Expression.Constant(null, typeof(object)))
                    , initExpr
                    , Expression.Assign(parContext.LocalExpression, convertExpr)
                    );
                parContext.CodeExpressions.Add(code);
            }

            

        }

        void GenerateDefaultNonullableBindCodes(BuildActionParameterBindContext parContext) {
            var binder_m = ValueTypes.GetParser(parContext.ParameterType);
            var tmpVarExp = Expression.Parameter(typeof(object), "TMP_" + parContext.ParameterInfo.Name);
            parContext.LocalVarExpressions.Add(tmpVarExp);
            if (binder_m == null)
            {
                var initExpr = GenerateInitCodes(parContext);
                parContext.CodeExpressions.Add(initExpr);
                //binder = this.ParameterBinderFactory.GetBinder(par, cmdText,)
            }
            else
            {
                var getValueExpr = Expression.Invoke(Expression.Constant(binder_m), parContext.GetItemExpression);
                var tmpAssignExpr = Expression.Assign(tmpVarExp, getValueExpr);
                var initExpr = GenerateInitCodes(parContext);
                var convertExpr = Expression.Convert(getValueExpr, parContext.ParameterType);
                var code = Expression.Condition(Expression.Equal(tmpVarExp, Expression.Constant(null, typeof(object)))
                    , initExpr
                    , Expression.Assign(parContext.LocalExpression, convertExpr)
                    );
                parContext.CodeExpressions.Add(code);
            }
        }

        
    }
}

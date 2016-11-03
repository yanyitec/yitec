using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yitec.Compiling;

namespace Yitec.Controller.Actions
{
    public class ActionGenerator 
    {
        public ActionGenerator() {
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
                    var action = Generate(method);
                    if(action!=null)result.Add(action);
                }
            }
            return result;
        }

        public IAction Generate(MethodInfo method) {
            if (!method.IsPublic) return null;
            //标记了NoAction的方法跳过
            var noAction = method.GetCustomAttribute<NonActionAttribute>();
            if (noAction != null) return null;
            var buildContext = new ActionGenerationContext(method, this.BinderFactory);
            return Generate(buildContext);
        }

        /// <summary>
        /// 根据上下文，生成一个Action
        /// </summary>
        /// <param name="cmdContext"></param>
        /// <returns></returns>
        IAction Generate(ActionGenerationContext cmdContext)
        {
            //获取所有参数
            var parameters = cmdContext.ActionMethodInfo.GetParameters();
            //函数调用的参数列表
            var argExprs = new List<ParameterExpression>();
            //循环所有参数
            foreach (var par in parameters)
            {
                #region 生成为每个参数赋值的表达式
                //构建参数上下文
                var parContext = new ActionGenerationParameterBindContext(cmdContext,par);
                //把参数变量添加到变量表中
                cmdContext.LocalVarExpressions.Add(parContext.LocalExpression);
                argExprs.Add(parContext.LocalExpression);
                
                //生成绑定参数值的表达式
                if (parContext.NullableChecker.IsNullable)
                {
                    GenerateNullableBindCodes(parContext);
                } 
                else {
                    GenerateNonullableBindCodes(parContext);
                }
                #endregion
            }
            #region 生成函数调用 controllerInstance.Insert(username,password);
            var instanceExpr = cmdContext.ControllerInstanceExpression;
            var instanceConvertExpr = Expression.Convert(instanceExpr,cmdContext.ActionMethodInfo.DeclaringType);
            var callExpr = Expression.Call(instanceConvertExpr, cmdContext.ActionMethodInfo, argExprs);
            cmdContext.CodeExpressions.Add(callExpr);
            #endregion

            //根据Action Method的不同返回值类型，生成不同的Action
            return GenerateReturn(cmdContext,callExpr);
            
        }

        /// <summary>
        /// 根据Action Method的不同返回值类型，生成不同的Action
        /// </summary>
        /// <param name="cmdContext"></param>
        /// <param name="callExpr"></param>
        /// <returns></returns>
        IAction GenerateReturn(ActionGenerationContext cmdContext, MethodCallExpression callExpr)
        {
            var retType = cmdContext.ActionMethodInfo.ReturnType;
            var labelTarget = Expression.Label(retType);
            
            if (cmdContext.ActionMethodInfo.ReturnType.FullName.StartsWith("System.Task"))
            {

            }
            else {
                var retExpr = Expression.Return(labelTarget, callExpr);
                LabelExpression labelExpr = Expression.Label(labelTarget);
                
                
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
        /// <summary>
        /// 生成初始化代码 gender = Genders.Secret;
        /// </summary>
        /// <param name="parContext"></param>
        /// <returns></returns>
        Expression GenerateInitCodes(ActionGenerationParameterBindContext parContext) {
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

        /// <summary>
        /// 给Nullable<>的参数变量赋值
        /// </summary>
        /// <param name="parContext"></param>
        void GenerateNullableBindCodes(ActionGenerationParameterBindContext parContext) {
            Expression assignExpr = null;
            if (parContext.NullableChecker.ActualType.IsEnum)
            {
                var method = ParseNullableEnumMethodInfo.MakeGenericMethod(parContext.NullableChecker.ActualType);
                // ParseEnum<T>(_ARGUMENTS['par_name']);
                var parseExpr = Expression.Call(null, method, parContext.GetItemExpression);
                assignExpr = Expression.Assign(parContext.LocalExpression, parseExpr);
            }
            else
            {
                var binder = NullableTypes.GetParser(parContext.NullableChecker);
                if (binder == null) throw new Exception(parContext.NullableChecker.ActualType.FullName + " is not supported to bind as nullable<>");
                var getValueExpr = Expression.Invoke(Expression.Constant(binder), parContext.GetItemExpression);
                var convertExpr = Expression.Convert(getValueExpr, parContext.ParameterType);
                // local = (T?)NullableTypes.GetParser(typeof(T))(_ARGUMENTS["parameter_name"]);
                assignExpr = Expression.Assign(parContext.LocalExpression, convertExpr);
            }

            parContext.CodeExpressions.Add(assignExpr);
            var hasValueExpr = Expression.Call(parContext.LocalExpression,
                parContext.ParameterType.GetMethod("get_HasValue")
                );
            var ckExpr = Expression.IfThen(Expression.Not(hasValueExpr)
                , GenerateInitCodes(parContext)
                );
            parContext.CodeExpressions.Add(ckExpr);
                
        }
        /// <summary>
        /// 给不是 Nullable<>的参数变量赋值
        /// </summary>
        /// <param name="parContext"></param>
        void GenerateNonullableBindCodes(ActionGenerationParameterBindContext parContext) {
           

            Func<ParameterInfo, IArguments, IRequest, Context, object> binder =
                parContext.BinderFactory == null ?null: parContext.BinderFactory(parContext.ParameterInfo,parContext.CommandText,parContext.HttpMethod);
            if (binder == null)
            {
                GenerateDefaultNonullableBindCodes(parContext);
            }
            else {
                var tmpVarExp = Expression.Parameter(typeof(object), "___EXTEmPORanEous_" + parContext.ParameterInfo.Name);
                parContext.LocalVarExpressions.Add(tmpVarExp);

                var getValueExpr = Expression.Invoke(Expression.Constant(binder), Expression.Constant(parContext.ParameterInfo),parContext.ArgumentsExpression,parContext.RequestExpression,parContext.ContextExpression);
                var tmpAssignExpr = Expression.Assign(tmpVarExp, getValueExpr);
                var initExpr = GenerateInitCodes(parContext);
                var convertExpr = Expression.Convert(getValueExpr, parContext.ParameterType);
                var code = Expression.IfThenElse(Expression.Equal(tmpVarExp, Expression.Constant(null, typeof(object)))
                    , initExpr
                    , Expression.Assign(parContext.LocalExpression, convertExpr)
                    );
                parContext.CodeExpressions.Add(code);
            }
        }

        

        void GenerateDefaultNonullableBindCodes(ActionGenerationParameterBindContext parContext) {

            if (parContext.ParameterType == typeof(string)) {
                this.GenerateStringBindCodes(parContext);
                return;
            }

            var vparser = ValueTypes.GetParser(parContext.ParameterType);

            if (vparser == null)
            {
                var initExpr = GenerateInitCodes(parContext);
                parContext.CodeExpressions.Add(initExpr);
                //binder = this.ParameterBinderFactory.GetBinder(par, cmdText,)
            }
            else
            {
                GenerateValueTypeBindCodes(parContext, vparser);
            }
        }
        void GenerateStringBindCodes(ActionGenerationParameterBindContext parContext)
        {
            //申请一个临时变量
            //string ___EXTEmPORanEous_username;
            var tmpVarExp = Expression.Parameter(typeof(string), "___EXTEmPORanEous_" + parContext.ParameterInfo.Name);  
            parContext.LocalVarExpressions.Add(tmpVarExp);
            //从arguments中拿到值
            //___EXTEmPORanEous_username = arguments["username"];
            var assignExpr = Expression.Assign(tmpVarExp, parContext.GetItemExpression);
            parContext.CodeExpressions.Add(assignExpr);
            //如果是null，就赋默认值，否者就赋临时变量的值
            //username = ___EXTEmPORanEous_username==null?string.Empty:___EXTEmPORanEous_username
            var condExpr = Expression.IfThenElse(
                Expression.Equal(tmpVarExp, Expression.Constant(null, typeof(string)))
                , GenerateInitCodes(parContext)
                , Expression.Assign(parContext.LocalExpression, tmpVarExp)
                );
            parContext.CodeExpressions.Add(condExpr);
        }

        void GenerateValueTypeBindCodes(ActionGenerationParameterBindContext parContext, Func<string, object> parser) {
            //定义一个临时变量
            var tmpVarExp = Expression.Parameter(typeof(object), "___EXTEmPORanEous_" + parContext.ParameterInfo.Name);
            parContext.LocalVarExpressions.Add(tmpVarExp);

            //获取parse的值到临时变量中
            var getValueExpr = Expression.Invoke(Expression.Constant(parser), parContext.GetItemExpression);
            var tmpAssignExpr = Expression.Assign(tmpVarExp, getValueExpr);
            parContext.CodeExpressions.Add(tmpAssignExpr);

            //if(tmp_username==null) username = "yiy";else username = (string)temp_usernmae;
            var initExpr = GenerateInitCodes(parContext);
            var convertExpr = Expression.Convert(getValueExpr, parContext.ParameterType);
            var code = Expression.IfThenElse(Expression.Equal(tmpVarExp, Expression.Constant(null, typeof(object)))
                , initExpr
                , Expression.Assign(parContext.LocalExpression, convertExpr)
                );
            parContext.CodeExpressions.Add(code);
        }
    }
}

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
    public class CommandBuilder 
    {
        public CommandBuilder() {
            this.defaultBinders = InitDefaultBinders();
        } 
        public IParameterBinderTypeFactory ParameterBinderFactory { get; private set; }

        Dictionary<string, string> defaultBinders = new Dictionary<string, string>();

        public IList<ICommand> Generate(IList<Type> controllerTypes) {
            return null;
        }

        Project DynamicProject { get; set; }

        protected static void GenerateAllCodes(Project dynamicProject,IList<Type> controllerTypes, IParameterBinderTypeFactory binderFactory, Dictionary<string, string> defaultBinders) {
            foreach (var controllerType in controllerTypes) {
                GenerateControllerCodes(dynamicProject,controllerType,binderFactory,defaultBinders);
            }
        }

        static void GenerateControllerCodes(Project dynamicProject, Type controllerType, IParameterBinderTypeFactory binderFactory, Dictionary<string, string> defaultBinders) {
            
            var controllerName = controllerType.Name;
            if (controllerName.EndsWith("Controller")) controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
            var methodInfos = controllerType.GetMethods();
            var count = 0;
            foreach (var methodInfo in methodInfos) {
                //排除标记为NoAction的方法
                var nonActionAttr = methodInfo.GetCustomAttribute<NonActionAttribute>();
                if (nonActionAttr != null) continue;
                //获取actionName与controllerName
                var actionName = methodInfo.Name;
                var actionAttr = methodInfo.GetCustomAttribute<ActionAttribute>();
                var cmdText = string.Empty;
                if (actionAttr != null)
                {
                    cmdText = actionAttr.CommandText;
                }
                if (string.IsNullOrEmpty(cmdText)) {
                    cmdText = controllerName + "/" + actionName;
                }
                GenerateActionCodes(dynamicProject , cmdText,HttpMethods.All,methodInfo,binderFactory);
                count++;
            }
            if (count != 0) {
                dynamicProject.WithReference(controllerType);
            }
        }

        static void GenerateActionCodes(Project dynamicProject, string cmdText, HttpMethods methods, MethodInfo methodInfo, IParameterBinderTypeFactory binderFactory, Dictionary<string, string> defaultBinders=null) {
            var binderMemberCodes = string.Empty;
            var paramInfos = methodInfo.GetParameters();
            //本地变量定义代码
            var localDefineCodes = string.Empty;
            //本地变量赋值
            var localAssignCodes = string.Empty;
            #region 参数列表与参数赋值
            var argListCodes = string.Empty;
            foreach (var paramInfo in paramInfos) {
                
                //定义本地变量，并赋予初值
                localDefineCodes += "\t\t\tvar " + paramInfo.Name + " = " + GetDefaultValueText(paramInfo.ParameterType) + "\r\n";

                #region 本地变量赋值
                var binderType = binderFactory.GetBinderType(paramInfo, cmdText, methods);
                if (binderType == null) {
                    BuildUndefinedBinder(paramInfo,defaultBinders, ref localAssignCodes,ref binderMemberCodes);
                } else { }
                #endregion

                //参数调用列表
                if (!string.IsNullOrEmpty(argListCodes)) argListCodes += ",";
                argListCodes += paramInfo.Name;
                #region 参数类型的assembly添加到生成的项目中
                var ptInfo = new NullableType(paramInfo.ParameterType);
                if (ptInfo.IsNullable)
                {
                    dynamicProject.WithReference(ptInfo.ActualType);
                }
                    
                dynamicProject.WithReference(ptInfo.OrignalType);
                #endregion
                
            }
            #endregion
            #region 返回类型添加到项目引用中
            var rtInfo = new NullableType(methodInfo.ReturnType);
            dynamicProject.WithReference(rtInfo.OrignalType);
            if (rtInfo.IsNullable) dynamicProject.WithReference(rtInfo.ActualType);
            #endregion

            #region 代码添加到项目中
            var classCode = GenCommandClassCode(dynamicProject,cmdText,methods,methodInfo,binderMemberCodes,localDefineCodes,localAssignCodes,argListCodes);
            dynamicProject.AddTextSource(classCode);
            #endregion

        }

        static string GenCommandClassCode(Project dynamicProject, string cmdText, HttpMethods methods, MethodInfo methodInfo, string binderMemberCodes,string localDefineCodes,string localAssignCodes , string argListCodes) {
            var code = "using System;using System.Collections.Generic;using System.Threading.Tasks;";
            code += "namespace " + dynamicProject.Name + "{\nclass " + methodInfo.ReflectedType.Name + "_" + methodInfo.Name + "_Command{";
            code += binderMemberCodes;
            code += "\t\t\tobject Execute(object __YITEC_INSTANCE , IArguments __YITEC_ARGUMENTS, IRequest __YITEC_REQUEST, Context IRequest __YITEC_CONTEXT){";
            code += localDefineCodes;
            code += localAssignCodes;
            code += "\t\t\treturn __YITEC_INSTANCE." + methodInfo.Name + "(" + argListCodes + ");\n";
            code += "\t\t}\t}//end class\n}}";
            return code;
        }
        static string GetDefaultValueText(Type type) {
            if (
                type == typeof(byte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(decimal)
                || type == typeof(float)
                || type == typeof(double)
                ) {
                return "0";
            }
            if (type == typeof(Guid)) {
                return "System.Guid.Empty";
            }
            if (type == typeof(DateTime)) {
                return "System.DateTime.MinValue";
            }
            if (type.IsEnum) {
                return "default(" + type.FullName + ")";
            }
            return "null";
        }
        static void BuildDefinedBinder(ParameterInfo paramInfo, ref string assignCodes) {
            var typeInfo = new NullableType(paramInfo.ParameterType);

        }

        static void BuildUndefinedBinder(ParameterInfo paramInfo, Dictionary<string, string> defaultBinders, ref string codes,ref string binderMemberCodes)
        {
           
            string binderCode = null;
            if (!defaultBinders.TryGetValue(paramInfo.ParameterType.FullName, out binderCode)) {
                var ptInfo = new NullableType(paramInfo.ParameterType);
                if (ptInfo.ActualType.IsEnum)
                {
                    binderCode = BuildEnumBinder(ptInfo.ActualType);
                    defaultBinders.Add(ptInfo.OrignalType.FullName, binderCode);
                }
                else {
                    binderCode = BuildObjectBinder(ptInfo.OrignalType,ref binderMemberCodes);
                }
                
            }
            var varname = paramInfo.Name;
            var varkey = paramInfo.Name;
            codes += binderCode.Replace("@varname", varname).Replace("@varkey",varkey);
            

        }

        static Dictionary<string, string> InitDefaultBinders() {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var numText = "\t\t\tTYPE_DEF @varname_tmp=0;if(TYPE_DEF.TryParse(_YITEC_INPUT_ARGUMENTS['@varkey'],out @varname_tmp))@varname=@varname_tmp;\n";
            
            result.Add(typeof(byte).FullName, numText.Replace("TYPE_DEF", "byte"));
            result.Add(typeof(short).FullName, numText.Replace("TYPE_DEF", "short"));
            result.Add(typeof(ushort).FullName, numText.Replace("TYPE_DEF", "ushort"));
            result.Add(typeof(int).FullName, numText.Replace("TYPE_DEF", "int"));
            result.Add(typeof(uint).FullName, numText.Replace("TYPE_DEF", "uint"));
            result.Add(typeof(long).FullName, numText.Replace("TYPE_DEF", "long"));
            result.Add(typeof(ulong).FullName, numText.Replace("TYPE_DEF", "ulong"));
            result.Add(typeof(decimal).FullName, numText.Replace("TYPE_DEF", "decimal"));
            result.Add(typeof(float).FullName, numText.Replace("TYPE_DEF", "float"));
            result.Add(typeof(double).FullName, numText.Replace("TYPE_DEF", "double"));
            result.Add(typeof(DateTime).FullName, numText.Replace("TYPE_DEF", "DateTime"));
            result.Add(typeof(Guid).FullName, numText.Replace("TYPE_DEF", "Guid"));

            result.Add(typeof(byte?).FullName, numText.Replace("TYPE_DEF", "byte"));
            result.Add(typeof(short?).FullName, numText.Replace("TYPE_DEF", "short"));
            result.Add(typeof(ushort?).FullName, numText.Replace("TYPE_DEF", "ushort"));
            result.Add(typeof(int?).FullName, numText.Replace("TYPE_DEF", "int"));
            result.Add(typeof(uint?).FullName, numText.Replace("TYPE_DEF", "uint"));
            result.Add(typeof(long?).FullName, numText.Replace("TYPE_DEF", "long"));
            result.Add(typeof(ulong?).FullName, numText.Replace("TYPE_DEF", "ulong"));
            result.Add(typeof(decimal?).FullName, numText.Replace("TYPE_DEF", "decimal"));
            result.Add(typeof(float?).FullName, numText.Replace("TYPE_DEF", "float"));
            result.Add(typeof(double?).FullName, numText.Replace("TYPE_DEF", "double"));
            result.Add(typeof(DateTime?).FullName, numText.Replace("TYPE_DEF", "DateTime"));
            result.Add(typeof(Guid?).FullName, numText.Replace("TYPE_DEF", "Guid"));

            result.Add(typeof(string).FullName, "\t\t\t@varname = _YITEC_INPUT_ARGUMENTS['@varkey'];\n");
            result.Add(typeof(bool).FullName,BuildBooleanBinder());
            return result;
        }

        static string BuildBooleanBinder()
        {
            var text = @"\t\t\tint @varname_tmp=0;string @varname_txt=_YITEC_INPUT_ARGUMENTS['@varkey'];
            if(TYPE_DEF.TryParse(@varname_txt,out @varname_tmp))
                @varname=(TYPE_DEF)@varname_tmp;\nelse{\n";
            text += "\t\t\t\tif(@varname_txt==\"true\" || @varname_txt==\"TRUE\" || @varname_txt==\"on\" || @varname_txt==\"On\" || @varname_txt==\"ON\") @varname=true;";
            text += "\t\t\t\telse if(@varname_txt==\"false\" || @varname_txt==\"FALSE\" || @varname_txt==\"off\" || @varname_txt==\"Off\" || @varname_txt==\"OFF\") @varname=false;";

            return text;

        }

        static string BuildEnumBinder(Type type) {
            var text = @"\t\t\tint @varname_tmp=0;string @varname_txt=_YITEC_INPUT_ARGUMENTS['@varkey'];
            if(TYPE_DEF.TryParse(@varname_txt,out @varname_tmp))
                @varname=(TYPE_DEF)@varname_tmp;\nelse{\n";
            var names = Enum.GetNames(type);
            text += "\t\t\tswitch(@varname_txt){\n";
            foreach (var ename in names) {
                text += "\t\t\t\tcase \"" + ename + "\":@varname=" + type.FullName + "." + ename + ";break;";
            }
            return text;

        }
        

        static string BuildObjectBinder(Type type, ref string binderMemberCodes) {
            return null;
        }
    }
}

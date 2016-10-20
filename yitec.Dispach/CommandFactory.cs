﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public class CommandFactory : ICommandFactory
    {
        SortedDictionary<string, SortedDictionary<string, Dictionary<HttpMethods, ICommand>>> _Datas = new SortedDictionary<string, SortedDictionary<string, Dictionary<HttpMethods, ICommand>>>();

        public void AddCommand(string controllerName, string actionName,HttpMethods method, ICommand cmd) {
            SortedDictionary<string, Dictionary<HttpMethods, ICommand>> actions = null;
            if (!_Datas.TryGetValue(controllerName, out actions)) {
                actions = new SortedDictionary<string, Dictionary<HttpMethods, ICommand>>();
                _Datas.Add(controllerName,actions);
            }
            Dictionary<HttpMethods, ICommand> methods = null;
            if (!actions.TryGetValue(actionName, out methods)) {
                methods = new Dictionary<HttpMethods, ICommand>();
                actions.Add(actionName,methods);
            }
            if (((int)method & (int)HttpMethods.GET) > 0) {
                if (methods.ContainsKey(HttpMethods.GET)) methods[HttpMethods.GET] = cmd;
                else methods.Add(HttpMethods.GET,cmd);
            }

            if (((int)method & (int)HttpMethods.POST) > 0)
            {
                if (methods.ContainsKey(HttpMethods.POST)) methods[HttpMethods.POST] = cmd;
                else methods.Add(HttpMethods.POST, cmd);
            }

            if (((int)method & (int)HttpMethods.PUT) > 0)
            {
                if (methods.ContainsKey(HttpMethods.PUT)) methods[HttpMethods.PUT] = cmd;
                else methods.Add(HttpMethods.PUT, cmd);
            }

            if (((int)method & (int)HttpMethods.DELETE) > 0)
            {
                if (methods.ContainsKey(HttpMethods.DELETE)) methods[HttpMethods.DELETE] = cmd;
                else methods.Add(HttpMethods.DELETE, cmd);
            }
        }

        public ICommand GetOrCreateCommand(RouteData routeData,HttpMethods method,Context context)
        {
            SortedDictionary<string, Dictionary<HttpMethods,ICommand>> actions = null;
            if (!_Datas.TryGetValue(routeData.ControllerName, out actions))
            {
                return null;
            }
            Dictionary<HttpMethods, ICommand> methods = null;
            if (!actions.TryGetValue(routeData.ActionName, out methods))
            {
                return null;
            }
            foreach (var pair in methods) {
                if (((int)pair.Key & (int)method) > 0) return pair.Value;
            }
            return null;

        }
    }
}

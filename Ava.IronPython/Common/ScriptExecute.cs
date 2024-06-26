﻿using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;
using IronPython.Runtime;
using Ava.IronPython.Models;
using Avalonia;
using Microsoft.Scripting.Runtime;
using System.IO;
namespace Ava.IronPython.Common
{
    internal class ScriptExecute
    {
        private static ScriptEngine? eng;
        private static ScriptScope? scope;
        public static async Task<(List<PyVariable>, ScriptScope)> Execute(string script, ScriptScope? scp, string filePath = "")
        {
            eng ??= Python.CreateEngine();
            if (scp == null)
            {
                scope = eng.CreateScope();
                scp = scope;
            }
            else
            {
                scp = scope;
            }

            // 设置引用库
            var paths = eng.GetSearchPaths();

            if (!paths.Contains(@"C:\Program Files\IronPython 3.4\Lib"))
            {
                paths.Add(@"C:\Program Files\IronPython 3.4\Lib");

            }
            if (!paths.Contains(Environment.CurrentDirectory))
            {
                paths.Add(Environment.CurrentDirectory);
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!paths.Contains(dir))
                {
                    paths.Add(dir);
                }
            }

            eng.SetSearchPaths(paths);

            await Task.Run(() =>
            {
                eng.Execute(script, scope);
            });
            if (scope == null)
            {
                return ([], scope);
            }

            List<string> variables = scope.GetVariableNames().Where(x => !x.Contains("__")).ToList();

            List<PyVariable> result = new List<PyVariable>();
            for (int i = 0; i < variables.Count; i++)
            {
                var val = scope.GetVariable(variables[i]);
                switch (val)
                {
                    case int:
                    case double:
                    case string:
                        result.Add(new PyVariable()
                        {
                            Name = variables[i],
                            Value = val.ToString(),
                        });
                        break;
                    case PythonList pyList:
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var item in pyList)
                        {
                            stringBuilder.Append(item.ToString() + ",");
                        }
                        result.Add(new PyVariable()
                        {
                            Name = variables[i],
                            Value = stringBuilder.ToString().TrimEnd(','),
                        });
                        break;
                    default:
                        break;
                }
            }

            return (result, scope);
        }
    }
}

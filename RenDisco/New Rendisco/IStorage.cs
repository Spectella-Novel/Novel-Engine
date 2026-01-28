using System;
using System.Collections.Generic;
using System.Linq;

namespace RenDisco
{
    public interface IStorage
    {
        public void Add(Variable variable);
        public Variable Get(string name);
    }
    
    public class Variable
    {
        public string Type;
        public string Name;
        public object Value;
        public Parameter[] Parameters;
        public Variable(){}
        public Variable(Define define)
        {
            Name = define.Name;
            Type = define.Definition.MethodName;
            Parameters = define.Definition.ParamList.Params.Select(parameter =>
                new Parameter()
                {
                    Name = parameter.ParamName,
                    // приводим к литералу, предпологая что нам уже дают вычисленные значения
                    Value = ((Literal)parameter.ParamValue).Value ?? throw new ArgumentException($"Expression don't calculate")
                }
            ).ToArray();
        }
    }

    public static class VariableType
    {
        public const string STRING = "string";
        public const string BOOLEAN = "boolean";
        public const string NUMBER = "number";
    }
    
    public struct Parameter
    {
        public string Name;
        public object Value;
    }
}

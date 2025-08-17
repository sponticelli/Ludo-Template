using System;
using UnityEngine;

namespace Ludo.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public class ConditionalAttribute : PropertyAttribute
    {

        
        public string VariableName = "";

        public bool IsTrue = true;

        public bool HideInInspector = true;

        public ConditionalPair Pair;

        public ConditionalAttribute(string variableName)
        {
            VariableName = variableName;
            IsTrue = true;
            HideInInspector = true;
        }

        public ConditionalAttribute(string variableName, bool isTrue, bool hideInInspector)
        {
            VariableName = variableName;
            IsTrue = isTrue;
            HideInInspector = hideInInspector;
        }

        public ConditionalAttribute(string variableNameA, string variableNameB)
        {
            Pair = new ConditionalPair(variableNameA, variableNameB);
            HideInInspector = true;
        }

        public ConditionalAttribute(string variableNameA, string variableNameB, LogicOperators op)
        {
            Pair = new ConditionalPair(variableNameA, variableNameB, op);
            HideInInspector = true;
        }
    }
}
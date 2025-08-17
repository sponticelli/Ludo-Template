namespace Ludo.Attributes
{
    public struct ConditionalPair
    {
        public string VariableA;

        public bool ATrue;

        public string VariableB;

        public bool BTrue;

        public LogicOperators Operators;

        public ConditionalPair(string varA, string varB)
        {
            VariableA = varA;
            ATrue = true;
            VariableB = varB;
            BTrue = true;
            Operators = LogicOperators.AND;
        }

        public ConditionalPair(string varA, string varB, LogicOperators op)
        {
            VariableA = varA;
            ATrue = true;
            VariableB = varB;
            BTrue = true;
            Operators = op;
        }

        public ConditionalPair(string varA, bool a, string varB, bool b)
        {
            VariableA = varA;
            ATrue = a;
            VariableB = varB;
            BTrue = b;
            Operators = LogicOperators.AND;
        }

        public ConditionalPair(string varA, bool a, string varB, bool b, LogicOperators op)
        {
            VariableA = varA;
            ATrue = a;
            VariableB = varB;
            BTrue = b;
            Operators = op;
        }
    }
}
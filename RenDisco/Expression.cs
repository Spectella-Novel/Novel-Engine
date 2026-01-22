using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static RenDisco.ArithmeticExpression;

namespace RenDisco
{
    // ────────────────────────────────────────────────────────────────
    // The root class of all expressions
    // ────────────────────────────────────────────────────────────────

    public abstract class Expression
    {
        public abstract string Type { get; }
        public abstract Expression? Evaluate(IStorage storage);
    }

    // ────────────────────────────────────────────────────────────────
    // Literal
    // ────────────────────────────────────────────────────────────────

    public abstract class Literal : Expression
    {
        public object Value { get; protected set; }
        public override string ToString() => Value.ToString();
        public override Expression? Evaluate(IStorage _) => this;


    }

    public class NumberLiteral : Literal
    {
        public new double Value { get => (double)base.Value; protected set => base.Value = value; }
        public NumberLiteral(double value) => Value = value;
        public override string Type => nameof(NumberLiteral);
    }

    public class StringLiteral : Literal
    {
        public new string Value { get => (string)base.Value; protected set => base.Value = value; }
        public StringLiteral(string value) => Value = value;
        public override string Type => nameof(StringLiteral);
    }

    public class BooleanLiteral : Literal
    {
        public new bool Value { get => (bool)base.Value; protected set => base.Value = value; }
        public BooleanLiteral(bool value) => Value = value;
        public override string Type => nameof(BooleanLiteral);
    }

    // ────────────────────────────────────────────────────────────────
    // Variable reference
    // ────────────────────────────────────────────────────────────────

    public class VariableReference : Expression
    {
        public string Name { get; }
        public VariableReference(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));
        public override string Type => nameof(VariableReference);

        public override Expression? Evaluate(IStorage storage)
        {
            var variable = storage.Get(Name);
            if (variable == null)
                throw new KeyNotFoundException($"Variable '{Name}' not found.");

            return variable.Type switch
            {
                VariableType.NUMBER => new NumberLiteral(Convert.ToDouble(variable.Value ?? 0)),
                VariableType.STRING => new StringLiteral(variable.Value?.ToString() ?? ""),
                VariableType.BOOLEAN => new BooleanLiteral(Convert.ToBoolean(variable.Value)),
                _ => throw new NotSupportedException($"Unsupported variable type: {variable.Type}")
            };
        }
    }

    // ────────────────────────────────────────────────────────────────
    // Arithmetic expressions
    // ────────────────────────────────────────────────────────────────

    public abstract class ArithmeticExpression : Expression
    {
        public Expression Left { get; }
        public Expression Right { get; }
        public string Operator { get; protected set; } 

        protected ArithmeticExpression(Expression left, Expression right, string op)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Operator = op ?? throw new ArgumentNullException(nameof(op));
        }
    }

    public class AdditiveExpression : ArithmeticExpression
    {
        public AdditiveExpression(Expression left, Expression right, string op)
            : base(left, right, op) { }

        public override string Type => nameof(AdditiveExpression);

        public override Expression? Evaluate(IStorage storage)
        {
            var leftVal = Left.Evaluate(storage);
            var rightVal = Right.Evaluate(storage);

            return Operator switch
            {
                "+" => Add(leftVal, rightVal),
                "-" => Subtract(leftVal, rightVal),
                _ => throw new ArgumentException($"Unsupported additive operator: '{Operator}'")
            };
        }

        private static Expression Add(Expression? a, Expression? b)
        {
            return (a, b) switch
            {
                (null, _) or (_, null) =>
                    throw new InvalidOperationException("Cannot add null"),

                // Строка + что угодно → строка
                (StringLiteral s1, _) => new StringLiteral(s1.Value + ToStringSafe(b)),
                (_, StringLiteral s2) => new StringLiteral(ToStringSafe(a) + s2.Value),

                // Число + число → число
                (NumberLiteral n1, NumberLiteral n2) => new NumberLiteral(n1.Value + n2.Value),

                // Булево → число (для совместимости, как в Python/JS)
                (BooleanLiteral bl1, NumberLiteral n2) => new NumberLiteral(BoolToNumber(bl1.Value) + n2.Value),
                (NumberLiteral n1, BooleanLiteral bl2) => new NumberLiteral(n1.Value + BoolToNumber(bl2.Value)),
                (BooleanLiteral bl1, BooleanLiteral bl2) => new NumberLiteral(BoolToNumber(bl1.Value) + BoolToNumber(bl2.Value)),

                _ => throw new InvalidOperationException($"Cannot add {a?.Type} and {b?.Type}")
            };
        }

        private static Expression Subtract(Expression? a, Expression? b)
        {
            return (a, b) switch
            {
                (null, _) or (_, null) =>
                    throw new InvalidOperationException("Cannot subtract null"),

                (NumberLiteral n1, NumberLiteral n2) => new NumberLiteral(n1.Value - n2.Value),

                (BooleanLiteral bl1, NumberLiteral n2) => new NumberLiteral(BoolToNumber(bl1.Value) - n2.Value),
                (NumberLiteral n1, BooleanLiteral bl2) => new NumberLiteral(n1.Value - BoolToNumber(bl2.Value)),
                (BooleanLiteral bl1, BooleanLiteral bl2) => new NumberLiteral(BoolToNumber(bl1.Value) - BoolToNumber(bl2.Value)),

                _ => throw new InvalidOperationException($"Cannot subtract {a?.Type} and {b?.Type}")
            };
        }

        // Вспомогательные утилиты
        private static string ToStringSafe(Expression? expr) => expr switch
        {
            null => "null",
            BooleanLiteral bl => bl.Value.ToString().ToLower(),
            NumberLiteral nl => nl.Value.ToString(),
            StringLiteral sl => sl.Value,
            _ => expr.Type  // fallback
        };

        private static double BoolToNumber(bool b) => b ? 1.0 : 0.0;
    }

    public class MultiplicativeExpression : ArithmeticExpression
    {

        public MultiplicativeExpression(Expression left, Expression right, string op)
            : base(left, right, op) { }

        public override string Type => nameof(MultiplicativeExpression);

        public override Expression? Evaluate(IStorage storage)
        {
            var leftVal = Left.Evaluate(storage) as NumberLiteral ??
                throw new InvalidOperationException($"Left operand must be a number: {Left.Type}");
            var rightVal = Right.Evaluate(storage) as NumberLiteral ??
                throw new InvalidOperationException($"Right operand must be a number: {Right.Type}");

            return Operator switch
            {
                "*" => new NumberLiteral(leftVal.Value * rightVal.Value),
                "/" => new NumberLiteral(leftVal.Value / rightVal.Value),
                _ => throw new ArgumentException($"Unsupported multiplicative operator: '{Operator}'")
            };
        }
    }

    // ────────────────────────────────────────────────────────────────
    // Conditional expressions (anything that returns a bool)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Base class for all expressions that evaluate to a boolean value (after evaluation).
    /// </summary>
    public abstract class ConditionalExpression : Expression
    {
        public override string Type => GetType().Name;

        protected static bool ToBoolean(Expression? expr)
        {
            return expr switch
            {
                null => false,
                BooleanLiteral bl => bl.Value,
                NumberLiteral nl => nl.Value != 0,
                StringLiteral sl => !string.IsNullOrEmpty(sl.Value),
                _ => throw new InvalidOperationException($"Cannot convert {expr?.Type} to boolean")
            };
        }
    }

    // ─── Comparisons: a == b, x < y, и т.д.
    public class ComparisonExpression : ConditionalExpression
    {
        public Expression Left { get; }
        public Expression Right { get; }
        public string Operator { get; } // "==", "!=", "<", "<=", ">", ">="

        public ComparisonExpression(Expression left, Expression right, string op)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Operator = op ?? throw new ArgumentNullException(nameof(op));
        }

        public override Expression? Evaluate(IStorage storage)
        {
            var leftVal = Left.Evaluate(storage);
            var rightVal = Right.Evaluate(storage);

            bool result = Operator switch
            {
                "==" => AreEqual(leftVal, rightVal),
                "!=" => !AreEqual(leftVal, rightVal),
                "<" => Compare(leftVal, rightVal) < 0,
                ">" => Compare(leftVal, rightVal) > 0,
                "<=" => Compare(leftVal, rightVal) <= 0,
                ">=" => Compare(leftVal, rightVal) >= 0,
                _ => throw new ArgumentException($"Unsupported comparison operator: '{Operator}'")
            };

            return new BooleanLiteral(result);
        }

        private static bool AreEqual(Expression? a, Expression? b)
        {
            return (a, b) switch
            {
                (null, null) => true,
                (null, _) or (_, null) => false,
                (NumberLiteral n1, NumberLiteral n2) => n1.Value == n2.Value,
                (StringLiteral s1, StringLiteral s2) => s1.Value == s2.Value,
                (BooleanLiteral b1, BooleanLiteral b2) => b1.Value == b2.Value,
                _ => false // different types → not equal
            };
        }

        private static int Compare(Expression? a, Expression? b)
        {
            return (a, b) switch
            {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                (NumberLiteral n1, NumberLiteral n2) => n1.Value.CompareTo(n2.Value),
                (StringLiteral s1, StringLiteral s2) => string.CompareOrdinal(s1.Value, s2.Value),
                _ => throw new InvalidOperationException($"Cannot compare {a?.Type} and {b?.Type}")
            };
        }
    }

    // ─── Logical unary: not x
    public abstract class LogicalUnaryExpression : ConditionalExpression
    {
        public Expression Operand { get; }

        protected LogicalUnaryExpression(Expression operand)
            => Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    public class LogicalNotExpression : LogicalUnaryExpression
    {
        public LogicalNotExpression(Expression operand) : base(operand) { }

        public override Expression? Evaluate(IStorage storage)
        {
            var val = Operand.Evaluate(storage);
            return new BooleanLiteral(!ToBoolean(val));
        }
    }

    // ─── Logical binary: a and b, a or b
    public abstract class LogicalBinaryExpression : ConditionalExpression
    {
        public Expression Left { get; }
        public Expression Right { get; }

        protected LogicalBinaryExpression(Expression left, Expression right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
    }

    public class LogicalAndExpression : LogicalBinaryExpression
    {
        public LogicalAndExpression(Expression left, Expression right) : base(left, right) { }

        public override Expression? Evaluate(IStorage storage)
        {
            var leftVal = Left.Evaluate(storage);
            if (!ToBoolean(leftVal)) return new BooleanLiteral(false);

            var rightVal = Right.Evaluate(storage);
            return new BooleanLiteral(ToBoolean(rightVal));
        }
    }

    public class LogicalOrExpression : LogicalBinaryExpression
    {
        public LogicalOrExpression(Expression left, Expression right) : base(left, right) { }

        public override Expression? Evaluate(IStorage storage)
        {
            var leftVal = Left.Evaluate(storage);
            if (ToBoolean(leftVal)) return new BooleanLiteral(true);

            var rightVal = Right.Evaluate(storage);
            return new BooleanLiteral(ToBoolean(rightVal));
        }
    }

    public class ParamPairExpression 
    {
        public string? ParamName { get; set; }
        public Expression ParamValue { get; set; }
    }

    public class ArgumentsExpression
    {
        public List<ParamPairExpression> Params { get; set; } = new List<ParamPairExpression>();
    }

    public class MethodExpression
    {
        public string MethodName { get; set; }
        public ArgumentsExpression ParamList { get; set; }
    }
}
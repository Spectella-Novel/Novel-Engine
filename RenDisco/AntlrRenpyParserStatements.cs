using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Unity.Plastic.Antlr3.Runtime.Tree;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine.TerrainTools;
using static RenpyParser;

namespace RenDisco
{
    /// <summary>
    /// Parses Ren'Py script code and creates a list of RenpyCommand objects to represent the script.
    /// </summary>
    public partial class AntlrRenpyParser : RenpyBaseVisitor<object>, IRenpyParser
    {

        public override object VisitLabel_def([NotNull] RenpyParser.Label_defContext context)
        {
            return new Label
            {
                Name = context.IDENT().GetText(),
                Instructions = (context.block() != null) ? 
                    (List<Instruction>)Visit(context.block()) : new List<Instruction>(),
            };
        }

        public override object VisitDefine_def([NotNull] RenpyParser.Define_defContext context)
        {
            string valueName = context.IDENT(0).ToString();
            string type = context.IDENT(1).ToString();
            var pairs = new ArgumentsExpression();

            for (int i = 0; i < context.arguments().argument().Length; i++)
            {
                var pair = new ParamPairExpression();
                pair.ParamName = context.arguments().argument(i).IDENT()?.GetText() ?? string.Empty;
                pair.ParamValue = (Expression)Visit(context.arguments().argument(i).expression());
                pairs.Params.Add(pair);
            }
            return new Define
            {
                Name = valueName,
                Definition = new MethodExpression
                {
                    MethodName = type,
                    ParamList = pairs
                }
            };
        }

        public override object VisitAssignment([NotNull] RenpyParser.AssignmentContext context)
        {
            return new Define
            {
                Name = context.IDENT().GetText(),
                Value = (Expression)Visit(context.expression()),
            };
        }

        // ─── expression: additive | boolean_expression ──────────────────
        public override object VisitExpression([NotNull] RenpyParser.ExpressionContext context)
        {
            if (context.additive() != null)
                return Visit(context.additive());
            if (context.logical_or() != null)
                return Visit(context.logical_or());

            throw new InvalidOperationException("Unexpected expression");
        }

        public override object VisitLogical_or([NotNull] Logical_orContext context)
        {
            var expression = context.logical_and()
                .Select(m => (Expression)Visit(m))
                .ToArray();

            if (expression.Length == 1) return expression[0];

            Expression result = expression[0];
            for (int i = 0; i < context.logical_and().Length; i++)
            {
                result = new LogicalOrExpression(result, expression[i + 1]);
            }

            return result;
        }

        public override object VisitLogical_and([NotNull] Logical_andContext context)
        {
            Expression[] expression = context.logical_not()
                .Select(m => (Expression)Visit(m))
                .ToArray();

            if (expression.Length == 1) return expression[0];

            Expression result = expression[0];
            for (int i = 0; i < context.logical_not().Length; i++)
            {
                result = new LogicalAndExpression(result, expression[i + 1]);
            }
            return result;
        }
        public override object VisitLogical_not([NotNull] Logical_notContext context)
        {
            if(context.logical_not() != null)
                return Visit(context.logical_not());
            if(context.relational() != null)
                return Visit(context.relational());
            return null;
        }

        public override object VisitRelational([NotNull] RelationalContext context)
        {
            Expression[] expression = context.additive()
                .Select(m => (Expression)Visit(m))
                .ToArray();

            if (expression.Length == 1) return expression[0];

            Expression result = expression[0];
            for (int i = 0; i < context._op.Count; i++)
            {
                string op = context._op[i].Text; //  > < >= <= == 
                result = new ComparisonExpression(result, expression[i + 1], op);
            }
            return result;
        }

        public override object VisitAdditive([NotNull] RenpyParser.AdditiveContext context)
        {
            var mults = context.multiplicative()
                .Select(m => (Expression)Visit(m))
                .ToArray();

            if (mults.Length == 1) return mults[0];
            
            Expression result = mults[0];
            for (int i = 0; i < context._op.Count; i++)
            {
                string op = context._op[i].Text; // "+" или "-"
                result = new AdditiveExpression(result, mults[i + 1], op);
            }
            return result;
        }

        // ─── multiplicative: primary (op=(STAR | SLASH) primary)* ──────
        public override object VisitMultiplicative([NotNull] RenpyParser.MultiplicativeContext context)
        {
            var primaries = context.primary()
                .Select(p => (Expression)Visit(p))
                .ToArray();

            if (primaries.Length == 1) return primaries[0];


            Expression result = primaries[0];
            for (int i = 0; i < context._op.Count; i++)
            {

                string op = context._op[i].Text; // "*" или "/"
                result = new MultiplicativeExpression(result, primaries[i + 1], op);
            }
            return result;
        }

        // ─── primary: literal | '(' expression ')' ──────────────────────
        public override object VisitPrimary([NotNull] RenpyParser.PrimaryContext context)
        {
            if (context.literal() != null)
                return Visit(context.literal());

            if (context.expression() != null)
                return Visit(context.expression()); // скобки: просто возвращаем внутреннее выражение

            throw new InvalidOperationException("Unexpected primary");
        }

        // ─── literal: NUMBER | STRING | IDENT ───────────────────────────
        public override object VisitLiteral([NotNull] RenpyParser.LiteralContext context)
        {
            if (context.NUMBER() != null)
            {
                string text = context.NUMBER().GetText();
                return double.TryParse(text, out double d)
                    ? new NumberLiteral(d)
                    : throw new FormatException($"Invalid number: {text}");
            }

            if (context.STRING() != null)
            {
                string text = context.STRING().GetText();
                // Убираем кавычки (простая версия)
                string unquoted = text.Length >= 2
                    ? text.Substring(1, text.Length - 2)
                    : text;
                return new StringLiteral(unquoted);
            }

            if (context.IDENT() != null)
                return new VariableReference(context.IDENT().GetText());

            throw new InvalidOperationException("Unexpected literal");
        }

        public override object VisitScene_def([NotNull] RenpyParser.Scene_defContext context)
        {
            return new Scene { SceneVariable = context.IDENT().GetText() };
        }

        public override object VisitPause_def([NotNull] RenpyParser.Pause_defContext context)
        {
            return new Pause { Duration = double.Parse(context.NUMBER().GetText()) };
        }

        public override object VisitPlay_music_def([NotNull] RenpyParser.Play_music_defContext context)
        {
            var playMusic = new PlayMusic { AudioName = context.IDENT().GetText()};
            if (context.NUMBER() != null)
            {
                playMusic.FadeIn = float.Parse(context.NUMBER().GetText());
            }
            return playMusic;
        }

        public override object VisitStop_music_def([NotNull] RenpyParser.Stop_music_defContext context)
        {
            var stopMusic = new StopMusic { AudioName = context.IDENT().GetText() };
            if (context.NUMBER() != null)
            {
                stopMusic.FadeOut = float.Parse(context.NUMBER().GetText());
            }
            return stopMusic;
        }

        public override object VisitJump_def([NotNull] RenpyParser.Jump_defContext context)
        {
            return new Jump { Label = context.IDENT().GetText() };
        }

        /*
        public override object VisitCall_def([NotNull] RenpyParser.Call_defContext context)
        {
            return new Call { Label = context.IDENT().GetText()};
        }
        */

        public override object VisitMenu_def([NotNull] RenpyParser.Menu_defContext context)
        {
            var menu = new Menu();

            menu.Character = context.character_ref()?.ToString();
            menu.Text = context.STRING().ToString();

            foreach (var optionContext in context.menu_option())
            {
                menu.Choices.Add((MenuChoice)Visit(optionContext));
            }
            return menu;
        }

        public override object VisitMenu_option([NotNull] RenpyParser.Menu_optionContext context)
        {
            var choice = new MenuChoice { OptionText = context.STRING().GetText().Trim('"') };
            if (context.block() != null)
            {
                choice.Response.AddRange((List<Instruction>)Visit(context.block()));
            }
            return choice;
        }

        public override object VisitDefault_def([NotNull] RenpyParser.Default_defContext context)
        {
            return new Define { Name = context.IDENT().GetText(), Value = (Expression)Visit(context.expression()) };
        }

        public override object VisitReturn_def([NotNull] RenpyParser.Return_defContext context)
        {
            return new Return();
        }

        public override object VisitShow_def([NotNull] RenpyParser.Show_defContext context)
        {
            return new Show
            {
                Character = context.IDENT(0).ToString(),
                Emotion = context.IDENT(1)?.ToString(), // emotion is optional
            };
        }
        public override object VisitHide_def([NotNull] Hide_defContext context)
        {
            return new Hide
            {
                Character = context.IDENT().ToString(),
            };
        }
        public override object VisitDialogue([NotNull] RenpyParser.DialogueContext context)
        {
            return new Dialogue { Character = context.character_ref().IDENT().GetText(), Text = context.STRING().GetText().Trim('"') };
        }

        public override object VisitNarration([NotNull] RenpyParser.NarrationContext context)
        {
            return new Narration { Text = context.STRING().GetText().Trim('"') };
        }

        public override object VisitConditional_block([NotNull] RenpyParser.Conditional_blockContext context)
        {
            var conditional_block = new IfCondition();
            conditional_block.Condition = (Expression)Visit(context.expression());
            conditional_block.Content = (List<Instruction>)Visit(context.block());

            foreach (var elifBlock in context.elif_block())
            {
                conditional_block.ElifConditions.Add((ElifCondition)Visit(elifBlock));
            }

            if (context.else_block() != null)
            {
                conditional_block.ElifConditions.Add((ElifCondition)Visit(context.else_block()));
            }

            return conditional_block;
        }

        public override object VisitElif_block([NotNull] RenpyParser.Elif_blockContext context)
        {
            var elifCondition = new ElifCondition { Condition = (Expression)Visit(context.expression()) };
            elifCondition.Content = (List<Instruction>)Visit(context.block());
            return elifCondition;
        }

        public override object VisitElse_block([NotNull] RenpyParser.Else_blockContext context)
        {
            // Else block will always be the last one and therefore it is enough to specify in the conditions true
            var elseCondition = new ElifCondition { Condition = new BooleanLiteral(true)};
            elseCondition.Content = (List<Instruction>)Visit(context.block());
            return elseCondition;
        }

        public override object VisitArguments([NotNull] RenpyParser.ArgumentsContext context)
        {
            var argumentsExpression = new ArgumentsExpression();
            argumentsExpression.Params = context.argument()
                .Select(argument => 
                    (ParamPairExpression)Visit(argument))
                .ToList();
            return argumentsExpression;
        }

        public override object VisitArgument([NotNull] RenpyParser.ArgumentContext context)
        {
            return new ParamPairExpression()
                {
                    ParamName = context.IDENT().GetText(),
                    ParamValue = (Expression)Visit(context.expression()),
                };
        }

        public override object VisitDebug_def([NotNull] Debug_defContext context)
        {
            Expression expression = (Expression)Visit(context.expression());
            
            return new DebugLog()
            {
                Expression = expression,
            };
        }
    }
}
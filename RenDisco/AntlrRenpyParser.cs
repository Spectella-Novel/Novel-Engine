using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace RenDisco
{
    /// <summary>
    /// Parses Ren'Py script code and creates a list of RenpyCommand objects to represent the script.
    /// </summary>
    public partial class AntlrRenpyParser : RenpyBaseVisitor<object>, IRenpyParser
    {
        /// <summary>
        /// Parses a script from a file by file path.
        /// </summary>
        /// <param name="filePath">Path to the Ren'Py script file.</param>
        /// <returns>A list of RenpyCommand objects representing the script.</returns>
        public List<Instruction> ParseFromFile(string filePath)
        {
            string rpyCode = File.ReadAllText(filePath);
            return Parse(rpyCode);
        }

        /// <summary>
        /// Parses Ren'Py script code from a string.
        /// </summary>
        /// <param name="rpyCode">The Ren'Py script code as a string.</param>
        /// <returns>A list of RenpyCommand objects representing the script.</returns>
        public List<Instruction> Parse(string rpyCode)
        {
            // Parse the input.
            AntlrInputStream inputStream = new AntlrInputStream(rpyCode);
            RenpyLexer lexer = new RenpyLexer(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            RenpyParser parser = new RenpyParser(tokens);

            var context = parser.block();

            // Evaluate the parsed tree.
            return (List<Instruction>)Visit(context);
        }

        public override object Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }

        public override object VisitBlock([NotNull] RenpyParser.BlockContext context)
        {
            List<Instruction> commands = new List<Instruction>();
            var array = context.statement();
            for (int i = 0; i < array.Length; i++)
            {
                var statement = array[i];
                if(statement.GetText().Length == 0)
                {
                    Console.WriteLine(1);
                    continue;
                }
                try
                {
                    Instruction command = (Instruction)Visit(statement);
                    if (command != null)
                    {
                        commands.Add(command);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in lines: " + i + '\n' + statement.GetText() + ex.Message);
                    throw;
                }

            }
            return commands;
        }
    }
}
﻿using ExpressionEvaluator.CodeGen;
using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.SyntaxAnalysis;
using ExpressionEvaluator.Utilites;
using System;
using System.Linq;

namespace ExpressionEvaluator
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Expression> ");

                var line = Console.ReadLine();

                using (var reader = StringHelpers.TextReaderFromString(line))
                {
                    var lexer = new Lexer(reader);
                    var parser = new Parser(lexer);
                    var expression = parser.ParseExpression();

                    if (parser.Diagnostics.Any())
                    {
                        foreach (var diagnostic in parser.Diagnostics)
                        {
                            Console.WriteLine($"{diagnostic.Type}: {diagnostic.Message} ({diagnostic.Span.Start})");
                        }

                        Console.WriteLine();

                        continue;
                    }

                    try
                    {
                        var generator = new CodeGenerator();
                        var result = generator.Generate(expression);

                        Console.WriteLine(result());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                Console.WriteLine();
            }
        }
    }
}

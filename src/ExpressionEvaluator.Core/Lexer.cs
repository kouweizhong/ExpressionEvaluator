﻿using System;
using System.IO;
using System.Text;

namespace ExpressionEvaluator
{
	/// <summary>
	/// Lexer.
	/// </summary>
	public class Lexer
	{
		private LexerState? state;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExpressionEvaluator.Lexer"/> class.
		/// </summary>
		/// <param name="textReader">Text reader.</param>
		public Lexer(TextReader textReader)
		{
			TextReader = textReader;

            Line = 1;
            Column = 1;
		}

		/// <summary>
		/// Gets the text reader.
		/// </summary>
		/// <value>The text reader.</value>
		public TextReader TextReader
		{
			get;
		}

		/// <summary>
		/// Peeks a token.
		/// </summary>
		/// <returns>The token.</returns>
		public TokenInfo PeekToken()
		{
			if (state != null)
			{
                var s = (LexerState)state;
				var peekedToken = s.Token;
                Line = s.Line;
                Column = s.Column;
				return peekedToken;
			}
			else 
			{
				var peekToken = ReadTokenCore();
				state = new LexerState(peekToken, Line, Column);
				return peekToken;
			}
		}

		/// <summary>
		/// Reads a token.
		/// </summary>
		/// <returns>The token.</returns>
		public TokenInfo ReadToken()
		{
			if (state != null)
			{
				var token = state.Value.Token;
				state = null;
				return token;
			}
			return ReadTokenCore();
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:ExpressionEvaluator.Lexer"/> has reached EOF.
		/// </summary>
		/// <value><c>true</c> if is EOF; otherwise, <c>false</c>.</value>
		public bool IsEof
		{
			get
			{
				return PeekToken().Kind == TokenKind.EndOfFile;
			}
		}

		/// <summary>
		/// Internal method for reading a token.
		/// </summary>
		/// <returns>The token core.</returns>
		private TokenInfo ReadTokenCore()
		{
			while (!IsEofCore)
			{
                int line = Line;
                int column = Column;

				var c = PeekChar();

				if (char.IsDigit(c))
				{
					var stringBuilder = new StringBuilder();
					do
					{
						ReadChar();

						stringBuilder.Append(c);

						c = PeekChar();
					} while (char.IsDigit(c));

					return new TokenInfo(TokenKind.Number, line, column, stringBuilder.Length, stringBuilder.ToString());
				}
				else 
				{
					ReadChar();

					switch (c) 
					{
						case '+':
							return new TokenInfo(TokenKind.Plus, line, column, 1);

						case '-':
							return new TokenInfo(TokenKind.Minus, line, column, 1);

						case '*':
							return new TokenInfo(TokenKind.Star, line, column, 1);

						case '/':
							return new TokenInfo(TokenKind.Slash, line, column, 1);

						case '(':
							return new TokenInfo(TokenKind.OpenParen, line, column, 1);

						case ')':
							return new TokenInfo(TokenKind.CloseParen, line, column, 1);

						case ' ':
							//return new TokenInfo(TokenKind.Whitespace, 1);
							break;

                        case '\r':
                            break;

                        case '\n':
                            Line++;
                            Column = 1;
                            break;

                        default:
						    return new TokenInfo(TokenKind.Invalid, line, column, 1, c.ToString());
					}
				}
			}

			return new TokenInfo(TokenKind.EndOfFile, Line, Column, 0);
		}

		private bool IsEofCore
		{
			get
			{
				return TextReader.Peek() == -1;
			}
		}

        private int Line { get; set; }
        private int Column { get; set; }

        private char ReadChar()
		{
            Column++;
			return (char)TextReader.Read();
		}

		private char PeekChar()
		{
			return (char)TextReader.Peek();
		}

		/// <summary>
		/// Encapsulates a lookahead state for the Lexer..
		/// </summary>
		struct LexerState
		{
			public LexerState(TokenInfo token, int line, int column)
			{
				Token = token;
                Line = line;
                Column = column;
			}

			public TokenInfo Token { get;}

            public int Line { get; }

            public int Column { get; }
        }
	}
}

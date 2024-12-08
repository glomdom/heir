﻿namespace Heir.Syntax
{
    public static class TokenFactory
    {
        public static Token Keyword(SyntaxKind kind, Location location)
        {
            return new Token(kind, SyntaxFacts.KeywordMap.GetKey(kind), null, location);
        }

        public static Token Identifier(string text, Location location)
        {
            return new Token(SyntaxKind.Identifier, text, null, location);
        }

        public static Token Operator(SyntaxKind kind, string text, Location location)
        {
            return new Token(kind, text, null, location);
        }

        public static Token BoolLiteral(string text, Location location)
        {
            return new Token(SyntaxKind.BoolLiteral, text, text == "true", location);
        }

        public static Token StringLiteral(string text, Location location)
        {
            return new Token(SyntaxKind.StringLiteral, text.Substring(1, text.Length - 2), text, location);
        }

        public static Token CharLiteral(string text, Location location)
        {
            return new Token(SyntaxKind.CharLiteral, text.Substring(1, text.Length - 2), text, location);
        }

        public static Token IntLiteral(string text, Location location, int radix = 10)
        {
            return new Token(SyntaxKind.IntLiteral, text, Convert.ToInt64(radix == 10 ? text : text.Substring(2), radix), location);
        }

        public static Token FloatLiteral(string text, Location location)
        {
            return new Token(SyntaxKind.FloatLiteral, text, Convert.ToDouble(text), location);
        }

        public static Token NoneLiteral(Location location)
        {
            return new Token(SyntaxKind.NoneLiteral, "none", null, location);
        }

        public static Token Trivia(string text, Location location, TriviaKind kind)
        {
            return new TriviaToken(text, location, kind);
        }
    }
}

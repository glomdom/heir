﻿using Heir.AST;

namespace Heir
{
    enum Context
    {
        Global,
        Parameters
    }

    public sealed class Binder(SyntaxTree syntaxTree) : Statement.Visitor<BoundStatement>, Expression.Visitor<BoundExpression>
    {
        private readonly SyntaxTree _syntaxTree = syntaxTree;
        private readonly Dictionary<SyntaxNode, BoundSyntaxNode> _boundNodes = new();
        private Context _context = Context.Global;

        private BoundStatement GetBoundNode(Statement statement) => (BoundStatement)_boundNodes[statement];
        private BoundExpression GetBoundNode(Expression expression) => (BoundExpression)_boundNodes[expression];

        public BoundStatement VisitSyntaxTree(SyntaxTree syntaxTree) => new BoundSyntaxTree(BindStatements(syntaxTree.Statements));

        public BoundExpression VisitAssignmentOpExpression(AssignmentOp assignmentOp)
        {
            throw new NotImplementedException();
        }

        public BoundExpression VisitBinaryOpExpression(BinaryOp binaryOp)
        {
            throw new NotImplementedException();
        }

        public BoundExpression VisitIdentifierNameExpression(IdentifierName identifierName)
        {
            throw new NotImplementedException();
        }

        public BoundExpression VisitLiteralExpression(Literal literal)
        {
            throw new NotImplementedException();
        }

        public BoundExpression VisitNoOp(NoOp noOp)
        {
            throw new NotImplementedException();
        }

        public BoundExpression VisitParenthesizedExpression(Parenthesized parenthesized)
        {
            throw new NotImplementedException();
        }

        public BoundExpression VisitUnaryOpExpression(UnaryOp unaryOp)
        {
            throw new NotImplementedException();
        }

        //private List<BoundStatement> BindStatements(List<Statement> statements) => statements.ConvertAll(Bind);
        private List<BoundSyntaxNode> BindStatements(List<SyntaxNode> statements) => statements.ConvertAll(Bind); // temp

        private BoundStatement Bind(Statement statement)
        {
            var boundStatement = statement.Accept(this);
            _boundNodes.Add(statement, boundStatement);
            return boundStatement;
        }

        private BoundExpression Bind(Expression expression)
        {
            var boundExpression = expression.Accept(this);
            _boundNodes.Add(expression, boundExpression);
            return boundExpression;
        }

        private BoundSyntaxNode Bind(SyntaxNode node)
        {
            if (node is Expression expression)
                return Bind(expression.Accept(this));
            else if (node is Statement statement)
                return Bind(statement.Accept(this));

            return null!; // poop
        }
    }
}
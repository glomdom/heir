﻿using Heir.Syntax;
using Heir.AST;
using Heir.CodeGeneration;

namespace Heir
{
    public sealed class BytecodeGenerator(Binder binder, SyntaxTree syntaxTree) : Statement.Visitor<List<Instruction>>, Expression.Visitor<List<Instruction>>
    {
        // TODO: do not continue pipeline (codegen/evaluation) if we have errors
        public DiagnosticBag Diagnostics { get; } = binder.Diagnostics;

        private readonly Binder _binder = binder;
        private readonly SyntaxTree _syntaxTree = syntaxTree;

        public List<Instruction> GenerateBytecode() => GenerateBytecode(_syntaxTree);

        public List<Instruction> VisitSyntaxTree(SyntaxTree syntaxTree) => GenerateStatementsBytecode(syntaxTree.Statements);
        // TODO: create scope
        public List<Instruction> VisitBlock(Block block) => GenerateStatementsBytecode(block.Statements);

        public List<Instruction> VisitAssignmentOpExpression(AssignmentOp assignmentOp) => VisitBinaryOpExpression(assignmentOp);
        public List<Instruction> VisitBinaryOpExpression(BinaryOp binaryOp)
        {
            var leftInstructions = GenerateBytecode(binaryOp.Left);
            var rightInstructions = GenerateBytecode(binaryOp.Right);
            var combined = leftInstructions.Concat(rightInstructions);

            if (BinaryOp.StandardOpCodeMap.TryGetValue(binaryOp.Operator.Kind, out var standardOp))
                return combined.Append(new Instruction(binaryOp, standardOp)).ToList();

            if (BinaryOp.AssignmentOpCodeMap.TryGetValue(binaryOp.Operator.Kind, out var assignmentOp))
                return leftInstructions
                    .Concat(rightInstructions)
                    .Append(new Instruction(binaryOp, assignmentOp))
                    .Append(new Instruction(binaryOp, OpCode.STORE))
                    .ToList();

            Diagnostics.Error("H011", $"Unsupported binary operator kind: {binaryOp.Operator.Kind}", binaryOp.Operator);
            return [new Instruction(binaryOp, OpCode.NOOP)];
        }

        public List<Instruction> VisitUnaryOpExpression(UnaryOp unaryOp)
        {
            var value = GenerateBytecode(unaryOp.Operand);
            var bytecode = unaryOp.Operator.Kind switch
            {
                SyntaxKind.Bang => value.Append(new Instruction(unaryOp, OpCode.NOT)),
                SyntaxKind.Tilde => value.Append(new Instruction(unaryOp, OpCode.BNOT)),
                SyntaxKind.Minus => value.Append(new Instruction(unaryOp, OpCode.UNM)),
                SyntaxKind.PlusPlus => value.Append(new Instruction(unaryOp, OpCode.PUSH, 1)).Append(new Instruction(unaryOp, OpCode.ADD)),
                SyntaxKind.MinusMinus => value.Append(new Instruction(unaryOp, OpCode.PUSH, 1)).Append(new Instruction(unaryOp, OpCode.SUB)),

                _ => null!
            };

            return bytecode.ToList();
        }

        public List<Instruction> VisitIdentifierNameExpression(IdentifierName identifierName) => [new Instruction(identifierName, OpCode.LOAD, identifierName.Token.Text)];
        public List<Instruction> VisitParenthesizedExpression(Parenthesized parenthesized) => GenerateBytecode(parenthesized.Expression);
        public List<Instruction> VisitLiteralExpression(Literal literal) => [new Instruction(literal, OpCode.PUSH, literal.Token.Value)];
        public List<Instruction> VisitNoOp(NoOp noOp) => [new Instruction(noOp, OpCode.NOOP)];

        //private List<Instruction> GenerateStatementsBytecode(List<Statement> statements) => statements.SelectMany(GenerateBytecode).ToList();
        private List<Instruction> GenerateStatementsBytecode(List<SyntaxNode> statements) => statements.SelectMany(GenerateBytecode).ToList(); // temp
        private List<Instruction> GenerateBytecode(Expression expression) => expression.Accept(this);
        private List<Instruction> GenerateBytecode(Statement statement) => statement.Accept(this);
        private List<Instruction> GenerateBytecode(SyntaxNode node)
        {
            if (node is Expression expression)
                return GenerateBytecode(expression);
            else if (node is Statement statement)
                return GenerateBytecode(statement);

            return null!; // poop
        }
    }
}

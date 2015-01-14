﻿namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;



    /// <summary>
    /// A C# statement contains a complex conditional expression which omits parenthesis around operators.
    /// </summary>
    /// <remarks>
    /// <para>C# maintains a hierarchy of precedence for conditional operators. It is possible in C# to string multiple
    /// conditional operations together in one statement without wrapping any of the operations in parenthesis, in which
    /// case the compiler will automatically set the order and precedence of the operations based on these
    /// pre-established rules. For example:</para>
    ///
    /// <code language="csharp">
    /// if (x || y &amp;&amp; z &amp;&amp; a || b)
    /// {
    /// }
    /// </code>
    ///
    /// <para>Although this code is legal, it is not highly readable or maintainable. In order to achieve full
    /// understanding of this code, the developer must know and understand the basic operator precedence rules in
    /// C#.</para>
    ///
    /// <para>This rule is intended to increase the readability and maintainability of this type of code, and to reduce
    /// the risk of introducing bugs later, by forcing the developer to insert parenthesis to explicitly declare the
    /// operator precedence. For example, a developer could write this code as:</para>
    ///
    /// <code language="csharp">
    /// if ((x || y) &amp;&amp; z &amp;&amp; (a || b))
    /// {
    /// }
    /// </code>
    ///
    /// <para>or</para>
    ///
    /// <code language="csharp">
    /// if (x || (y &amp;&amp; z &amp;&amp; a) || b)
    /// {
    /// }
    /// </code>
    ///
    /// <para>Inserting parenthesis makes the code more obvious and easy to understand, and removes the need for the
    /// reader to make assumptions about the code.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1408ConditionalExpressionsMustDeclarePrecedence : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1408";
        internal const string Title = "Conditional expressions must declare precedence";
        internal const string MessageFormat = "Conditional expressions must declare precedence";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A C# statement contains a complex conditional expression which omits parenthesis around operators.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1408.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleLogicalExpression, SyntaxKind.LogicalAndExpression);
            context.RegisterSyntaxNodeAction(HandleLogicalExpression, SyntaxKind.LogicalOrExpression);
        }

        private void HandleLogicalExpression(SyntaxNodeAnalysisContext context)
        {
            BinaryExpressionSyntax binSyntax = context.Node as BinaryExpressionSyntax;

            if (binSyntax != null)
            {
                if (binSyntax.Left is BinaryExpressionSyntax)
                {
                    // Check if the operations are of the same kind

                    var left = (BinaryExpressionSyntax)binSyntax.Left;

                    if (!IsSameFamily(binSyntax.OperatorToken, left.OperatorToken))
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, left.GetLocation()));
                }
                if (binSyntax.Right is BinaryExpressionSyntax)
                {
                    // Check if the operations are of the same kind

                    var right = (BinaryExpressionSyntax)binSyntax.Right;

                    if (!IsSameFamily(binSyntax.OperatorToken, right.OperatorToken))
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, right.GetLocation()));
                }
            }
        }

        private bool IsSameFamily(SyntaxToken operatorToken1, SyntaxToken operatorToken2)
        {
            return (operatorToken1.IsKind(SyntaxKind.AmpersandAmpersandToken) && operatorToken2.IsKind(SyntaxKind.AmpersandAmpersandToken))
             || (operatorToken1.IsKind(SyntaxKind.BarBarToken) && operatorToken2.IsKind(SyntaxKind.BarBarToken));
        }
    }
}
using HiTechStore.Core;
using HiTechStore.Core.Helpers;
using HiTechStore.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace HiTechStore.Helpers.ConditionParser;

public class RoslynConditionScriptParser(IRosylnDiscountConditionMapper mapper) : IDiscountConditionScriptParser
{
    public ConditionComponent? Parse(string conditionString)
    {

        var syntaxTree = CSharpSyntaxTree.ParseText($"bool __temp = {conditionString}");

        var root = syntaxTree.GetCompilationUnitRoot();


        var assignment = root.DescendantNodes()
                              .OfType<VariableDeclaratorSyntax>()
                              .First();// __tmp = <expression>

        if (assignment.Initializer is null)
        {
            throw new Exception();
        }

        return mapper.Parse(assignment.Initializer.Value);
    }
}


public interface IRosylnDiscountConditionMapper
{
    ConditionComponent? Parse(ExpressionSyntax expression);
}

public class RoslynExpressionVisitorBase(IUnitOfWork unitOfWork) : CSharpSyntaxVisitor<ConditionComponent?>, IRosylnDiscountConditionMapper
{
    public DiscountEntity? Entity { get; set; }

    virtual protected string ResolveEntityName(string candidateName)
    {
        return candidateName;
    }

    private IEnumerable<string> GetFullPath(MemberAccessExpressionSyntax node)
    {
        var pathParts = new List<string>();
        var currentNode = (SyntaxNode)node;

        while (currentNode != null)
        {
            if (currentNode is MemberAccessExpressionSyntax memberAccess)
            {
                pathParts.Add(memberAccess.Name.Identifier.Text);
                currentNode = memberAccess.Expression;
            }
            else if (currentNode is IdentifierNameSyntax identifierName)
            {
                pathParts.Add(identifierName.Identifier.Text);
                currentNode = null;
            }
            else
            {
                return [];
            }
        }

        pathParts.Reverse();
        return pathParts;
    }


    public override ConditionComponent? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var fullPath = GetFullPath(node);

        // this assume that member access is one of:
        // 1- Elementery Property -> Product.Price
        // 2- Array Property -> Product.Variations
        // 3- Array Method -> Product.Variations.Any

        var entityName = ResolveEntityName(fullPath.First());
        var propertyName = fullPath.ElementAt(1);
        // var method = fullPath.ElementAtOrDefault(2);

        var property = unitOfWork.DiscountEntityRepository.GetPropertyByEntityAsync(entityName, propertyName).Result;

        if (property is null)
        {
            return null;
        }
        return new ConditionComponent()
        {
            Property = property,
            Type = ConditionComponentType.Value
        };

    }

    public override ConditionComponent? VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        return new ConditionComponent
        {
            Type = ConditionComponentType.Value,
            Value = node.Token.Value?.ToString()
        };
    }
    public override ConditionComponent? VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
    {
        var innerExpr = node.Expression;

        return Visit(innerExpr);
    }

    public override ConditionComponent? VisitBinaryExpression(BinaryExpressionSyntax node)
    {
        var left = Visit(node.Left);
        var right = Visit(node.Right);

        ConditionComponentType opType;
        switch (node.OperatorToken.Kind())
        {
            case SyntaxKind.EqualsEqualsToken: opType = ConditionComponentType.Equality; break;
            case SyntaxKind.ExclamationEqualsToken: opType = ConditionComponentType.NotEquality; break;
            case SyntaxKind.GreaterThanToken: opType = ConditionComponentType.GreaterThan; break;
            case SyntaxKind.GreaterThanEqualsToken: opType = ConditionComponentType.GreaterThanOrEqual; break;
            case SyntaxKind.LessThanToken: opType = ConditionComponentType.LessThan; break;
            case SyntaxKind.LessThanEqualsToken: opType = ConditionComponentType.LessThanOrEqual; break;
            case SyntaxKind.AmpersandAmpersandToken: opType = ConditionComponentType.And; break;
            case SyntaxKind.BarBarToken: opType = ConditionComponentType.Or; break;

            default: throw new NotSupportedException("Operator not supported");
        }

        return new ConditionComponent
        {
            Type = opType,
            SubConditions = new List<ConditionComponent> { left!, right! }
        };
    }

    public override ConditionComponent? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        var expression = node.Expression;
        if (expression is not MemberAccessExpressionSyntax)
        {
            throw new Exception();
        }


        var memberAccess = (MemberAccessExpressionSyntax)expression;

        string methodName = memberAccess.Name.Identifier.Text;
        ConditionComponent? ownerComponent = Visit(memberAccess.Expression);

        var methodEntity = unitOfWork.DiscountEntityRepository.GetConditionMethodByNameAsync(methodName).Result;

        if (methodEntity is null)
        {
            throw new NotSupportedException($"Used method named {methodName} not allowed");
        }

        ConditionComponent? lambdaBody = default;

        foreach (var argSyntax in node.ArgumentList.Arguments)
        {
            if (argSyntax.Expression is SimpleLambdaExpressionSyntax lambdaSyntax)
            {
                lambdaBody = MapLambda(lambdaSyntax, ownerComponent!.Property!.SubEntity!);
                break;
            }
            else if (argSyntax.Expression is ParenthesizedLambdaExpressionSyntax parenthesizedLambdaSyntax)
            {
                lambdaBody = MapLambda(parenthesizedLambdaSyntax, ownerComponent!.Property!.SubEntity!);
                break;
            }
        }

        var methodCondition = new ConditionLambda
        {
            Method = methodEntity,
            Body = lambdaBody
        };

        var component = new ConditionComponent
        {
            Type = ConditionComponentType.Method,
            Lambda = methodCondition,
            Property = ownerComponent!.Property
        };

        return component;
    }

    private ConditionComponent? MapLambda(ParenthesizedLambdaExpressionSyntax node, DiscountEntity entity)
    {
        var parameterName = node.ParameterList.Parameters.FirstOrDefault()?.Identifier.Text ?? "x";

        return new RoslynLambdaParserWithContext(unitOfWork, parameterName, entity, node.Body).Parse();
    }

    private ConditionComponent? MapLambda(SimpleLambdaExpressionSyntax node, DiscountEntity entity)
    {
        var parameterName = node.Parameter.Identifier.Text ?? "x";

        return new RoslynLambdaParserWithContext(unitOfWork, parameterName, entity, node.Body).Parse();
    }

    public ConditionComponent? Parse(ExpressionSyntax expression)
    {
        return Visit(expression);
    }
}

public class RoslynLambdaParserWithContext : RoslynExpressionVisitorBase
{
    protected string ParamName { get; set; }
    protected DiscountEntity DiscountEntity { get; set; }
    protected CSharpSyntaxNode Expression { get; set; }

    public RoslynLambdaParserWithContext(IUnitOfWork unitOfWork, string paramName, DiscountEntity paramType, CSharpSyntaxNode expression) : base(unitOfWork)
    {
        ParamName = paramName;
        DiscountEntity = paramType;
        Expression = expression;
    }

    public ConditionComponent? Parse()
    {
        return Visit(Expression);
    }

    protected override string ResolveEntityName(string candidateName)
    {
        return DiscountEntity.Name ?? base.ResolveEntityName(candidateName);
    }
}
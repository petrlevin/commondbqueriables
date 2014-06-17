// Type: System.Linq.Expressions.MemberExpression
// Assembly: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Core.dll

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime;

namespace System.Linq.Expressions
{
  /// <summary>
  /// Represents accessing a field or property.
  /// </summary>
  [DebuggerTypeProxy(typeof (Expression.MemberExpressionProxy))]
  [__DynamicallyInvokable]
  public class MemberExpression : Expression
  {
    private readonly Expression _expression;

    /// <summary>
    /// Gets the field or property to be accessed.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Reflection.MemberInfo"/> that represents the field or property to be accessed.
    /// </returns>
    [__DynamicallyInvokable]
    public MemberInfo Member
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.GetMember();
      }
    }

    /// <summary>
    /// Gets the containing object of the field or property.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.Expression"/> that represents the containing object of the field or property.
    /// </returns>
    [__DynamicallyInvokable]
    public Expression Expression
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._expression;
      }
    }

    /// <summary>
    /// Returns the node type of this <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Linq.Expressions.ExpressionType"/> that represents this expression.
    /// </returns>
    [__DynamicallyInvokable]
    public override sealed ExpressionType NodeType
    {
      [__DynamicallyInvokable] get
      {
        return ExpressionType.MemberAccess;
      }
    }

    internal MemberExpression(Expression expression)
    {
      this._expression = expression;
    }

    internal virtual MemberInfo GetMember()
    {
      throw ContractUtils.Unreachable;
    }

    /// <summary>
    /// Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression"/> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The result of visiting this node.
    /// </returns>
    /// <param name="visitor">The visitor to visit this node with.</param>
    [__DynamicallyInvokable]
    protected internal override Expression Accept(ExpressionVisitor visitor)
    {
      return visitor.VisitMember(this);
    }

    /// <summary>
    /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
    /// </summary>
    /// 
    /// <returns>
    /// This expression if no children are changed or an expression with the updated children.
    /// </returns>
    /// <param name="expression">The <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> property of the result.</param>
    [__DynamicallyInvokable]
    public MemberExpression Update(Expression expression)
    {
      if (expression == this.Expression)
        return this;
      else
        return Expression.MakeMemberAccess(expression, this.Member);
    }

    internal static MemberExpression Make(Expression expression, MemberInfo member)
    {
      if (member.MemberType == MemberTypes.Field)
      {
        FieldInfo member1 = (FieldInfo) member;
        return (MemberExpression) new FieldExpression(expression, member1);
      }
      else
      {
        PropertyInfo member1 = (PropertyInfo) member;
        return (MemberExpression) new PropertyExpression(expression, member1);
      }
    }
  }
}

// Type: System.Linq.Expressions.MethodCallExpression
// Assembly: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Core.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime;

namespace System.Linq.Expressions
{
  /// <summary>
  /// Represents a call to either static or an instance method.
  /// </summary>
  [DebuggerTypeProxy(typeof (Expression.MethodCallExpressionProxy))]
  [__DynamicallyInvokable]
  public class MethodCallExpression : Expression, IArgumentProvider
  {
    private readonly MethodInfo _method;

    /// <summary>
    /// Returns the node type of this <see cref="T:System.Linq.Expressions.Expression"/>.
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
        return ExpressionType.Call;
      }
    }

    /// <summary>
    /// Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression"/> represents.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="P:System.Linq.Expressions.MethodCallExpression.Type"/> that represents the static type of the expression.
    /// </returns>
    [__DynamicallyInvokable]
    public override sealed Type Type
    {
      [__DynamicallyInvokable] get
      {
        return this._method.ReturnType;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Reflection.MethodInfo"/> for the method to be called.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Reflection.MethodInfo"/> that represents the called method.
    /// </returns>
    [__DynamicallyInvokable]
    public MethodInfo Method
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._method;
      }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Linq.Expressions.Expression"/> that represents the instance for instance method calls or null for static method calls.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.Expression"/> that represents the receiving object of the method.
    /// </returns>
    [__DynamicallyInvokable]
    public Expression Object
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.GetInstance();
      }
    }

    /// <summary>
    /// Gets a collection of expressions that represent arguments of the called method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1"/> of <see cref="T:System.Linq.Expressions.Expression"/> objects which represent the arguments to the called method.
    /// </returns>
    [__DynamicallyInvokable]
    public ReadOnlyCollection<Expression> Arguments
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.GetOrMakeArguments();
      }
    }

    int IArgumentProvider.ArgumentCount
    {
      get
      {
        throw ContractUtils.Unreachable;
      }
    }

    internal MethodCallExpression(MethodInfo method)
    {
      this._method = method;
    }

    internal virtual Expression GetInstance()
    {
      return (Expression) null;
    }

    /// <summary>
    /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
    /// </summary>
    /// 
    /// <returns>
    /// This expression if no children are changed or an expression with the updated children.
    /// </returns>
    /// <param name="object">The <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> property of the result.</param><param name="arguments">The <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> property of the result.</param>
    [__DynamicallyInvokable]
    public MethodCallExpression Update(Expression @object, IEnumerable<Expression> arguments)
    {
      if (@object == this.Object && arguments == this.Arguments)
        return this;
      else
        return Expression.Call(@object, this.Method, arguments);
    }

    internal virtual ReadOnlyCollection<Expression> GetOrMakeArguments()
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
      return visitor.VisitMethodCall(this);
    }

    internal virtual MethodCallExpression Rewrite(Expression instance, IList<Expression> args)
    {
      throw ContractUtils.Unreachable;
    }

    Expression IArgumentProvider.GetArgument(int index)
    {
      throw ContractUtils.Unreachable;
    }
  }
}

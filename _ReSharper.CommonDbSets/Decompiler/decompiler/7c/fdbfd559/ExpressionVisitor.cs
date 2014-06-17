// Type: System.Linq.Expressions.ExpressionVisitor
// Assembly: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Core.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
  /// <summary>
  /// Represents a visitor or rewriter for expression trees.
  /// </summary>
  [__DynamicallyInvokable]
  public abstract class ExpressionVisitor
  {
    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Linq.Expressions.ExpressionVisitor"/>.
    /// </summary>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    protected ExpressionVisitor()
    {
    }

    /// <summary>
    /// Dispatches the expression to one of the more specialized visit methods in this class.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    public virtual Expression Visit(Expression node)
    {
      if (node != null)
        return node.Accept(this);
      else
        return (Expression) null;
    }

    /// <summary>
    /// Dispatches the list of expressions to one of the more specialized visit methods in this class.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression list, if any one of the elements were modified; otherwise, returns the original expression list.
    /// </returns>
    /// <param name="nodes">The expressions to visit.</param>
    [__DynamicallyInvokable]
    public ReadOnlyCollection<Expression> Visit(ReadOnlyCollection<Expression> nodes)
    {
      Expression[] list = (Expression[]) null;
      int index1 = 0;
      for (int count = nodes.Count; index1 < count; ++index1)
      {
        Expression expression = this.Visit(nodes[index1]);
        if (list != null)
          list[index1] = expression;
        else if (!object.ReferenceEquals((object) expression, (object) nodes[index1]))
        {
          list = new Expression[count];
          for (int index2 = 0; index2 < index1; ++index2)
            list[index2] = nodes[index2];
          list[index1] = expression;
        }
      }
      if (list == null)
        return nodes;
      else
        return (ReadOnlyCollection<Expression>) new TrueReadOnlyCollection<Expression>(list);
    }

    /// <summary>
    /// Visits all nodes in the collection using a specified element visitor.
    /// </summary>
    /// 
    /// <returns>
    /// The modified node list, if any of the elements were modified; otherwise, returns the original node list.
    /// </returns>
    /// <param name="nodes">The nodes to visit.</param><param name="elementVisitor">A delegate that visits a single element, optionally replacing it with a new element.</param><typeparam name="T">The type of the nodes.</typeparam>
    [__DynamicallyInvokable]
    public static ReadOnlyCollection<T> Visit<T>(ReadOnlyCollection<T> nodes, Func<T, T> elementVisitor)
    {
      T[] list = (T[]) null;
      int index1 = 0;
      for (int count = nodes.Count; index1 < count; ++index1)
      {
        T obj = elementVisitor(nodes[index1]);
        if (list != null)
          list[index1] = obj;
        else if (!object.ReferenceEquals((object) obj, (object) nodes[index1]))
        {
          list = new T[count];
          for (int index2 = 0; index2 < index1; ++index2)
            list[index2] = nodes[index2];
          list[index1] = obj;
        }
      }
      if (list == null)
        return nodes;
      else
        return (ReadOnlyCollection<T>) new TrueReadOnlyCollection<T>(list);
    }

    /// <summary>
    /// Visits an expression, casting the result back to the original expression type.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param><param name="callerName">The name of the calling method; used to report to report a better error message.</param><typeparam name="T">The type of the expression.</typeparam><exception cref="T:System.InvalidOperationException">The visit method for this node returned a different type.</exception>
    [__DynamicallyInvokable]
    public T VisitAndConvert<T>(T node, string callerName) where T : Expression
    {
      if ((object) node == null)
        return default (T);
      node = this.Visit((Expression) node) as T;
      if ((object) node == null)
        throw Error.MustRewriteToSameNode((object) callerName, (object) typeof (T), (object) callerName);
      else
        return node;
    }

    /// <summary>
    /// Visits an expression, casting the result back to the original expression type.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="nodes">The expression to visit.</param><param name="callerName">The name of the calling method; used to report to report a better error message.</param><typeparam name="T">The type of the expression.</typeparam><exception cref="T:System.InvalidOperationException">The visit method for this node returned a different type.</exception>
    [__DynamicallyInvokable]
    public ReadOnlyCollection<T> VisitAndConvert<T>(ReadOnlyCollection<T> nodes, string callerName) where T : Expression
    {
      T[] list = (T[]) null;
      int index1 = 0;
      for (int count = nodes.Count; index1 < count; ++index1)
      {
        T obj = this.Visit((Expression) nodes[index1]) as T;
        if ((object) obj == null)
          throw Error.MustRewriteToSameNode((object) callerName, (object) typeof (T), (object) callerName);
        if (list != null)
          list[index1] = obj;
        else if (!object.ReferenceEquals((object) obj, (object) nodes[index1]))
        {
          list = new T[count];
          for (int index2 = 0; index2 < index1; ++index2)
            list[index2] = nodes[index2];
          list[index1] = obj;
        }
      }
      if (list == null)
        return nodes;
      else
        return (ReadOnlyCollection<T>) new TrueReadOnlyCollection<T>(list);
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitBinary(BinaryExpression node)
    {
      return (Expression) ExpressionVisitor.ValidateBinary(node, node.Update(this.Visit(node.Left), this.VisitAndConvert<LambdaExpression>(node.Conversion, "VisitBinary"), this.Visit(node.Right)));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitBlock(BlockExpression node)
    {
      int expressionCount = node.ExpressionCount;
      Expression[] args = (Expression[]) null;
      for (int index = 0; index < expressionCount; ++index)
      {
        Expression expression1 = node.GetExpression(index);
        Expression expression2 = this.Visit(expression1);
        if (expression1 != expression2)
        {
          if (args == null)
            args = new Expression[expressionCount];
          args[index] = expression2;
        }
      }
      ReadOnlyCollection<ParameterExpression> variables = this.VisitAndConvert<ParameterExpression>(node.Variables, "VisitBlock");
      if (variables == node.Variables && args == null)
        return (Expression) node;
      for (int index = 0; index < expressionCount; ++index)
      {
        if (args[index] == null)
          args[index] = node.GetExpression(index);
      }
      return (Expression) node.Rewrite(variables, args);
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.ConditionalExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitConditional(ConditionalExpression node)
    {
      return (Expression) node.Update(this.Visit(node.Test), this.Visit(node.IfTrue), this.Visit(node.IfFalse));
    }

    /// <summary>
    /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitConstant(ConstantExpression node)
    {
      return (Expression) node;
    }

    /// <summary>
    /// Visits the <see cref="T:System.Linq.Expressions.DebugInfoExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitDebugInfo(DebugInfoExpression node)
    {
      return (Expression) node;
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.DynamicExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitDynamic(DynamicExpression node)
    {
      Expression[] args = this.VisitArguments((IArgumentProvider) node);
      if (args == null)
        return (Expression) node;
      else
        return (Expression) node.Rewrite(args);
    }

    /// <summary>
    /// Visits the <see cref="T:System.Linq.Expressions.DefaultExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitDefault(DefaultExpression node)
    {
      return (Expression) node;
    }

    /// <summary>
    /// Visits the children of the extension expression.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitExtension(Expression node)
    {
      return node.VisitChildren(this);
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.GotoExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitGoto(GotoExpression node)
    {
      return (Expression) node.Update(this.VisitLabelTarget(node.Target), this.Visit(node.Value));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.InvocationExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitInvocation(InvocationExpression node)
    {
      Expression lambda = this.Visit(node.Expression);
      Expression[] arguments = this.VisitArguments((IArgumentProvider) node);
      if (lambda == node.Expression && arguments == null)
        return (Expression) node;
      else
        return (Expression) node.Rewrite(lambda, arguments);
    }

    /// <summary>
    /// Visits the <see cref="T:System.Linq.Expressions.LabelTarget"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected virtual LabelTarget VisitLabelTarget(LabelTarget node)
    {
      return node;
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.LabelExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitLabel(LabelExpression node)
    {
      return (Expression) node.Update(this.VisitLabelTarget(node.Target), this.Visit(node.DefaultValue));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param><typeparam name="T">The type of the delegate.</typeparam>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitLambda<T>(Expression<T> node)
    {
      return (Expression) node.Update(this.Visit(node.Body), (IEnumerable<ParameterExpression>) this.VisitAndConvert<ParameterExpression>(node.Parameters, "VisitLambda"));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.LoopExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitLoop(LoopExpression node)
    {
      return (Expression) node.Update(this.VisitLabelTarget(node.BreakLabel), this.VisitLabelTarget(node.ContinueLabel), this.Visit(node.Body));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitMember(MemberExpression node)
    {
      return (Expression) node.Update(this.Visit(node.Expression));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.IndexExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitIndex(IndexExpression node)
    {
      Expression instance = this.Visit(node.Object);
      Expression[] arguments = this.VisitArguments((IArgumentProvider) node);
      if (instance == node.Object && arguments == null)
        return (Expression) node;
      else
        return node.Rewrite(instance, arguments);
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitMethodCall(MethodCallExpression node)
    {
      Expression instance = this.Visit(node.Object);
      Expression[] expressionArray = this.VisitArguments((IArgumentProvider) node);
      if (instance == node.Object && expressionArray == null)
        return (Expression) node;
      else
        return (Expression) node.Rewrite(instance, (IList<Expression>) expressionArray);
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitNewArray(NewArrayExpression node)
    {
      return (Expression) node.Update((IEnumerable<Expression>) this.Visit(node.Expressions));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitNew(NewExpression node)
    {
      return (Expression) node.Update((IEnumerable<Expression>) this.Visit(node.Arguments));
    }

    /// <summary>
    /// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitParameter(ParameterExpression node)
    {
      return (Expression) node;
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
    {
      return (Expression) node.Update((IEnumerable<ParameterExpression>) this.VisitAndConvert<ParameterExpression>(node.Variables, "VisitRuntimeVariables"));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchCase"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected virtual SwitchCase VisitSwitchCase(SwitchCase node)
    {
      return node.Update((IEnumerable<Expression>) this.Visit(node.TestValues), this.Visit(node.Body));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitSwitch(SwitchExpression node)
    {
      return (Expression) ExpressionVisitor.ValidateSwitch(node, node.Update(this.Visit(node.SwitchValue), (IEnumerable<SwitchCase>) ExpressionVisitor.Visit<SwitchCase>(node.Cases, new Func<SwitchCase, SwitchCase>(this.VisitSwitchCase)), this.Visit(node.DefaultBody)));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.CatchBlock"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected virtual CatchBlock VisitCatchBlock(CatchBlock node)
    {
      return node.Update(this.VisitAndConvert<ParameterExpression>(node.Variable, "VisitCatchBlock"), this.Visit(node.Filter), this.Visit(node.Body));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.TryExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitTry(TryExpression node)
    {
      return (Expression) node.Update(this.Visit(node.Body), (IEnumerable<CatchBlock>) ExpressionVisitor.Visit<CatchBlock>(node.Handlers, new Func<CatchBlock, CatchBlock>(this.VisitCatchBlock)), this.Visit(node.Finally), this.Visit(node.Fault));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.TypeBinaryExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitTypeBinary(TypeBinaryExpression node)
    {
      return (Expression) node.Update(this.Visit(node.Expression));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitUnary(UnaryExpression node)
    {
      return (Expression) ExpressionVisitor.ValidateUnary(node, node.Update(this.Visit(node.Operand)));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberInitExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitMemberInit(MemberInitExpression node)
    {
      return (Expression) node.Update(this.VisitAndConvert<NewExpression>(node.NewExpression, "VisitMemberInit"), (IEnumerable<MemberBinding>) ExpressionVisitor.Visit<MemberBinding>(node.Bindings, new Func<MemberBinding, MemberBinding>(this.VisitMemberBinding)));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.ListInitExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitListInit(ListInitExpression node)
    {
      return (Expression) node.Update(this.VisitAndConvert<NewExpression>(node.NewExpression, "VisitListInit"), (IEnumerable<ElementInit>) ExpressionVisitor.Visit<ElementInit>(node.Initializers, new Func<ElementInit, ElementInit>(this.VisitElementInit)));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.ElementInit"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected virtual ElementInit VisitElementInit(ElementInit node)
    {
      return node.Update((IEnumerable<Expression>) this.Visit(node.Arguments));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberBinding"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected virtual MemberBinding VisitMemberBinding(MemberBinding node)
    {
      switch (node.BindingType)
      {
        case MemberBindingType.Assignment:
          return (MemberBinding) this.VisitMemberAssignment((MemberAssignment) node);
        case MemberBindingType.MemberBinding:
          return (MemberBinding) this.VisitMemberMemberBinding((MemberMemberBinding) node);
        case MemberBindingType.ListBinding:
          return (MemberBinding) this.VisitMemberListBinding((MemberListBinding) node);
        default:
          throw Error.UnhandledBindingType((object) node.BindingType);
      }
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberAssignment"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
      return node.Update(this.Visit(node.Expression));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberMemberBinding"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
    {
      return node.Update((IEnumerable<MemberBinding>) ExpressionVisitor.Visit<MemberBinding>(node.Bindings, new Func<MemberBinding, MemberBinding>(this.VisitMemberBinding)));
    }

    /// <summary>
    /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberListBinding"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
    /// </returns>
    /// <param name="node">The expression to visit.</param>
    [__DynamicallyInvokable]
    protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding node)
    {
      return node.Update((IEnumerable<ElementInit>) ExpressionVisitor.Visit<ElementInit>(node.Initializers, new Func<ElementInit, ElementInit>(this.VisitElementInit)));
    }

    internal Expression[] VisitArguments(IArgumentProvider nodes)
    {
      Expression[] expressionArray = (Expression[]) null;
      int index1 = 0;
      for (int argumentCount = nodes.ArgumentCount; index1 < argumentCount; ++index1)
      {
        Expression node = nodes.GetArgument(index1);
        Expression expression = this.Visit(node);
        if (expressionArray != null)
          expressionArray[index1] = expression;
        else if (!object.ReferenceEquals((object) expression, (object) node))
        {
          expressionArray = new Expression[argumentCount];
          for (int index2 = 0; index2 < index1; ++index2)
            expressionArray[index2] = nodes.GetArgument(index2);
          expressionArray[index1] = expression;
        }
      }
      return expressionArray;
    }

    private static UnaryExpression ValidateUnary(UnaryExpression before, UnaryExpression after)
    {
      if (before != after && before.Method == (MethodInfo) null)
      {
        if (after.Method != (MethodInfo) null)
          throw Error.MustRewriteWithoutMethod((object) after.Method, (object) "VisitUnary");
        if (before.Operand != null && after.Operand != null)
          ExpressionVisitor.ValidateChildType(before.Operand.Type, after.Operand.Type, "VisitUnary");
      }
      return after;
    }

    private static BinaryExpression ValidateBinary(BinaryExpression before, BinaryExpression after)
    {
      if (before != after && before.Method == (MethodInfo) null)
      {
        if (after.Method != (MethodInfo) null)
          throw Error.MustRewriteWithoutMethod((object) after.Method, (object) "VisitBinary");
        ExpressionVisitor.ValidateChildType(before.Left.Type, after.Left.Type, "VisitBinary");
        ExpressionVisitor.ValidateChildType(before.Right.Type, after.Right.Type, "VisitBinary");
      }
      return after;
    }

    private static SwitchExpression ValidateSwitch(SwitchExpression before, SwitchExpression after)
    {
      if (before.Comparison == (MethodInfo) null && after.Comparison != (MethodInfo) null)
        throw Error.MustRewriteWithoutMethod((object) after.Comparison, (object) "VisitSwitch");
      else
        return after;
    }

    private static void ValidateChildType(Type before, Type after, string methodName)
    {
      if (before.IsValueType)
      {
        if (TypeUtils.AreEquivalent(before, after))
          return;
      }
      else if (!after.IsValueType)
        return;
      throw Error.MustRewriteChildToSameType((object) before, (object) after, (object) methodName);
    }
  }
}

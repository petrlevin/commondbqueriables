// Type: System.Linq.Expressions.Expression
// Assembly: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions.Compiler;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace System.Linq.Expressions
{
  /// <summary>
  /// Provides the base class from which the classes that represent expression tree nodes are derived. It also contains static (Shared in Visual Basic) factory methods to create the various node types. This is an abstract class.
  /// </summary>
  [__DynamicallyInvokable]
  public abstract class Expression
  {
    private static readonly CacheDict<Type, MethodInfo> _LambdaDelegateCache = new CacheDict<Type, MethodInfo>(40);
    private static volatile CacheDict<Type, Expression.LambdaFactory> _LambdaFactories;
    private static ConditionalWeakTable<Expression, Expression.ExtensionInfo> _legacyCtorSupportTable;

    /// <summary>
    /// Gets the node type of this <see cref="T:System.Linq.Expressions.Expression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// One of the <see cref="T:System.Linq.Expressions.ExpressionType"/> values.
    /// </returns>
    [__DynamicallyInvokable]
    public virtual ExpressionType NodeType
    {
      [__DynamicallyInvokable] get
      {
        Expression.ExtensionInfo extensionInfo;
        if (Expression._legacyCtorSupportTable != null && Expression._legacyCtorSupportTable.TryGetValue(this, out extensionInfo))
          return extensionInfo.NodeType;
        else
          throw System.Linq.Expressions.Error.ExtensionNodeMustOverrideProperty((object) "Expression.NodeType");
      }
    }

    /// <summary>
    /// Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression"/> represents.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Type"/> that represents the static type of the expression.
    /// </returns>
    [__DynamicallyInvokable]
    public virtual Type Type
    {
      [__DynamicallyInvokable] get
      {
        Expression.ExtensionInfo extensionInfo;
        if (Expression._legacyCtorSupportTable != null && Expression._legacyCtorSupportTable.TryGetValue(this, out extensionInfo))
          return extensionInfo.Type;
        else
          throw System.Linq.Expressions.Error.ExtensionNodeMustOverrideProperty((object) "Expression.Type");
      }
    }

    /// <summary>
    /// Indicates that the node can be reduced to a simpler node. If this returns true, Reduce() can be called to produce the reduced form.
    /// </summary>
    /// 
    /// <returns>
    /// True if the node can be reduced, otherwise false.
    /// </returns>
    [__DynamicallyInvokable]
    public virtual bool CanReduce
    {
      [__DynamicallyInvokable] get
      {
        return false;
      }
    }

    string DebugView
    {
      private get
      {
        using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.CurrentCulture))
        {
          DebugViewWriter.WriteTo(this, (TextWriter) stringWriter);
          return stringWriter.ToString();
        }
      }
    }

    static Expression()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Linq.Expressions.Expression"/> class.
    /// </summary>
    /// <param name="nodeType">The <see cref="T:System.Linq.Expressions.ExpressionType"/> to set as the node type.</param><param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type"/> of this <see cref="T:System.Linq.Expressions.Expression"/>.</param>
    [Obsolete("use a different constructor that does not take ExpressionType. Then override NodeType and Type properties to provide the values that would be specified to this constructor.")]
    protected Expression(ExpressionType nodeType, Type type)
    {
      if (Expression._legacyCtorSupportTable == null)
        Interlocked.CompareExchange<ConditionalWeakTable<Expression, Expression.ExtensionInfo>>(ref Expression._legacyCtorSupportTable, new ConditionalWeakTable<Expression, Expression.ExtensionInfo>(), (ConditionalWeakTable<Expression, Expression.ExtensionInfo>) null);
      Expression._legacyCtorSupportTable.Add(this, new Expression.ExtensionInfo(nodeType, type));
    }

    /// <summary>
    /// Constructs a new instance of <see cref="T:System.Linq.Expressions.Expression"/>.
    /// </summary>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    protected Expression()
    {
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Assign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression Assign(Expression left, Expression right)
    {
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      TypeUtils.ValidateType(left.Type);
      TypeUtils.ValidateType(right.Type);
      if (!TypeUtils.AreReferenceAssignable(left.Type, right.Type))
        throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchAssignment((object) right.Type, (object) left.Type);
      else
        return (BinaryExpression) new AssignBinaryExpression(left, right);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/>, given the left and right operands, by calling an appropriate factory method.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Linq.Expressions.BinaryExpression"/> that results from calling the appropriate factory method.
    /// </returns>
    /// <param name="binaryType">The <see cref="T:System.Linq.Expressions.ExpressionType"/> that specifies the type of binary operation.</param><param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the left operand.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the right operand.</param><exception cref="T:System.ArgumentException"><paramref name="binaryType"/> does not correspond to a binary expression node.</exception><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right)
    {
      return Expression.MakeBinary(binaryType, left, right, false, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/>, given the left operand, right operand and implementing method, by calling the appropriate factory method.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Linq.Expressions.BinaryExpression"/> that results from calling the appropriate factory method.
    /// </returns>
    /// <param name="binaryType">The <see cref="T:System.Linq.Expressions.ExpressionType"/> that specifies the type of binary operation.</param><param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the left operand.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the right operand.</param><param name="liftToNull">true to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to true; false to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to false.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that specifies the implementing method.</param><exception cref="T:System.ArgumentException"><paramref name="binaryType"/> does not correspond to a binary expression node.</exception><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method)
    {
      return Expression.MakeBinary(binaryType, left, right, liftToNull, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/>, given the left operand, right operand, implementing method and type conversion function, by calling the appropriate factory method.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Linq.Expressions.BinaryExpression"/> that results from calling the appropriate factory method.
    /// </returns>
    /// <param name="binaryType">The <see cref="T:System.Linq.Expressions.ExpressionType"/> that specifies the type of binary operation.</param><param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the left operand.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the right operand.</param><param name="liftToNull">true to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to true; false to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to false.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that specifies the implementing method.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that represents a type conversion function. This parameter is used only if <paramref name="binaryType"/> is <see cref="F:System.Linq.Expressions.ExpressionType.Coalesce"/> or compound assignment..</param><exception cref="T:System.ArgumentException"><paramref name="binaryType"/> does not correspond to a binary expression node.</exception><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method, LambdaExpression conversion)
    {
      switch (binaryType)
      {
        case ExpressionType.Add:
          return Expression.Add(left, right, method);
        case ExpressionType.AddChecked:
          return Expression.AddChecked(left, right, method);
        case ExpressionType.And:
          return Expression.And(left, right, method);
        case ExpressionType.AndAlso:
          return Expression.AndAlso(left, right, method);
        case ExpressionType.ArrayIndex:
          return Expression.ArrayIndex(left, right);
        case ExpressionType.Coalesce:
          return Expression.Coalesce(left, right, conversion);
        case ExpressionType.Divide:
          return Expression.Divide(left, right, method);
        case ExpressionType.Equal:
          return Expression.Equal(left, right, liftToNull, method);
        case ExpressionType.ExclusiveOr:
          return Expression.ExclusiveOr(left, right, method);
        case ExpressionType.GreaterThan:
          return Expression.GreaterThan(left, right, liftToNull, method);
        case ExpressionType.GreaterThanOrEqual:
          return Expression.GreaterThanOrEqual(left, right, liftToNull, method);
        case ExpressionType.LeftShift:
          return Expression.LeftShift(left, right, method);
        case ExpressionType.LessThan:
          return Expression.LessThan(left, right, liftToNull, method);
        case ExpressionType.LessThanOrEqual:
          return Expression.LessThanOrEqual(left, right, liftToNull, method);
        case ExpressionType.Modulo:
          return Expression.Modulo(left, right, method);
        case ExpressionType.Multiply:
          return Expression.Multiply(left, right, method);
        case ExpressionType.MultiplyChecked:
          return Expression.MultiplyChecked(left, right, method);
        case ExpressionType.NotEqual:
          return Expression.NotEqual(left, right, liftToNull, method);
        case ExpressionType.Or:
          return Expression.Or(left, right, method);
        case ExpressionType.OrElse:
          return Expression.OrElse(left, right, method);
        case ExpressionType.Power:
          return Expression.Power(left, right, method);
        case ExpressionType.RightShift:
          return Expression.RightShift(left, right, method);
        case ExpressionType.Subtract:
          return Expression.Subtract(left, right, method);
        case ExpressionType.SubtractChecked:
          return Expression.SubtractChecked(left, right, method);
        case ExpressionType.Assign:
          return Expression.Assign(left, right);
        case ExpressionType.AddAssign:
          return Expression.AddAssign(left, right, method, conversion);
        case ExpressionType.AndAssign:
          return Expression.AndAssign(left, right, method, conversion);
        case ExpressionType.DivideAssign:
          return Expression.DivideAssign(left, right, method, conversion);
        case ExpressionType.ExclusiveOrAssign:
          return Expression.ExclusiveOrAssign(left, right, method, conversion);
        case ExpressionType.LeftShiftAssign:
          return Expression.LeftShiftAssign(left, right, method, conversion);
        case ExpressionType.ModuloAssign:
          return Expression.ModuloAssign(left, right, method, conversion);
        case ExpressionType.MultiplyAssign:
          return Expression.MultiplyAssign(left, right, method, conversion);
        case ExpressionType.OrAssign:
          return Expression.OrAssign(left, right, method, conversion);
        case ExpressionType.PowerAssign:
          return Expression.PowerAssign(left, right, method, conversion);
        case ExpressionType.RightShiftAssign:
          return Expression.RightShiftAssign(left, right, method, conversion);
        case ExpressionType.SubtractAssign:
          return Expression.SubtractAssign(left, right, method, conversion);
        case ExpressionType.AddAssignChecked:
          return Expression.AddAssignChecked(left, right, method, conversion);
        case ExpressionType.MultiplyAssignChecked:
          return Expression.MultiplyAssignChecked(left, right, method, conversion);
        case ExpressionType.SubtractAssignChecked:
          return Expression.SubtractAssignChecked(left, right, method, conversion);
        default:
          throw System.Linq.Expressions.Error.UnhandledBinary((object) binaryType);
      }
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an equality comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Equal"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The equality operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Equal(Expression left, Expression right)
    {
      return Expression.Equal(left, right, false, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an equality comparison. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Equal"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="liftToNull">true to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to true; false to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to false.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the equality operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
        return Expression.GetEqualityComparisonOperator(ExpressionType.Equal, "op_Equality", left, right, liftToNull);
      else
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.Equal, left, right, method, liftToNull);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a reference equality comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Equal"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression ReferenceEqual(Expression left, Expression right)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (TypeUtils.HasReferenceEquality(left.Type, right.Type))
        return (BinaryExpression) new LogicalBinaryExpression(ExpressionType.Equal, left, right);
      else
        throw System.Linq.Expressions.Error.ReferenceEqualityNotDefined((object) left.Type, (object) right.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an inequality comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NotEqual"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The inequality operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression NotEqual(Expression left, Expression right)
    {
      return Expression.NotEqual(left, right, false, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an inequality comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NotEqual"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="liftToNull">true to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to true; false to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to false.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the inequality operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
        return Expression.GetEqualityComparisonOperator(ExpressionType.NotEqual, "op_Inequality", left, right, liftToNull);
      else
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.NotEqual, left, right, method, liftToNull);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a reference inequality comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NotEqual"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression ReferenceNotEqual(Expression left, Expression right)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (TypeUtils.HasReferenceEquality(left.Type, right.Type))
        return (BinaryExpression) new LogicalBinaryExpression(ExpressionType.NotEqual, left, right);
      else
        throw System.Linq.Expressions.Error.ReferenceEqualityNotDefined((object) left.Type, (object) right.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a "greater than" numeric comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.GreaterThan"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The "greater than" operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression GreaterThan(Expression left, Expression right)
    {
      return Expression.GreaterThan(left, right, false, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a "greater than" numeric comparison. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.GreaterThan"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="liftToNull">true to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to true; false to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to false.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the "greater than" operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
        return Expression.GetComparisonOperator(ExpressionType.GreaterThan, "op_GreaterThan", left, right, liftToNull);
      else
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.GreaterThan, left, right, method, liftToNull);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a "less than" numeric comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LessThan"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The "less than" operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression LessThan(Expression left, Expression right)
    {
      return Expression.LessThan(left, right, false, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a "less than" numeric comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LessThan"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="liftToNull">true to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to true; false to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to false.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the "less than" operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
        return Expression.GetComparisonOperator(ExpressionType.LessThan, "op_LessThan", left, right, liftToNull);
      else
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.LessThan, left, right, method, liftToNull);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a "greater than or equal" numeric comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.GreaterThanOrEqual"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The "greater than or equal" operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right)
    {
      return Expression.GreaterThanOrEqual(left, right, false, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a "greater than or equal" numeric comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.GreaterThanOrEqual"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="liftToNull">true to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to true; false to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to false.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the "greater than or equal" operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
        return Expression.GetComparisonOperator(ExpressionType.GreaterThanOrEqual, "op_GreaterThanOrEqual", left, right, liftToNull);
      else
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.GreaterThanOrEqual, left, right, method, liftToNull);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a " less than or equal" numeric comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LessThanOrEqual"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The "less than or equal" operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression LessThanOrEqual(Expression left, Expression right)
    {
      return Expression.LessThanOrEqual(left, right, false, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a "less than or equal" numeric comparison.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LessThanOrEqual"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="liftToNull">true to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to true; false to set <see cref="P:System.Linq.Expressions.BinaryExpression.IsLiftedToNull"/> to false.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the "less than or equal" operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
        return Expression.GetComparisonOperator(ExpressionType.LessThanOrEqual, "op_LessThanOrEqual", left, right, liftToNull);
      else
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.LessThanOrEqual, left, right, method, liftToNull);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a conditional AND operation that evaluates the second operand only if the first operand evaluates to true.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAlso"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The bitwise AND operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.-or-<paramref name="left"/>.Type and <paramref name="right"/>.Type are not the same Boolean type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression AndAlso(Expression left, Expression right)
    {
      return Expression.AndAlso(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a conditional AND operation that evaluates the second operand only if the first operand is resolved to true. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAlso"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the bitwise AND operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.-or-<paramref name="method"/> is null and <paramref name="left"/>.Type and <paramref name="right"/>.Type are not the same Boolean type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
      {
        if (left.Type == right.Type)
        {
          if (left.Type == typeof (bool))
            return (BinaryExpression) new LogicalBinaryExpression(ExpressionType.AndAlso, left, right);
          if (left.Type == typeof (bool?))
            return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.AndAlso, left, right, left.Type);
        }
        method = Expression.GetUserDefinedBinaryOperator(ExpressionType.AndAlso, left.Type, right.Type, "op_BitwiseAnd");
        if (!(method != (MethodInfo) null))
          throw System.Linq.Expressions.Error.BinaryOperatorNotDefined((object) ExpressionType.AndAlso, (object) left.Type, (object) right.Type);
        Expression.ValidateUserDefinedConditionalLogicOperator(ExpressionType.AndAlso, left.Type, right.Type, method);
        Type type = !TypeUtils.IsNullableType(left.Type) || !TypeUtils.AreEquivalent(method.ReturnType, TypeUtils.GetNonNullableType(left.Type)) ? method.ReturnType : left.Type;
        return (BinaryExpression) new MethodBinaryExpression(ExpressionType.AndAlso, left, right, type, method);
      }
      else
      {
        Expression.ValidateUserDefinedConditionalLogicOperator(ExpressionType.AndAlso, left.Type, right.Type, method);
        Type type = !TypeUtils.IsNullableType(left.Type) || !TypeUtils.AreEquivalent(method.ReturnType, TypeUtils.GetNonNullableType(left.Type)) ? method.ReturnType : left.Type;
        return (BinaryExpression) new MethodBinaryExpression(ExpressionType.AndAlso, left, right, type, method);
      }
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a conditional OR operation that evaluates the second operand only if the first operand evaluates to false.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrElse"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The bitwise OR operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.-or-<paramref name="left"/>.Type and <paramref name="right"/>.Type are not the same Boolean type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression OrElse(Expression left, Expression right)
    {
      return Expression.OrElse(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a conditional OR operation that evaluates the second operand only if the first operand evaluates to false.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrElse"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the bitwise OR operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.-or-<paramref name="method"/> is null and <paramref name="left"/>.Type and <paramref name="right"/>.Type are not the same Boolean type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
      {
        if (left.Type == right.Type)
        {
          if (left.Type == typeof (bool))
            return (BinaryExpression) new LogicalBinaryExpression(ExpressionType.OrElse, left, right);
          if (left.Type == typeof (bool?))
            return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.OrElse, left, right, left.Type);
        }
        method = Expression.GetUserDefinedBinaryOperator(ExpressionType.OrElse, left.Type, right.Type, "op_BitwiseOr");
        if (!(method != (MethodInfo) null))
          throw System.Linq.Expressions.Error.BinaryOperatorNotDefined((object) ExpressionType.OrElse, (object) left.Type, (object) right.Type);
        Expression.ValidateUserDefinedConditionalLogicOperator(ExpressionType.OrElse, left.Type, right.Type, method);
        Type type = !TypeUtils.IsNullableType(left.Type) || !(method.ReturnType == TypeUtils.GetNonNullableType(left.Type)) ? method.ReturnType : left.Type;
        return (BinaryExpression) new MethodBinaryExpression(ExpressionType.OrElse, left, right, type, method);
      }
      else
      {
        Expression.ValidateUserDefinedConditionalLogicOperator(ExpressionType.OrElse, left.Type, right.Type, method);
        Type type = !TypeUtils.IsNullableType(left.Type) || !(method.ReturnType == TypeUtils.GetNonNullableType(left.Type)) ? method.ReturnType : left.Type;
        return (BinaryExpression) new MethodBinaryExpression(ExpressionType.OrElse, left, right, type, method);
      }
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a coalescing operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Coalesce"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of <paramref name="left"/> does not represent a reference type or a nullable value type.</exception><exception cref="T:System.ArgumentException"><paramref name="left"/>.Type and <paramref name="right"/>.Type are not convertible to each other.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Coalesce(Expression left, Expression right)
    {
      return Expression.Coalesce(left, right, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a coalescing operation, given a conversion function.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Coalesce"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="left"/>.Type and <paramref name="right"/>.Type are not convertible to each other.-or-<paramref name="conversion"/> is not null and <paramref name="conversion"/>.Type is a delegate type that does not take exactly one argument.</exception><exception cref="T:System.InvalidOperationException">The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of <paramref name="left"/> does not represent a reference type or a nullable value type.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of <paramref name="left"/> represents a type that is not assignable to the parameter type of the delegate type <paramref name="conversion"/>.Type.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of <paramref name="right"/> is not equal to the return type of the delegate type <paramref name="conversion"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Coalesce(Expression left, Expression right, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (conversion == null)
      {
        Type type = Expression.ValidateCoalesceArgTypes(left.Type, right.Type);
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.Coalesce, left, right, type);
      }
      else
      {
        if (left.Type.IsValueType && !TypeUtils.IsNullableType(left.Type))
          throw System.Linq.Expressions.Error.CoalesceUsedOnNonNullType();
        MethodInfo method = conversion.Type.GetMethod("Invoke");
        if (method.ReturnType == typeof (void))
          throw System.Linq.Expressions.Error.UserDefinedOperatorMustNotBeVoid((object) conversion);
        ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) method);
        if (parametersCached.Length != 1)
          throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) conversion);
        if (!TypeUtils.AreEquivalent(method.ReturnType, right.Type))
          throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) ExpressionType.Coalesce, (object) conversion.ToString());
        if (!Expression.ParameterIsAssignable(parametersCached[0], TypeUtils.GetNonNullableType(left.Type)) && !Expression.ParameterIsAssignable(parametersCached[0], left.Type))
          throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) ExpressionType.Coalesce, (object) conversion.ToString());
        else
          return (BinaryExpression) new CoalesceConversionBinaryExpression(left, right, conversion);
      }
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic addition operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Add"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The addition operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Add(Expression left, Expression right)
    {
      return Expression.Add(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic addition operation that does not have overflow checking. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Add"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the addition operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Add(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.Add, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.Add, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Add, "op_Addition", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an addition assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression AddAssign(Expression left, Expression right)
    {
      return Expression.AddAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an addition assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression AddAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.AddAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an addition assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression AddAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.AddAssign, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsArithmetic(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.AddAssign, "op_Addition", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.AddAssign, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an addition assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression AddAssignChecked(Expression left, Expression right)
    {
      return Expression.AddAssignChecked(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an addition assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression AddAssignChecked(Expression left, Expression right, MethodInfo method)
    {
      return Expression.AddAssignChecked(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an addition assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression AddAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.AddAssignChecked, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsArithmetic(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.AddAssignChecked, "op_Addition", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.AddAssignChecked, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic addition operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The addition operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression AddChecked(Expression left, Expression right)
    {
      return Expression.AddChecked(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic addition operation that has overflow checking. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AddChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the addition operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.AddChecked, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.AddChecked, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.AddChecked, "op_Addition", left, right, false);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic subtraction operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Subtract"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The subtraction operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Subtract(Expression left, Expression right)
    {
      return Expression.Subtract(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic subtraction operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Subtract"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the subtraction operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.Subtract, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.Subtract, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Subtract, "op_Subtraction", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a subtraction assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression SubtractAssign(Expression left, Expression right)
    {
      return Expression.SubtractAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a subtraction assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression SubtractAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.SubtractAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a subtraction assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression SubtractAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.SubtractAssign, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsArithmetic(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.SubtractAssign, "op_Subtraction", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.SubtractAssign, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a subtraction assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression SubtractAssignChecked(Expression left, Expression right)
    {
      return Expression.SubtractAssignChecked(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a subtraction assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method)
    {
      return Expression.SubtractAssignChecked(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a subtraction assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.SubtractAssignChecked, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsArithmetic(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.SubtractAssignChecked, "op_Subtraction", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.SubtractAssignChecked, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic subtraction operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The subtraction operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression SubtractChecked(Expression left, Expression right)
    {
      return Expression.SubtractChecked(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic subtraction operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.SubtractChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the subtraction operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.SubtractChecked, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.SubtractChecked, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.SubtractChecked, "op_Subtraction", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic division operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Divide"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The division operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Divide(Expression left, Expression right)
    {
      return Expression.Divide(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic division operation. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Divide"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the division operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.Divide, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.Divide, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Divide, "op_Division", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a division assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.DivideAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression DivideAssign(Expression left, Expression right)
    {
      return Expression.DivideAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a division assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.DivideAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression DivideAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.DivideAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a division assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.DivideAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression DivideAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.DivideAssign, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsArithmetic(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.DivideAssign, "op_Division", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.DivideAssign, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic remainder operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Modulo"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The modulus operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Modulo(Expression left, Expression right)
    {
      return Expression.Modulo(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic remainder operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Modulo"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the modulus operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.Modulo, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.Modulo, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Modulo, "op_Modulus", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a remainder assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ModuloAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression ModuloAssign(Expression left, Expression right)
    {
      return Expression.ModuloAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a remainder assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ModuloAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression ModuloAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.ModuloAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a remainder assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ModuloAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression ModuloAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.ModuloAssign, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsArithmetic(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.ModuloAssign, "op_Modulus", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.ModuloAssign, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic multiplication operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Multiply"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The multiplication operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Multiply(Expression left, Expression right)
    {
      return Expression.Multiply(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic multiplication operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Multiply"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the multiplication operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.Multiply, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.Multiply, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Multiply, "op_Multiply", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a multiplication assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression MultiplyAssign(Expression left, Expression right)
    {
      return Expression.MultiplyAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a multiplication assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression MultiplyAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.MultiplyAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a multiplication assignment operation that does not have overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression MultiplyAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.MultiplyAssign, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsArithmetic(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.MultiplyAssign, "op_Multiply", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.MultiplyAssign, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a multiplication assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right)
    {
      return Expression.MultiplyAssignChecked(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a multiplication assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method)
    {
      return Expression.MultiplyAssignChecked(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a multiplication assignment operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyAssignChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.MultiplyAssignChecked, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsArithmetic(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.MultiplyAssignChecked, "op_Multiply", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.MultiplyAssignChecked, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic multiplication operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The multiplication operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression MultiplyChecked(Expression left, Expression right)
    {
      return Expression.MultiplyChecked(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents an arithmetic multiplication operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MultiplyChecked"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the multiplication operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.MultiplyChecked, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.MultiplyChecked, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.MultiplyChecked, "op_Multiply", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise left-shift operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShift"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The left-shift operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression LeftShift(Expression left, Expression right)
    {
      return Expression.LeftShift(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise left-shift operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShift"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the left-shift operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.LeftShift, left, right, method, true);
      if (!Expression.IsSimpleShift(left.Type, right.Type))
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.LeftShift, "op_LeftShift", left, right, true);
      Type resultTypeOfShift = Expression.GetResultTypeOfShift(left.Type, right.Type);
      return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.LeftShift, left, right, resultTypeOfShift);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise left-shift assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShiftAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression LeftShiftAssign(Expression left, Expression right)
    {
      return Expression.LeftShiftAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise left-shift assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShiftAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.LeftShiftAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise left-shift assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.LeftShiftAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.LeftShiftAssign, left, right, method, conversion, true);
      if (!Expression.IsSimpleShift(left.Type, right.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.LeftShiftAssign, "op_LeftShift", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      Type resultTypeOfShift = Expression.GetResultTypeOfShift(left.Type, right.Type);
      return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.LeftShiftAssign, left, right, resultTypeOfShift);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise right-shift operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShift"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The right-shift operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression RightShift(Expression left, Expression right)
    {
      return Expression.RightShift(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise right-shift operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShift"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the right-shift operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.RightShift, left, right, method, true);
      if (!Expression.IsSimpleShift(left.Type, right.Type))
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.RightShift, "op_RightShift", left, right, true);
      Type resultTypeOfShift = Expression.GetResultTypeOfShift(left.Type, right.Type);
      return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.RightShift, left, right, resultTypeOfShift);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise right-shift assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShiftAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression RightShiftAssign(Expression left, Expression right)
    {
      return Expression.RightShiftAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise right-shift assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShiftAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression RightShiftAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.RightShiftAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise right-shift assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RightShiftAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression RightShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.RightShiftAssign, left, right, method, conversion, true);
      if (!Expression.IsSimpleShift(left.Type, right.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.RightShiftAssign, "op_RightShift", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      Type resultTypeOfShift = Expression.GetResultTypeOfShift(left.Type, right.Type);
      return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.RightShiftAssign, left, right, resultTypeOfShift);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise AND operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.And"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The bitwise AND operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression And(Expression left, Expression right)
    {
      return Expression.And(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise AND operation. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.And"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the bitwise AND operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression And(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.And, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.And, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.And, "op_BitwiseAnd", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise AND assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression AndAssign(Expression left, Expression right)
    {
      return Expression.AndAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise AND assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression AndAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.AndAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise AND assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.AndAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression AndAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.AndAssign, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsIntegerOrBool(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.AndAssign, "op_BitwiseAnd", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.AndAssign, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise OR operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Or"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The bitwise OR operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Or(Expression left, Expression right)
    {
      return Expression.Or(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise OR operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Or"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the bitwise OR operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Or(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.Or, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.Or, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Or, "op_BitwiseOr", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise OR assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression OrAssign(Expression left, Expression right)
    {
      return Expression.OrAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise OR assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression OrAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.OrAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise OR assignment operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.OrAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression OrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.OrAssign, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsIntegerOrBool(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.OrAssign, "op_BitwiseOr", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.OrAssign, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise XOR operation, using op_ExclusiveOr for user-defined types.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOr"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The XOR operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression ExclusiveOr(Expression left, Expression right)
    {
      return Expression.ExclusiveOr(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise XOR operation, using op_ExclusiveOr for user-defined types. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOr"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the XOR operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedBinaryOperator(ExpressionType.ExclusiveOr, left, right, method, true);
      if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.ExclusiveOr, left, right, left.Type);
      else
        return Expression.GetUserDefinedBinaryOperatorOrThrow(ExpressionType.ExclusiveOr, "op_ExclusiveOr", left, right, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise XOR assignment operation, using op_ExclusiveOr for user-defined types.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOrAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right)
    {
      return Expression.ExclusiveOrAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise XOR assignment operation, using op_ExclusiveOr for user-defined types.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOrAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.ExclusiveOrAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents a bitwise XOR assignment operation, using op_ExclusiveOr for user-defined types.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ExclusiveOrAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedAssignOperator(ExpressionType.ExclusiveOrAssign, left, right, method, conversion, true);
      if (!(left.Type == right.Type) || !TypeUtils.IsIntegerOrBool(left.Type))
        return Expression.GetUserDefinedAssignOperatorOrThrow(ExpressionType.ExclusiveOrAssign, "op_ExclusiveOr", left, right, conversion, true);
      if (conversion != null)
        throw System.Linq.Expressions.Error.ConversionIsNotSupportedForArithmeticTypes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.ExclusiveOrAssign, left, right, left.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents raising a number to a power.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Power"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.InvalidOperationException">The exponentiation operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.-or-<paramref name="left"/>.Type and/or <paramref name="right"/>.Type are not <see cref="T:System.Double"/>.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression Power(Expression left, Expression right)
    {
      return Expression.Power(left, right, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents raising a number to a power.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Power"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="left"/> or <paramref name="right"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly two arguments.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the exponentiation operator is not defined for <paramref name="left"/>.Type and <paramref name="right"/>.Type.-or-<paramref name="method"/> is null and <paramref name="left"/>.Type and/or <paramref name="right"/>.Type are not <see cref="T:System.Double"/>.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression Power(Expression left, Expression right, MethodInfo method)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
      {
        method = typeof (Math).GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
        if (method == (MethodInfo) null)
          throw System.Linq.Expressions.Error.BinaryOperatorNotDefined((object) ExpressionType.Power, (object) left.Type, (object) right.Type);
      }
      return Expression.GetMethodBasedBinaryOperator(ExpressionType.Power, left, right, method, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents raising an expression to a power and assigning the result back to the expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.PowerAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression PowerAssign(Expression left, Expression right)
    {
      return Expression.PowerAssign(left, right, (MethodInfo) null, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents raising an expression to a power and assigning the result back to the expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.PowerAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BinaryExpression PowerAssign(Expression left, Expression right, MethodInfo method)
    {
      return Expression.PowerAssign(left, right, method, (LambdaExpression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents raising an expression to a power and assigning the result back to the expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.PowerAssign"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/>, <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/>, and <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> properties set to the specified values.
    /// </returns>
    /// <param name="left">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="right">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Method"/> property equal to.</param><param name="conversion">A <see cref="T:System.Linq.Expressions.LambdaExpression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static BinaryExpression PowerAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
    {
      Expression.RequiresCanRead(left, "left");
      Expression.RequiresCanWrite(left, "left");
      Expression.RequiresCanRead(right, "right");
      if (method == (MethodInfo) null)
      {
        method = typeof (Math).GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
        if (method == (MethodInfo) null)
          throw System.Linq.Expressions.Error.BinaryOperatorNotDefined((object) ExpressionType.PowerAssign, (object) left.Type, (object) right.Type);
      }
      return Expression.GetMethodBasedAssignOperator(ExpressionType.PowerAssign, left, right, method, conversion, true);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BinaryExpression"/> that represents applying an array index operator to an array of rank one.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.BinaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ArrayIndex"/> and the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> and <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> properties set to the specified values.
    /// </returns>
    /// <param name="array">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Left"/> property equal to.</param><param name="index">A <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.BinaryExpression.Right"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> or <paramref name="index"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/>.Type does not represent an array type.-or-<paramref name="array"/>.Type represents an array type whose rank is not 1.-or-<paramref name="index"/>.Type does not represent the <see cref="T:System.Int32"/> type.</exception>
    [__DynamicallyInvokable]
    public static BinaryExpression ArrayIndex(Expression array, Expression index)
    {
      Expression.RequiresCanRead(array, "array");
      Expression.RequiresCanRead(index, "index");
      if (index.Type != typeof (int))
        throw System.Linq.Expressions.Error.ArgumentMustBeArrayIndexType();
      Type type = array.Type;
      if (!type.IsArray)
        throw System.Linq.Expressions.Error.ArgumentMustBeArray();
      if (type.GetArrayRank() != 1)
        throw System.Linq.Expressions.Error.IncorrectNumberOfIndexes();
      else
        return (BinaryExpression) new SimpleBinaryExpression(ExpressionType.ArrayIndex, array, index, type.GetElementType());
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains two expressions and has no variables.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="arg0">The first expression in the block.</param><param name="arg1">The second expression in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(Expression arg0, Expression arg1)
    {
      Expression.RequiresCanRead(arg0, "arg0");
      Expression.RequiresCanRead(arg1, "arg1");
      return (BlockExpression) new Block2(arg0, arg1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains three expressions and has no variables.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="arg0">The first expression in the block.</param><param name="arg1">The second expression in the block.</param><param name="arg2">The third expression in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(Expression arg0, Expression arg1, Expression arg2)
    {
      Expression.RequiresCanRead(arg0, "arg0");
      Expression.RequiresCanRead(arg1, "arg1");
      Expression.RequiresCanRead(arg2, "arg2");
      return (BlockExpression) new Block3(arg0, arg1, arg2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains four expressions and has no variables.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="arg0">The first expression in the block.</param><param name="arg1">The second expression in the block.</param><param name="arg2">The third expression in the block.</param><param name="arg3">The fourth expression in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(Expression arg0, Expression arg1, Expression arg2, Expression arg3)
    {
      Expression.RequiresCanRead(arg0, "arg0");
      Expression.RequiresCanRead(arg1, "arg1");
      Expression.RequiresCanRead(arg2, "arg2");
      Expression.RequiresCanRead(arg3, "arg3");
      return (BlockExpression) new Block4(arg0, arg1, arg2, arg3);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains five expressions and has no variables.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="arg0">The first expression in the block.</param><param name="arg1">The second expression in the block.</param><param name="arg2">The third expression in the block.</param><param name="arg3">The fourth expression in the block.</param><param name="arg4">The fifth expression in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
    {
      Expression.RequiresCanRead(arg0, "arg0");
      Expression.RequiresCanRead(arg1, "arg1");
      Expression.RequiresCanRead(arg2, "arg2");
      Expression.RequiresCanRead(arg3, "arg3");
      Expression.RequiresCanRead(arg4, "arg4");
      return (BlockExpression) new Block5(arg0, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains the given expressions and has no variables.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="expressions">The expressions in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(params Expression[] expressions)
    {
      ContractUtils.RequiresNotNull((object) expressions, "expressions");
      switch (expressions.Length)
      {
        case 2:
          return Expression.Block(expressions[0], expressions[1]);
        case 3:
          return Expression.Block(expressions[0], expressions[1], expressions[2]);
        case 4:
          return Expression.Block(expressions[0], expressions[1], expressions[2], expressions[3]);
        case 5:
          return Expression.Block(expressions[0], expressions[1], expressions[2], expressions[3], expressions[4]);
        default:
          ContractUtils.RequiresNotEmpty<Expression>((ICollection<Expression>) expressions, "expressions");
          Expression.RequiresCanRead((IEnumerable<Expression>) expressions, "expressions");
          return (BlockExpression) new BlockN((IList<Expression>) CollectionExtensions.Copy<Expression>(expressions));
      }
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains the given expressions and has no variables.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="expressions">The expressions in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(IEnumerable<Expression> expressions)
    {
      return Expression.Block((IEnumerable<ParameterExpression>) EmptyReadOnlyCollection<ParameterExpression>.Instance, expressions);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains the given expressions, has no variables and has specific result type.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="type">The result type of the block.</param><param name="expressions">The expressions in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(Type type, params Expression[] expressions)
    {
      ContractUtils.RequiresNotNull((object) expressions, "expressions");
      return Expression.Block(type, (IEnumerable<Expression>) expressions);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains the given expressions, has no variables and has specific result type.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="type">The result type of the block.</param><param name="expressions">The expressions in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(Type type, IEnumerable<Expression> expressions)
    {
      return Expression.Block(type, (IEnumerable<ParameterExpression>) EmptyReadOnlyCollection<ParameterExpression>.Instance, expressions);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains the given variables and expressions.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="variables">The variables in the block.</param><param name="expressions">The expressions in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(IEnumerable<ParameterExpression> variables, params Expression[] expressions)
    {
      return Expression.Block(variables, (IEnumerable<Expression>) expressions);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains the given variables and expressions.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="type">The result type of the block.</param><param name="variables">The variables in the block.</param><param name="expressions">The expressions in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(Type type, IEnumerable<ParameterExpression> variables, params Expression[] expressions)
    {
      return Expression.Block(type, variables, (IEnumerable<Expression>) expressions);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains the given variables and expressions.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="variables">The variables in the block.</param><param name="expressions">The expressions in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions)
    {
      ContractUtils.RequiresNotNull((object) expressions, "expressions");
      ReadOnlyCollection<Expression> readOnlyCollection = CollectionExtensions.ToReadOnly<Expression>(expressions);
      ContractUtils.RequiresNotEmpty<Expression>((ICollection<Expression>) readOnlyCollection, "expressions");
      Expression.RequiresCanRead((IEnumerable<Expression>) readOnlyCollection, "expressions");
      return Expression.Block(Enumerable.Last<Expression>((IEnumerable<Expression>) readOnlyCollection).Type, variables, (IEnumerable<Expression>) readOnlyCollection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.BlockExpression"/> that contains the given variables and expressions.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.BlockExpression"/>.
    /// </returns>
    /// <param name="type">The result type of the block.</param><param name="variables">The variables in the block.</param><param name="expressions">The expressions in the block.</param>
    [__DynamicallyInvokable]
    public static BlockExpression Block(Type type, IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      ContractUtils.RequiresNotNull((object) expressions, "expressions");
      ReadOnlyCollection<Expression> readOnlyCollection = CollectionExtensions.ToReadOnly<Expression>(expressions);
      ReadOnlyCollection<ParameterExpression> varList = CollectionExtensions.ToReadOnly<ParameterExpression>(variables);
      ContractUtils.RequiresNotEmpty<Expression>((ICollection<Expression>) readOnlyCollection, "expressions");
      Expression.RequiresCanRead((IEnumerable<Expression>) readOnlyCollection, "expressions");
      Expression.ValidateVariables(varList, "variables");
      Expression expression = Enumerable.Last<Expression>((IEnumerable<Expression>) readOnlyCollection);
      if (type != typeof (void) && !TypeUtils.AreReferenceAssignable(type, expression.Type))
        throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
      if (!TypeUtils.AreEquivalent(type, expression.Type))
        return (BlockExpression) new ScopeWithType((IList<ParameterExpression>) varList, (IList<Expression>) readOnlyCollection, type);
      if (readOnlyCollection.Count == 1)
        return (BlockExpression) new Scope1((IList<ParameterExpression>) varList, readOnlyCollection[0]);
      else
        return (BlockExpression) new ScopeN((IList<ParameterExpression>) varList, (IList<Expression>) readOnlyCollection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.CatchBlock"/> representing a catch statement.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.CatchBlock"/>.
    /// </returns>
    /// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type"/> of <see cref="T:System.Exception"/> this <see cref="T:System.Linq.Expressions.CatchBlock"/> will handle.</param><param name="body">The body of the catch statement.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static CatchBlock Catch(Type type, Expression body)
    {
      return Expression.MakeCatchBlock(type, (ParameterExpression) null, body, (Expression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.CatchBlock"/> representing a catch statement with a reference to the caught <see cref="T:System.Exception"/> object for use in the handler body.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.CatchBlock"/>.
    /// </returns>
    /// <param name="variable">A <see cref="T:System.Linq.Expressions.ParameterExpression"/> representing a reference to the <see cref="T:System.Exception"/> object caught by this handler.</param><param name="body">The body of the catch statement.</param>
    [__DynamicallyInvokable]
    public static CatchBlock Catch(ParameterExpression variable, Expression body)
    {
      ContractUtils.RequiresNotNull((object) variable, "variable");
      return Expression.MakeCatchBlock(variable.Type, variable, body, (Expression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.CatchBlock"/> representing a catch statement with an <see cref="T:System.Exception"/> filter but no reference to the caught <see cref="T:System.Exception"/> object.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.CatchBlock"/>.
    /// </returns>
    /// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type"/> of <see cref="T:System.Exception"/> this <see cref="T:System.Linq.Expressions.CatchBlock"/> will handle.</param><param name="body">The body of the catch statement.</param><param name="filter">The body of the <see cref="T:System.Exception"/> filter.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static CatchBlock Catch(Type type, Expression body, Expression filter)
    {
      return Expression.MakeCatchBlock(type, (ParameterExpression) null, body, filter);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.CatchBlock"/> representing a catch statement with an <see cref="T:System.Exception"/> filter and a reference to the caught <see cref="T:System.Exception"/> object.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.CatchBlock"/>.
    /// </returns>
    /// <param name="variable">A <see cref="T:System.Linq.Expressions.ParameterExpression"/> representing a reference to the <see cref="T:System.Exception"/> object caught by this handler.</param><param name="body">The body of the catch statement.</param><param name="filter">The body of the <see cref="T:System.Exception"/> filter.</param>
    [__DynamicallyInvokable]
    public static CatchBlock Catch(ParameterExpression variable, Expression body, Expression filter)
    {
      ContractUtils.RequiresNotNull((object) variable, "variable");
      return Expression.MakeCatchBlock(variable.Type, variable, body, filter);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.CatchBlock"/> representing a catch statement with the specified elements.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.CatchBlock"/>.
    /// </returns>
    /// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type"/> of <see cref="T:System.Exception"/> this <see cref="T:System.Linq.Expressions.CatchBlock"/> will handle.</param><param name="variable">A <see cref="T:System.Linq.Expressions.ParameterExpression"/> representing a reference to the <see cref="T:System.Exception"/> object caught by this handler.</param><param name="body">The body of the catch statement.</param><param name="filter">The body of the <see cref="T:System.Exception"/> filter.</param>
    [__DynamicallyInvokable]
    public static CatchBlock MakeCatchBlock(Type type, ParameterExpression variable, Expression body, Expression filter)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      ContractUtils.Requires(variable == null || TypeUtils.AreEquivalent(variable.Type, type), "variable");
      if (variable != null && variable.IsByRef)
        throw System.Linq.Expressions.Error.VariableMustNotBeByRef((object) variable, (object) variable.Type);
      Expression.RequiresCanRead(body, "body");
      if (filter != null)
      {
        Expression.RequiresCanRead(filter, "filter");
        if (filter.Type != typeof (bool))
          throw System.Linq.Expressions.Error.ArgumentMustBeBoolean();
      }
      return new CatchBlock(type, variable, body, filter);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ConditionalExpression"/> that represents a conditional statement.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ConditionalExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Conditional"/> and the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test"/>, <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue"/>, and <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse"/> properties set to the specified values.
    /// </returns>
    /// <param name="test">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test"/> property equal to.</param><param name="ifTrue">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue"/> property equal to.</param><param name="ifFalse">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="test"/> or <paramref name="ifTrue"/> or <paramref name="ifFalse"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="test"/>.Type is not <see cref="T:System.Boolean"/>.-or-<paramref name="ifTrue"/>.Type is not equal to <paramref name="ifFalse"/>.Type.</exception>
    [__DynamicallyInvokable]
    public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
    {
      Expression.RequiresCanRead(test, "test");
      Expression.RequiresCanRead(ifTrue, "ifTrue");
      Expression.RequiresCanRead(ifFalse, "ifFalse");
      if (test.Type != typeof (bool))
        throw System.Linq.Expressions.Error.ArgumentMustBeBoolean();
      if (!TypeUtils.AreEquivalent(ifTrue.Type, ifFalse.Type))
        throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
      else
        return ConditionalExpression.Make(test, ifTrue, ifFalse, ifTrue.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ConditionalExpression"/> that represents a conditional statement.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ConditionalExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Conditional"/> and the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test"/>, <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue"/>, and <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse"/> properties set to the specified values.
    /// </returns>
    /// <param name="test">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test"/> property equal to.</param><param name="ifTrue">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue"/> property equal to.</param><param name="ifFalse">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse"/> property equal to.</param><param name="type">A <see cref="P:System.Linq.Expressions.Expression.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse, Type type)
    {
      Expression.RequiresCanRead(test, "test");
      Expression.RequiresCanRead(ifTrue, "ifTrue");
      Expression.RequiresCanRead(ifFalse, "ifFalse");
      ContractUtils.RequiresNotNull((object) type, "type");
      if (test.Type != typeof (bool))
        throw System.Linq.Expressions.Error.ArgumentMustBeBoolean();
      if (type != typeof (void) && (!TypeUtils.AreReferenceAssignable(type, ifTrue.Type) || !TypeUtils.AreReferenceAssignable(type, ifFalse.Type)))
        throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
      else
        return ConditionalExpression.Make(test, ifTrue, ifFalse, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ConditionalExpression"/> that represents a conditional block with an if statement.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ConditionalExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Conditional"/> and the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test"/>, <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue"/>, properties set to the specified values. The <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse"/> property is set to default expression and the type of the resulting <see cref="T:System.Linq.Expressions.ConditionalExpression"/> returned by this method is <see cref="T:System.Void"/>.
    /// </returns>
    /// <param name="test">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test"/> property equal to.</param><param name="ifTrue">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static ConditionalExpression IfThen(Expression test, Expression ifTrue)
    {
      return Expression.Condition(test, ifTrue, (Expression) Expression.Empty(), typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ConditionalExpression"/> that represents a conditional block with if and else statements.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ConditionalExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Conditional"/> and the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test"/>, <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue"/>, and <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse"/> properties set to the specified values. The type of the resulting <see cref="T:System.Linq.Expressions.ConditionalExpression"/> returned by this method is <see cref="T:System.Void"/>.
    /// </returns>
    /// <param name="test">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.Test"/> property equal to.</param><param name="ifTrue">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfTrue"/> property equal to.</param><param name="ifFalse">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.ConditionalExpression.IfFalse"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static ConditionalExpression IfThenElse(Expression test, Expression ifTrue, Expression ifFalse)
    {
      return Expression.Condition(test, ifTrue, ifFalse, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ConstantExpression"/> that has the <see cref="P:System.Linq.Expressions.ConstantExpression.Value"/> property set to the specified value.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ConstantExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Constant"/> and the <see cref="P:System.Linq.Expressions.ConstantExpression.Value"/> property set to the specified value.
    /// </returns>
    /// <param name="value">An <see cref="T:System.Object"/> to set the <see cref="P:System.Linq.Expressions.ConstantExpression.Value"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static ConstantExpression Constant(object value)
    {
      return ConstantExpression.Make(value, value == null ? typeof (object) : value.GetType());
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ConstantExpression"/> that has the <see cref="P:System.Linq.Expressions.ConstantExpression.Value"/> and <see cref="P:System.Linq.Expressions.Expression.Type"/> properties set to the specified values.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ConstantExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Constant"/> and the <see cref="P:System.Linq.Expressions.ConstantExpression.Value"/> and <see cref="P:System.Linq.Expressions.Expression.Type"/> properties set to the specified values.
    /// </returns>
    /// <param name="value">An <see cref="T:System.Object"/> to set the <see cref="P:System.Linq.Expressions.ConstantExpression.Value"/> property equal to.</param><param name="type">A <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="value"/> is not null and <paramref name="type"/> is not assignable from the dynamic type of <paramref name="value"/>.</exception>
    [__DynamicallyInvokable]
    public static ConstantExpression Constant(object value, Type type)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      if (value == null && type.IsValueType && !TypeUtils.IsNullableType(type))
        throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
      if (value != null && !type.IsAssignableFrom(value.GetType()))
        throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
      else
        return ConstantExpression.Make(value, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DebugInfoExpression"/> with the specified span.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.DebugInfoExpression"/>.
    /// </returns>
    /// <param name="document">The <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/> that represents the source file.</param><param name="startLine">The start line of this <see cref="T:System.Linq.Expressions.DebugInfoExpression"/>. Must be greater than 0.</param><param name="startColumn">The start column of this <see cref="T:System.Linq.Expressions.DebugInfoExpression"/>. Must be greater than 0.</param><param name="endLine">The end line of this <see cref="T:System.Linq.Expressions.DebugInfoExpression"/>. Must be greater or equal than the start line.</param><param name="endColumn">The end column of this <see cref="T:System.Linq.Expressions.DebugInfoExpression"/>. If the end line is the same as the start line, it must be greater or equal than the start column. In any case, must be greater than 0.</param>
    [__DynamicallyInvokable]
    public static DebugInfoExpression DebugInfo(SymbolDocumentInfo document, int startLine, int startColumn, int endLine, int endColumn)
    {
      ContractUtils.RequiresNotNull((object) document, "document");
      if (startLine == 16707566 && startColumn == 0 && (endLine == 16707566 && endColumn == 0))
        return (DebugInfoExpression) new ClearDebugInfoExpression(document);
      Expression.ValidateSpan(startLine, startColumn, endLine, endColumn);
      return (DebugInfoExpression) new SpanDebugInfoExpression(document, startLine, startColumn, endLine, endColumn);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DebugInfoExpression"/> for clearing a sequence point.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.DebugInfoExpression"/> for clearning a sequence point.
    /// </returns>
    /// <param name="document">The <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/> that represents the source file.</param>
    [__DynamicallyInvokable]
    public static DebugInfoExpression ClearDebugInfo(SymbolDocumentInfo document)
    {
      ContractUtils.RequiresNotNull((object) document, "document");
      return (DebugInfoExpression) new ClearDebugInfoExpression(document);
    }

    /// <summary>
    /// Creates an empty expression that has <see cref="T:System.Void"/> type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DefaultExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Default"/> and the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <see cref="T:System.Void"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public static DefaultExpression Empty()
    {
      return new DefaultExpression(typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DefaultExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to the specified type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DefaultExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Default"/> and the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to the specified type.
    /// </returns>
    /// <param name="type">A <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static DefaultExpression Default(Type type)
    {
      if (type == typeof (void))
        return Expression.Empty();
      else
        return new DefaultExpression(type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType"/>, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/>, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite"/>.</param><param name="binder">The runtime binder for the dynamic operation.</param><param name="arguments">The arguments to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, params Expression[] arguments)
    {
      return Expression.MakeDynamic(delegateType, binder, (IEnumerable<Expression>) arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType"/>, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/>, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite"/>.</param><param name="binder">The runtime binder for the dynamic operation.</param><param name="arguments">The arguments to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, IEnumerable<Expression> arguments)
    {
      ContractUtils.RequiresNotNull((object) delegateType, "delegateType");
      ContractUtils.RequiresNotNull((object) binder, "binder");
      if (!delegateType.IsSubclassOf(typeof (MulticastDelegate)))
        throw System.Linq.Expressions.Error.TypeMustBeDerivedFromSystemDelegate();
      MethodInfo methodForDynamic = Expression.GetValidMethodForDynamic(delegateType);
      ReadOnlyCollection<Expression> arguments1 = CollectionExtensions.ToReadOnly<Expression>(arguments);
      Expression.ValidateArgumentTypes((MethodBase) methodForDynamic, ExpressionType.Dynamic, ref arguments1);
      return DynamicExpression.Make(TypeExtensions.GetReturnType((MethodBase) methodForDynamic), delegateType, binder, arguments1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/> and one argument.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType"/>, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/>, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite"/>.</param><param name="binder">The runtime binder for the dynamic operation.</param><param name="arg0">The argument to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0)
    {
      ContractUtils.RequiresNotNull((object) delegateType, "delegateType");
      ContractUtils.RequiresNotNull((object) binder, "binder");
      if (!delegateType.IsSubclassOf(typeof (MulticastDelegate)))
        throw System.Linq.Expressions.Error.TypeMustBeDerivedFromSystemDelegate();
      MethodInfo methodForDynamic = Expression.GetValidMethodForDynamic(delegateType);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) methodForDynamic);
      Expression.ValidateArgumentCount((MethodBase) methodForDynamic, ExpressionType.Dynamic, 2, parametersCached);
      Expression.ValidateDynamicArgument(arg0);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg0, parametersCached[1]);
      return DynamicExpression.Make(TypeExtensions.GetReturnType((MethodBase) methodForDynamic), delegateType, binder, arg0);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/> and two arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType"/>, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/>, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite"/>.</param><param name="binder">The runtime binder for the dynamic operation.</param><param name="arg0">The first argument to the dynamic operation.</param><param name="arg1">The second argument to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
    {
      ContractUtils.RequiresNotNull((object) delegateType, "delegateType");
      ContractUtils.RequiresNotNull((object) binder, "binder");
      if (!delegateType.IsSubclassOf(typeof (MulticastDelegate)))
        throw System.Linq.Expressions.Error.TypeMustBeDerivedFromSystemDelegate();
      MethodInfo methodForDynamic = Expression.GetValidMethodForDynamic(delegateType);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) methodForDynamic);
      Expression.ValidateArgumentCount((MethodBase) methodForDynamic, ExpressionType.Dynamic, 3, parametersCached);
      Expression.ValidateDynamicArgument(arg0);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg0, parametersCached[1]);
      Expression.ValidateDynamicArgument(arg1);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg1, parametersCached[2]);
      return DynamicExpression.Make(TypeExtensions.GetReturnType((MethodBase) methodForDynamic), delegateType, binder, arg0, arg1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/> and three arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType"/>, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/>, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite"/>.</param><param name="binder">The runtime binder for the dynamic operation.</param><param name="arg0">The first argument to the dynamic operation.</param><param name="arg1">The second argument to the dynamic operation.</param><param name="arg2">The third argument to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
    {
      ContractUtils.RequiresNotNull((object) delegateType, "delegateType");
      ContractUtils.RequiresNotNull((object) binder, "binder");
      if (!delegateType.IsSubclassOf(typeof (MulticastDelegate)))
        throw System.Linq.Expressions.Error.TypeMustBeDerivedFromSystemDelegate();
      MethodInfo methodForDynamic = Expression.GetValidMethodForDynamic(delegateType);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) methodForDynamic);
      Expression.ValidateArgumentCount((MethodBase) methodForDynamic, ExpressionType.Dynamic, 4, parametersCached);
      Expression.ValidateDynamicArgument(arg0);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg0, parametersCached[1]);
      Expression.ValidateDynamicArgument(arg1);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg1, parametersCached[2]);
      Expression.ValidateDynamicArgument(arg2);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg2, parametersCached[3]);
      return DynamicExpression.Make(TypeExtensions.GetReturnType((MethodBase) methodForDynamic), delegateType, binder, arg0, arg1, arg2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/> and four arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.DelegateType"/>, <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/>, and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="delegateType">The type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite"/>.</param><param name="binder">The runtime binder for the dynamic operation.</param><param name="arg0">The first argument to the dynamic operation.</param><param name="arg1">The second argument to the dynamic operation.</param><param name="arg2">The third argument to the dynamic operation.</param><param name="arg3">The fourth argument to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
    {
      ContractUtils.RequiresNotNull((object) delegateType, "delegateType");
      ContractUtils.RequiresNotNull((object) binder, "binder");
      if (!delegateType.IsSubclassOf(typeof (MulticastDelegate)))
        throw System.Linq.Expressions.Error.TypeMustBeDerivedFromSystemDelegate();
      MethodInfo methodForDynamic = Expression.GetValidMethodForDynamic(delegateType);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) methodForDynamic);
      Expression.ValidateArgumentCount((MethodBase) methodForDynamic, ExpressionType.Dynamic, 5, parametersCached);
      Expression.ValidateDynamicArgument(arg0);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg0, parametersCached[1]);
      Expression.ValidateDynamicArgument(arg1);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg1, parametersCached[2]);
      Expression.ValidateDynamicArgument(arg2);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg2, parametersCached[3]);
      Expression.ValidateDynamicArgument(arg3);
      Expression.ValidateOneArgument((MethodBase) methodForDynamic, ExpressionType.Dynamic, arg3, parametersCached[4]);
      return DynamicExpression.Make(TypeExtensions.GetReturnType((MethodBase) methodForDynamic), delegateType, binder, arg0, arg1, arg2, arg3);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="binder">The runtime binder for the dynamic operation.</param><param name="returnType">The result type of the dynamic expression.</param><param name="arguments">The arguments to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, params Expression[] arguments)
    {
      return Expression.Dynamic(binder, returnType, (IEnumerable<Expression>) arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="binder">The runtime binder for the dynamic operation.</param><param name="returnType">The result type of the dynamic expression.</param><param name="arg0">The first argument to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0)
    {
      ContractUtils.RequiresNotNull((object) binder, "binder");
      Expression.ValidateDynamicArgument(arg0);
      DelegateHelpers.TypeInfo nextTypeInfo = DelegateHelpers.GetNextTypeInfo(returnType, DelegateHelpers.GetNextTypeInfo(arg0.Type, DelegateHelpers.NextTypeInfo(typeof (CallSite))));
      Type delegateType = nextTypeInfo.DelegateType;
      if (delegateType == (Type) null)
        delegateType = nextTypeInfo.MakeDelegateType(returnType, new Expression[1]
        {
          arg0
        });
      return DynamicExpression.Make(returnType, delegateType, binder, arg0);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="binder">The runtime binder for the dynamic operation.</param><param name="returnType">The result type of the dynamic expression.</param><param name="arg0">The first argument to the dynamic operation.</param><param name="arg1">The second argument to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1)
    {
      ContractUtils.RequiresNotNull((object) binder, "binder");
      Expression.ValidateDynamicArgument(arg0);
      Expression.ValidateDynamicArgument(arg1);
      DelegateHelpers.TypeInfo nextTypeInfo = DelegateHelpers.GetNextTypeInfo(returnType, DelegateHelpers.GetNextTypeInfo(arg1.Type, DelegateHelpers.GetNextTypeInfo(arg0.Type, DelegateHelpers.NextTypeInfo(typeof (CallSite)))));
      Type delegateType = nextTypeInfo.DelegateType;
      if (delegateType == (Type) null)
        delegateType = nextTypeInfo.MakeDelegateType(returnType, arg0, arg1);
      return DynamicExpression.Make(returnType, delegateType, binder, arg0, arg1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="binder">The runtime binder for the dynamic operation.</param><param name="returnType">The result type of the dynamic expression.</param><param name="arg0">The first argument to the dynamic operation.</param><param name="arg1">The second argument to the dynamic operation.</param><param name="arg2">The third argument to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2)
    {
      ContractUtils.RequiresNotNull((object) binder, "binder");
      Expression.ValidateDynamicArgument(arg0);
      Expression.ValidateDynamicArgument(arg1);
      Expression.ValidateDynamicArgument(arg2);
      DelegateHelpers.TypeInfo nextTypeInfo = DelegateHelpers.GetNextTypeInfo(returnType, DelegateHelpers.GetNextTypeInfo(arg2.Type, DelegateHelpers.GetNextTypeInfo(arg1.Type, DelegateHelpers.GetNextTypeInfo(arg0.Type, DelegateHelpers.NextTypeInfo(typeof (CallSite))))));
      Type delegateType = nextTypeInfo.DelegateType;
      if (delegateType == (Type) null)
        delegateType = nextTypeInfo.MakeDelegateType(returnType, arg0, arg1, arg2);
      return DynamicExpression.Make(returnType, delegateType, binder, arg0, arg1, arg2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="binder">The runtime binder for the dynamic operation.</param><param name="returnType">The result type of the dynamic expression.</param><param name="arg0">The first argument to the dynamic operation.</param><param name="arg1">The second argument to the dynamic operation.</param><param name="arg2">The third argument to the dynamic operation.</param><param name="arg3">The fourth argument to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
    {
      ContractUtils.RequiresNotNull((object) binder, "binder");
      Expression.ValidateDynamicArgument(arg0);
      Expression.ValidateDynamicArgument(arg1);
      Expression.ValidateDynamicArgument(arg2);
      Expression.ValidateDynamicArgument(arg3);
      DelegateHelpers.TypeInfo nextTypeInfo = DelegateHelpers.GetNextTypeInfo(returnType, DelegateHelpers.GetNextTypeInfo(arg3.Type, DelegateHelpers.GetNextTypeInfo(arg2.Type, DelegateHelpers.GetNextTypeInfo(arg1.Type, DelegateHelpers.GetNextTypeInfo(arg0.Type, DelegateHelpers.NextTypeInfo(typeof (CallSite)))))));
      Type delegateType = nextTypeInfo.DelegateType;
      if (delegateType == (Type) null)
        delegateType = nextTypeInfo.MakeDelegateType(returnType, arg0, arg1, arg2, arg3);
      return DynamicExpression.Make(returnType, delegateType, binder, arg0, arg1, arg2, arg3);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.DynamicExpression"/> that represents a dynamic operation bound by the provided <see cref="T:System.Runtime.CompilerServices.CallSiteBinder"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.DynamicExpression"/> that has <see cref="P:System.Linq.Expressions.Expression.NodeType"/> equal to <see cref="F:System.Linq.Expressions.ExpressionType.Dynamic"/> and has the <see cref="P:System.Linq.Expressions.DynamicExpression.Binder"/> and <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments"/> set to the specified values.
    /// </returns>
    /// <param name="binder">The runtime binder for the dynamic operation.</param><param name="returnType">The result type of the dynamic expression.</param><param name="arguments">The arguments to the dynamic operation.</param>
    [__DynamicallyInvokable]
    public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, IEnumerable<Expression> arguments)
    {
      ContractUtils.RequiresNotNull((object) arguments, "arguments");
      ContractUtils.RequiresNotNull((object) returnType, "returnType");
      ReadOnlyCollection<Expression> args = CollectionExtensions.ToReadOnly<Expression>(arguments);
      ContractUtils.RequiresNotEmpty<Expression>((ICollection<Expression>) args, "args");
      return Expression.MakeDynamic(binder, returnType, args);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.ElementInit"/>, given an array of values as the second argument.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.ElementInit"/> that has the <see cref="P:System.Linq.Expressions.ElementInit.AddMethod"/> and <see cref="P:System.Linq.Expressions.ElementInit.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="addMethod">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.ElementInit.AddMethod"/> property equal to.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to set the <see cref="P:System.Linq.Expressions.ElementInit.Arguments"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="addMethod"/> or <paramref name="arguments"/> is null.</exception><exception cref="T:System.ArgumentException">The method that addMethod represents is not named "Add" (case insensitive).-or-The method that addMethod represents is not an instance method.-or-arguments does not contain the same number of elements as the number of parameters for the method that addMethod represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of one or more elements of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the method that <paramref name="addMethod"/> represents.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static ElementInit ElementInit(MethodInfo addMethod, params Expression[] arguments)
    {
      return Expression.ElementInit(addMethod, (IEnumerable<Expression>) arguments);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.ElementInit"/>, given an <see cref="T:System.Collections.Generic.IEnumerable`1"/> as the second argument.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.ElementInit"/> that has the <see cref="P:System.Linq.Expressions.ElementInit.AddMethod"/> and <see cref="P:System.Linq.Expressions.ElementInit.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="addMethod">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.ElementInit.AddMethod"/> property equal to.</param><param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to set the <see cref="P:System.Linq.Expressions.ElementInit.Arguments"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="addMethod"/> or <paramref name="arguments"/> is null.</exception><exception cref="T:System.ArgumentException">The method that <paramref name="addMethod"/> represents is not named "Add" (case insensitive).-or-The method that <paramref name="addMethod"/> represents is not an instance method.-or-<paramref name="arguments"/> does not contain the same number of elements as the number of parameters for the method that <paramref name="addMethod"/> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of one or more elements of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the method that <paramref name="addMethod"/> represents.</exception>
    [__DynamicallyInvokable]
    public static ElementInit ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
    {
      ContractUtils.RequiresNotNull((object) addMethod, "addMethod");
      ContractUtils.RequiresNotNull((object) arguments, "arguments");
      ReadOnlyCollection<Expression> arguments1 = CollectionExtensions.ToReadOnly<Expression>(arguments);
      Expression.RequiresCanRead((IEnumerable<Expression>) arguments1, "arguments");
      Expression.ValidateElementInitAddMethodInfo(addMethod);
      Expression.ValidateArgumentTypes((MethodBase) addMethod, ExpressionType.Call, ref arguments1);
      return new ElementInit(addMethod, arguments1);
    }

    /// <summary>
    /// Reduces this node to a simpler expression. If CanReduce returns true, this should return a valid expression. This method can return another node which itself must be reduced.
    /// </summary>
    /// 
    /// <returns>
    /// The reduced expression.
    /// </returns>
    [__DynamicallyInvokable]
    public virtual Expression Reduce()
    {
      if (this.CanReduce)
        throw System.Linq.Expressions.Error.ReducibleMustOverrideReduce();
      else
        return this;
    }

    /// <summary>
    /// Reduces the node and then calls the visitor delegate on the reduced expression. The method throws an exception if the node is not reducible.
    /// </summary>
    /// 
    /// <returns>
    /// The expression being visited, or an expression which should replace it in the tree.
    /// </returns>
    /// <param name="visitor">An instance of <see cref="T:System.Func`2"/>.</param>
    [__DynamicallyInvokable]
    protected internal virtual Expression VisitChildren(ExpressionVisitor visitor)
    {
      if (!this.CanReduce)
        throw System.Linq.Expressions.Error.MustBeReducible();
      else
        return visitor.Visit(this.ReduceAndCheck());
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
    protected internal virtual Expression Accept(ExpressionVisitor visitor)
    {
      return visitor.VisitExtension(this);
    }

    /// <summary>
    /// Reduces this node to a simpler expression. If CanReduce returns true, this should return a valid expression. This method can return another node which itself must be reduced.
    /// </summary>
    /// 
    /// <returns>
    /// The reduced expression.
    /// </returns>
    [__DynamicallyInvokable]
    public Expression ReduceAndCheck()
    {
      if (!this.CanReduce)
        throw System.Linq.Expressions.Error.MustBeReducible();
      Expression expression = this.Reduce();
      if (expression == null || expression == this)
        throw System.Linq.Expressions.Error.MustReduceToDifferent();
      if (!TypeUtils.AreReferenceAssignable(this.Type, expression.Type))
        throw System.Linq.Expressions.Error.ReducedNotCompatible();
      else
        return expression;
    }

    /// <summary>
    /// Reduces the expression to a known node type (that is not an Extension node) or just returns the expression if it is already a known type.
    /// </summary>
    /// 
    /// <returns>
    /// The reduced expression.
    /// </returns>
    [__DynamicallyInvokable]
    public Expression ReduceExtensions()
    {
      Expression expression = this;
      while (expression.NodeType == ExpressionType.Extension)
        expression = expression.ReduceAndCheck();
      return expression;
    }

    /// <summary>
    /// Returns a textual representation of the <see cref="T:System.Linq.Expressions.Expression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A textual representation of the <see cref="T:System.Linq.Expressions.Expression"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public override string ToString()
    {
      return ExpressionStringBuilder.ExpressionToString(this);
    }

    internal static ReadOnlyCollection<T> ReturnReadOnly<T>(ref IList<T> collection)
    {
      IList<T> comparand = collection;
      ReadOnlyCollection<T> readOnlyCollection = comparand as ReadOnlyCollection<T>;
      if (readOnlyCollection != null)
        return readOnlyCollection;
      Interlocked.CompareExchange<IList<T>>(ref collection, (IList<T>) CollectionExtensions.ToReadOnly<T>((IEnumerable<T>) comparand), comparand);
      return (ReadOnlyCollection<T>) collection;
    }

    internal static T ReturnObject<T>(object collectionOrT) where T : class
    {
      T obj = collectionOrT as T;
      if ((object) obj != null)
        return obj;
      else
        return ((ReadOnlyCollection<T>) collectionOrT)[0];
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a break statement.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Break, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, and a null value to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param>
    [__DynamicallyInvokable]
    public static GotoExpression Break(LabelTarget target)
    {
      return Expression.MakeGoto(GotoExpressionKind.Break, target, (Expression) null, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a break statement. The value passed to the label upon jumping can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Break, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, and <paramref name="value"/> to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="value">The value that will be passed to the associated label upon jumping.</param>
    [__DynamicallyInvokable]
    public static GotoExpression Break(LabelTarget target, Expression value)
    {
      return Expression.MakeGoto(GotoExpressionKind.Break, target, value, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a break statement with the specified type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Break, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, and the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <paramref name="type"/>.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="type">An <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static GotoExpression Break(LabelTarget target, Type type)
    {
      return Expression.MakeGoto(GotoExpressionKind.Break, target, (Expression) null, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a break statement with the specified type. The value passed to the label upon jumping can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Break, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <paramref name="type"/>, and <paramref name="value"/> to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="value">The value that will be passed to the associated label upon jumping.</param><param name="type">An <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static GotoExpression Break(LabelTarget target, Expression value, Type type)
    {
      return Expression.MakeGoto(GotoExpressionKind.Break, target, value, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a continue statement.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Continue, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, and a null value to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param>
    [__DynamicallyInvokable]
    public static GotoExpression Continue(LabelTarget target)
    {
      return Expression.MakeGoto(GotoExpressionKind.Continue, target, (Expression) null, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a continue statement with the specified type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Continue, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <paramref name="type"/>, and a null value to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="type">An <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static GotoExpression Continue(LabelTarget target, Type type)
    {
      return Expression.MakeGoto(GotoExpressionKind.Continue, target, (Expression) null, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a return statement.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Return, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, and a null value to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param>
    [__DynamicallyInvokable]
    public static GotoExpression Return(LabelTarget target)
    {
      return Expression.MakeGoto(GotoExpressionKind.Return, target, (Expression) null, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a return statement with the specified type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Return, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <paramref name="type"/>, and a null value to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="type">An <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static GotoExpression Return(LabelTarget target, Type type)
    {
      return Expression.MakeGoto(GotoExpressionKind.Return, target, (Expression) null, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a return statement. The value passed to the label upon jumping can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Continue, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, and <paramref name="value"/> to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="value">The value that will be passed to the associated label upon jumping.</param>
    [__DynamicallyInvokable]
    public static GotoExpression Return(LabelTarget target, Expression value)
    {
      return Expression.MakeGoto(GotoExpressionKind.Return, target, value, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a return statement with the specified type. The value passed to the label upon jumping can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Continue, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <paramref name="type"/>, and <paramref name="value"/> to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="value">The value that will be passed to the associated label upon jumping.</param><param name="type">An <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static GotoExpression Return(LabelTarget target, Expression value, Type type)
    {
      return Expression.MakeGoto(GotoExpressionKind.Return, target, value, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a "go to" statement.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Goto, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to the specified value, and a null value to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param>
    [__DynamicallyInvokable]
    public static GotoExpression Goto(LabelTarget target)
    {
      return Expression.MakeGoto(GotoExpressionKind.Goto, target, (Expression) null, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a "go to" statement with the specified type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Goto, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to the specified value, the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <paramref name="type"/>, and a null value to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="type">An <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static GotoExpression Goto(LabelTarget target, Type type)
    {
      return Expression.MakeGoto(GotoExpressionKind.Goto, target, (Expression) null, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a "go to" statement. The value passed to the label upon jumping can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Goto, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, and <paramref name="value"/> to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="value">The value that will be passed to the associated label upon jumping.</param>
    [__DynamicallyInvokable]
    public static GotoExpression Goto(LabelTarget target, Expression value)
    {
      return Expression.MakeGoto(GotoExpressionKind.Goto, target, value, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a "go to" statement with the specified type. The value passed to the label upon jumping can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to Goto, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <paramref name="type"/>, and <paramref name="value"/> to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="value">The value that will be passed to the associated label upon jumping.</param><param name="type">An <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static GotoExpression Goto(LabelTarget target, Expression value, Type type)
    {
      return Expression.MakeGoto(GotoExpressionKind.Goto, target, value, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.GotoExpression"/> representing a jump of the specified <see cref="T:System.Linq.Expressions.GotoExpressionKind"/>. The value passed to the label upon jumping can also be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.GotoExpression"/> with <see cref="P:System.Linq.Expressions.GotoExpression.Kind"/> equal to <paramref name="kind"/>, the <see cref="P:System.Linq.Expressions.GotoExpression.Target"/> property set to <paramref name="target"/>, the <see cref="P:System.Linq.Expressions.Expression.Type"/> property set to <paramref name="type"/>, and <paramref name="value"/> to be passed to the target label upon jumping.
    /// </returns>
    /// <param name="kind">The <see cref="T:System.Linq.Expressions.GotoExpressionKind"/> of the <see cref="T:System.Linq.Expressions.GotoExpression"/>.</param><param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> that the <see cref="T:System.Linq.Expressions.GotoExpression"/> will jump to.</param><param name="value">The value that will be passed to the associated label upon jumping.</param><param name="type">An <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static GotoExpression MakeGoto(GotoExpressionKind kind, LabelTarget target, Expression value, Type type)
    {
      Expression.ValidateGoto(target, ref value, "target", "value");
      return new GotoExpression(kind, target, value, type);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.IndexExpression"/> that represents accessing an indexed property in an object.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.IndexExpression"/>.
    /// </returns>
    /// <param name="instance">The object to which the property belongs. It should be null if the property is static (shared in Visual Basic).</param><param name="indexer">An <see cref="T:System.Linq.Expressions.Expression"/> representing the property to index.</param><param name="arguments">An IEnumerable&lt;Expression&gt; (IEnumerable (Of Expression) in Visual Basic) that contains the arguments that will be used to index the property.</param>
    [__DynamicallyInvokable]
    public static IndexExpression MakeIndex(Expression instance, PropertyInfo indexer, IEnumerable<Expression> arguments)
    {
      if (indexer != (PropertyInfo) null)
        return Expression.Property(instance, indexer, arguments);
      else
        return Expression.ArrayAccess(instance, arguments);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.IndexExpression"/> to access an array.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.IndexExpression"/>.
    /// </returns>
    /// <param name="array">An expression representing the array to index.</param><param name="indexes">An array that contains expressions used to index the array.</param>
    [__DynamicallyInvokable]
    public static IndexExpression ArrayAccess(Expression array, params Expression[] indexes)
    {
      return Expression.ArrayAccess(array, (IEnumerable<Expression>) indexes);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.IndexExpression"/> to access a multidimensional array.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.IndexExpression"/>.
    /// </returns>
    /// <param name="array">An expression that represents the multidimensional array.</param><param name="indexes">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing expressions used to index the array.</param>
    [__DynamicallyInvokable]
    public static IndexExpression ArrayAccess(Expression array, IEnumerable<Expression> indexes)
    {
      Expression.RequiresCanRead(array, "array");
      Type type = array.Type;
      if (!type.IsArray)
        throw System.Linq.Expressions.Error.ArgumentMustBeArray();
      ReadOnlyCollection<Expression> readOnlyCollection = CollectionExtensions.ToReadOnly<Expression>(indexes);
      if (type.GetArrayRank() != readOnlyCollection.Count)
        throw System.Linq.Expressions.Error.IncorrectNumberOfIndexes();
      foreach (Expression expression in readOnlyCollection)
      {
        Expression.RequiresCanRead(expression, "indexes");
        if (expression.Type != typeof (int))
          throw System.Linq.Expressions.Error.ArgumentMustBeArrayIndexType();
      }
      return new IndexExpression(array, (PropertyInfo) null, (IList<Expression>) readOnlyCollection);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.IndexExpression"/> representing the access to an indexed property.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.IndexExpression"/>.
    /// </returns>
    /// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param><param name="propertyName">The name of the indexer.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects that are used to index the property.</param>
    [__DynamicallyInvokable]
    public static IndexExpression Property(Expression instance, string propertyName, params Expression[] arguments)
    {
      Expression.RequiresCanRead(instance, "instance");
      ContractUtils.RequiresNotNull((object) propertyName, "indexerName");
      PropertyInfo instanceProperty = Expression.FindInstanceProperty(instance.Type, propertyName, arguments);
      return Expression.Property(instance, instanceProperty, arguments);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.IndexExpression"/> representing the access to an indexed property.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.IndexExpression"/>.
    /// </returns>
    /// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param><param name="indexer">The <see cref="T:System.Reflection.PropertyInfo"/> that represents the property to index.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects that are used to index the property.</param>
    [__DynamicallyInvokable]
    public static IndexExpression Property(Expression instance, PropertyInfo indexer, params Expression[] arguments)
    {
      return Expression.Property(instance, indexer, (IEnumerable<Expression>) arguments);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.IndexExpression"/> representing the access to an indexed property.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.IndexExpression"/>.
    /// </returns>
    /// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param><param name="indexer">The <see cref="T:System.Reflection.PropertyInfo"/> that represents the property to index.</param><param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of <see cref="T:System.Linq.Expressions.Expression"/> objects that are used to index the property.</param>
    [__DynamicallyInvokable]
    public static IndexExpression Property(Expression instance, PropertyInfo indexer, IEnumerable<Expression> arguments)
    {
      ReadOnlyCollection<Expression> argList = CollectionExtensions.ToReadOnly<Expression>(arguments);
      Expression.ValidateIndexedProperty(instance, indexer, ref argList);
      return new IndexExpression(instance, indexer, (IList<Expression>) argList);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.InvocationExpression"/> that applies a delegate or lambda expression to a list of argument expressions.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.InvocationExpression"/> that applies the specified delegate or lambda expression to the provided arguments.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the delegate or lambda expression to be applied.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects that represent the arguments that the delegate or lambda expression is applied to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="expression"/>.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1"/>.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception><exception cref="T:System.InvalidOperationException"><paramref name="arguments"/> does not contain the same number of elements as the list of parameters for the delegate represented by <paramref name="expression"/>.</exception>
    [__DynamicallyInvokable]
    public static InvocationExpression Invoke(Expression expression, params Expression[] arguments)
    {
      return Expression.Invoke(expression, (IEnumerable<Expression>) arguments);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.InvocationExpression"/> that applies a delegate or lambda expression to a list of argument expressions.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.InvocationExpression"/> that applies the specified delegate or lambda expression to the provided arguments.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the delegate or lambda expression to be applied to.</param><param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects that represent the arguments that the delegate or lambda expression is applied to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="expression"/>.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1"/>.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception><exception cref="T:System.InvalidOperationException"><paramref name="arguments"/> does not contain the same number of elements as the list of parameters for the delegate represented by <paramref name="expression"/>.</exception>
    [__DynamicallyInvokable]
    public static InvocationExpression Invoke(Expression expression, IEnumerable<Expression> arguments)
    {
      Expression.RequiresCanRead(expression, "expression");
      ReadOnlyCollection<Expression> arguments1 = CollectionExtensions.ToReadOnly<Expression>(arguments);
      MethodInfo invokeMethod = Expression.GetInvokeMethod(expression);
      Expression.ValidateArgumentTypes((MethodBase) invokeMethod, ExpressionType.Invoke, ref arguments1);
      return new InvocationExpression(expression, (IList<Expression>) arguments1, invokeMethod.ReturnType);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LabelExpression"/> representing a label without a default value.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LabelExpression"/> without a default value.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> which this <see cref="T:System.Linq.Expressions.LabelExpression"/> will be associated with.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LabelExpression Label(LabelTarget target)
    {
      return Expression.Label(target, (Expression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LabelExpression"/> representing a label with the given default value.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LabelExpression"/> with the given default value.
    /// </returns>
    /// <param name="target">The <see cref="T:System.Linq.Expressions.LabelTarget"/> which this <see cref="T:System.Linq.Expressions.LabelExpression"/> will be associated with.</param><param name="defaultValue">The value of this <see cref="T:System.Linq.Expressions.LabelExpression"/> when the label is reached through regular control flow.</param>
    [__DynamicallyInvokable]
    public static LabelExpression Label(LabelTarget target, Expression defaultValue)
    {
      Expression.ValidateGoto(target, ref defaultValue, "label", "defaultValue");
      return new LabelExpression(target, defaultValue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LabelTarget"/> representing a label with void type and no name.
    /// </summary>
    /// 
    /// <returns>
    /// The new <see cref="T:System.Linq.Expressions.LabelTarget"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public static LabelTarget Label()
    {
      return Expression.Label(typeof (void), (string) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LabelTarget"/> representing a label with void type and the given name.
    /// </summary>
    /// 
    /// <returns>
    /// The new <see cref="T:System.Linq.Expressions.LabelTarget"/>.
    /// </returns>
    /// <param name="name">The name of the label.</param>
    [__DynamicallyInvokable]
    public static LabelTarget Label(string name)
    {
      return Expression.Label(typeof (void), name);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LabelTarget"/> representing a label with the given type.
    /// </summary>
    /// 
    /// <returns>
    /// The new <see cref="T:System.Linq.Expressions.LabelTarget"/>.
    /// </returns>
    /// <param name="type">The type of value that is passed when jumping to the label.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LabelTarget Label(Type type)
    {
      return Expression.Label(type, (string) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LabelTarget"/> representing a label with the given type and name.
    /// </summary>
    /// 
    /// <returns>
    /// The new <see cref="T:System.Linq.Expressions.LabelTarget"/>.
    /// </returns>
    /// <param name="type">The type of value that is passed when jumping to the label.</param><param name="name">The name of the label.</param>
    [__DynamicallyInvokable]
    public static LabelTarget Label(Type type, string name)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      TypeUtils.ValidateType(type);
      return new LabelTarget(type, name);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.Expression`1"/> where the delegate type is known at compile time.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.Expression`1"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda"/> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param><typeparam name="TDelegate">A delegate type.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="body"/> is null.-or-One or more elements in <paramref name="parameters"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="TDelegate"/> is not a delegate type.-or-<paramref name="body"/>.Type represents a type that is not assignable to the return type of <paramref name="TDelegate"/>.-or-<paramref name="parameters"/> does not contain the same number of elements as the list of parameters for <paramref name="TDelegate"/>.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="parameters"/> is not assignable from the type of the corresponding parameter type of <paramref name="TDelegate"/>.</exception>
    [__DynamicallyInvokable]
    public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
    {
      return Expression.Lambda<TDelegate>(body, false, (IEnumerable<ParameterExpression>) parameters);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.Expression`1"/> where the delegate type is known at compile time.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.Expression`1"/> that has the <see cref="P:System.Linq.Expressions.Expression`1.NodeType"/> property equal to <see cref="P:System.Linq.Expressions.Expression`1.Lambda"/> and the <see cref="P:System.Linq.Expressions.Expression`1.Body"/> and <see cref="P:System.Linq.Expressions.Expression`1.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.Expression`1.Body"/> property equal to.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression.</param><param name="parameters">An array that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.Expression`1.Parameters"/> collection.</param><typeparam name="TDelegate">The delegate type. </typeparam>
    [__DynamicallyInvokable]
    public static Expression<TDelegate> Lambda<TDelegate>(Expression body, bool tailCall, params ParameterExpression[] parameters)
    {
      return Expression.Lambda<TDelegate>(body, tailCall, (IEnumerable<ParameterExpression>) parameters);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.Expression`1"/> where the delegate type is known at compile time.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.Expression`1"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda"/> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param><typeparam name="TDelegate">A delegate type.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="body"/> is null.-or-One or more elements in <paramref name="parameters"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="TDelegate"/> is not a delegate type.-or-<paramref name="body"/>.Type represents a type that is not assignable to the return type of <paramref name="TDelegate"/>.-or-<paramref name="parameters"/> does not contain the same number of elements as the list of parameters for <paramref name="TDelegate"/>.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="parameters"/> is not assignable from the type of the corresponding parameter type of <paramref name="TDelegate"/>.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static Expression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
    {
      return Expression.Lambda<TDelegate>(body, (string) null, false, parameters);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.Expression`1"/> where the delegate type is known at compile time.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.Expression`1"/> that has the <see cref="P:System.Linq.Expressions.Expression`1.NodeType"/> property equal to <see cref="P:System.Linq.Expressions.Expression`1.Lambda"/> and the <see cref="P:System.Linq.Expressions.Expression`1.Body"/> and <see cref="P:System.Linq.Expressions.Expression`1.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.Expression`1.Body"/> property equal to.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.Expression`1.Parameters"/> collection.</param><typeparam name="TDelegate">The delegate type. </typeparam>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static Expression<TDelegate> Lambda<TDelegate>(Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
    {
      return Expression.Lambda<TDelegate>(body, (string) null, tailCall, parameters);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.Expression`1"/> where the delegate type is known at compile time.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.Expression`1"/> that has the <see cref="P:System.Linq.Expressions.Expression`1.NodeType"/> property equal to <see cref="P:System.Linq.Expressions.Expression`1.Lambda"/> and the <see cref="P:System.Linq.Expressions.Expression`1.Body"/> and <see cref="P:System.Linq.Expressions.Expression`1.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.Expression`1.Body"/> property equal to.</param><param name="name">The name of the lambda. Used for generating debugging information.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.Expression`1.Parameters"/> collection.</param><typeparam name="TDelegate">The delegate type. </typeparam>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static Expression<TDelegate> Lambda<TDelegate>(Expression body, string name, IEnumerable<ParameterExpression> parameters)
    {
      return Expression.Lambda<TDelegate>(body, name, false, parameters);
    }

    /// <summary>
    /// Creates an <see cref="T:System.Linq.Expressions.Expression`1"/> where the delegate type is known at compile time.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.Expressions.Expression`1"/> that has the <see cref="P:System.Linq.Expressions.Expression`1.NodeType"/> property equal to <see cref="P:System.Linq.Expressions.Expression`1.Lambda"/> and the <see cref="P:System.Linq.Expressions.Expression`1.Body"/> and <see cref="P:System.Linq.Expressions.Expression`1.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.Expression`1.Body"/> property equal to.</param><param name="name">The name of the lambda. Used for generating debugging info.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.Expression`1.Parameters"/> collection.</param><typeparam name="TDelegate">The delegate type. </typeparam>
    [__DynamicallyInvokable]
    public static Expression<TDelegate> Lambda<TDelegate>(Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
    {
      ReadOnlyCollection<ParameterExpression> parameters1 = CollectionExtensions.ToReadOnly<ParameterExpression>(parameters);
      Expression.ValidateLambdaArgs(typeof (TDelegate), ref body, parameters1);
      return new Expression<TDelegate>(body, name, tailCall, parameters1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LambdaExpression"/> by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda"/> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="body"/> is null.-or-One or more elements of <paramref name="parameters"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="parameters"/> contains more than sixteen elements.</exception>
    [__DynamicallyInvokable]
    public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
    {
      return Expression.Lambda(body, false, (IEnumerable<ParameterExpression>) parameters);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression.</param><param name="parameters">An array that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param>
    [__DynamicallyInvokable]
    public static LambdaExpression Lambda(Expression body, bool tailCall, params ParameterExpression[] parameters)
    {
      return Expression.Lambda(body, tailCall, (IEnumerable<ParameterExpression>) parameters);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LambdaExpression Lambda(Expression body, IEnumerable<ParameterExpression> parameters)
    {
      return Expression.Lambda(body, (string) null, false, parameters);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LambdaExpression Lambda(Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
    {
      return Expression.Lambda(body, (string) null, tailCall, parameters);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LambdaExpression"/> by first constructing a delegate type. It can be used when the delegate type is not known at compile time.
    /// </summary>
    /// 
    /// <returns>
    /// An object that represents a lambda expression which has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda"/> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="delegateType">A <see cref="T:System.Type"/> that represents a delegate signature for the lambda.</param><param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="parameters">An array of <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="delegateType"/> or <paramref name="body"/> is null.-or-One or more elements in <paramref name="parameters"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="delegateType"/> does not represent a delegate type.-or-<paramref name="body"/>.Type represents a type that is not assignable to the return type of the delegate type represented by <paramref name="delegateType"/>.-or-<paramref name="parameters"/> does not contain the same number of elements as the list of parameters for the delegate type represented by <paramref name="delegateType"/>.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="parameters"/> is not assignable from the type of the corresponding parameter type of the delegate type represented by <paramref name="delegateType"/>.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LambdaExpression Lambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
    {
      return Expression.Lambda(delegateType, body, (string) null, false, (IEnumerable<ParameterExpression>) parameters);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="delegateType">A <see cref="P:System.Linq.Expressions.Expression.Type"/> representing the delegate signature for the lambda.</param><param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression.</param><param name="parameters">An array that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LambdaExpression Lambda(Type delegateType, Expression body, bool tailCall, params ParameterExpression[] parameters)
    {
      return Expression.Lambda(delegateType, body, (string) null, tailCall, (IEnumerable<ParameterExpression>) parameters);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LambdaExpression"/> by first constructing a delegate type. It can be used when the delegate type is not known at compile time.
    /// </summary>
    /// 
    /// <returns>
    /// An object that represents a lambda expression which has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Lambda"/> and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="delegateType">A <see cref="T:System.Type"/> that represents a delegate signature for the lambda.</param><param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="delegateType"/> or <paramref name="body"/> is null.-or-One or more elements in <paramref name="parameters"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="delegateType"/> does not represent a delegate type.-or-<paramref name="body"/>.Type represents a type that is not assignable to the return type of the delegate type represented by <paramref name="delegateType"/>.-or-<paramref name="parameters"/> does not contain the same number of elements as the list of parameters for the delegate type represented by <paramref name="delegateType"/>.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="parameters"/> is not assignable from the type of the corresponding parameter type of the delegate type represented by <paramref name="delegateType"/>.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LambdaExpression Lambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
    {
      return Expression.Lambda(delegateType, body, (string) null, false, parameters);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="delegateType">A <see cref="P:System.Linq.Expressions.Expression.Type"/> representing the delegate signature for the lambda.</param><param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LambdaExpression Lambda(Type delegateType, Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
    {
      return Expression.Lambda(delegateType, body, (string) null, tailCall, parameters);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="name">The name for the lambda. Used for emitting debug information.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LambdaExpression Lambda(Expression body, string name, IEnumerable<ParameterExpression> parameters)
    {
      return Expression.Lambda(body, name, false, parameters);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="name">The name for the lambda. Used for emitting debug information.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param>
    [__DynamicallyInvokable]
    public static LambdaExpression Lambda(Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
    {
      ContractUtils.RequiresNotNull((object) body, "body");
      ReadOnlyCollection<ParameterExpression> parameters1 = CollectionExtensions.ToReadOnly<ParameterExpression>(parameters);
      int count = parameters1.Count;
      Type[] types = new Type[count + 1];
      if (count > 0)
      {
        Set<ParameterExpression> set = new Set<ParameterExpression>(parameters1.Count);
        for (int index = 0; index < count; ++index)
        {
          ParameterExpression parameterExpression = parameters1[index];
          ContractUtils.RequiresNotNull((object) parameterExpression, "parameter");
          types[index] = parameterExpression.IsByRef ? parameterExpression.Type.MakeByRefType() : parameterExpression.Type;
          if (set.Contains(parameterExpression))
            throw System.Linq.Expressions.Error.DuplicateVariable((object) parameterExpression);
          set.Add(parameterExpression);
        }
      }
      types[count] = body.Type;
      return Expression.CreateLambda(DelegateHelpers.MakeDelegateType(types), body, name, tailCall, parameters1);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="delegateType">A <see cref="P:System.Linq.Expressions.Expression.Type"/> representing the delegate signature for the lambda.</param><param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to.</param><param name="name">The name for the lambda. Used for emitting debug information.</param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection.</param>
    [__DynamicallyInvokable]
    public static LambdaExpression Lambda(Type delegateType, Expression body, string name, IEnumerable<ParameterExpression> parameters)
    {
      ReadOnlyCollection<ParameterExpression> parameters1 = CollectionExtensions.ToReadOnly<ParameterExpression>(parameters);
      Expression.ValidateLambdaArgs(delegateType, ref body, parameters1);
      return Expression.CreateLambda(delegateType, body, name, false, parameters1);
    }

    /// <summary>
    /// Creates a LambdaExpression by first constructing a delegate type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.LambdaExpression"/> that has the <see cref="P:System.Linq.Expressions.LambdaExpression.NodeType"/> property equal to Lambda and the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> and <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> properties set to the specified values.
    /// </returns>
    /// <param name="delegateType">A <see cref="P:System.Linq.Expressions.Expression.Type"/> representing the delegate signature for the lambda.</param><param name="body">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.LambdaExpression.Body"/> property equal to. </param><param name="name">The name for the lambda. Used for emitting debug information.</param><param name="tailCall">A <see cref="T:System.Boolean"/> that indicates if tail call optimization will be applied when compiling the created expression. </param><param name="parameters">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.LambdaExpression.Parameters"/> collection. </param>
    [__DynamicallyInvokable]
    public static LambdaExpression Lambda(Type delegateType, Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
    {
      ReadOnlyCollection<ParameterExpression> parameters1 = CollectionExtensions.ToReadOnly<ParameterExpression>(parameters);
      Expression.ValidateLambdaArgs(delegateType, ref body, parameters1);
      return Expression.CreateLambda(delegateType, body, name, tailCall, parameters1);
    }

    private static void ValidateLambdaArgs(Type delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters)
    {
      ContractUtils.RequiresNotNull((object) delegateType, "delegateType");
      Expression.RequiresCanRead(body, "body");
      if (!typeof (MulticastDelegate).IsAssignableFrom(delegateType) || delegateType == typeof (MulticastDelegate))
        throw System.Linq.Expressions.Error.LambdaTypeMustBeDerivedFromSystemDelegate();
      CacheDict<Type, MethodInfo> cacheDict = Expression._LambdaDelegateCache;
      MethodInfo method;
      if (!cacheDict.TryGetValue(delegateType, out method))
      {
        method = delegateType.GetMethod("Invoke");
        if (TypeUtils.CanCache(delegateType))
          cacheDict[delegateType] = method;
      }
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) method);
      if (parametersCached.Length > 0)
      {
        if (parametersCached.Length != parameters.Count)
          throw System.Linq.Expressions.Error.IncorrectNumberOfLambdaDeclarationParameters();
        Set<ParameterExpression> set = new Set<ParameterExpression>(parametersCached.Length);
        int index = 0;
        for (int length = parametersCached.Length; index < length; ++index)
        {
          ParameterExpression parameterExpression = parameters[index];
          ParameterInfo parameterInfo = parametersCached[index];
          Expression.RequiresCanRead((Expression) parameterExpression, "parameters");
          Type src = parameterInfo.ParameterType;
          if (parameterExpression.IsByRef)
          {
            if (!src.IsByRef)
              throw System.Linq.Expressions.Error.ParameterExpressionNotValidAsDelegate((object) parameterExpression.Type.MakeByRefType(), (object) src);
            src = src.GetElementType();
          }
          if (!TypeUtils.AreReferenceAssignable(parameterExpression.Type, src))
            throw System.Linq.Expressions.Error.ParameterExpressionNotValidAsDelegate((object) parameterExpression.Type, (object) src);
          if (set.Contains(parameterExpression))
            throw System.Linq.Expressions.Error.DuplicateVariable((object) parameterExpression);
          set.Add(parameterExpression);
        }
      }
      else if (parameters.Count > 0)
        throw System.Linq.Expressions.Error.IncorrectNumberOfLambdaDeclarationParameters();
      if (method.ReturnType != typeof (void) && !TypeUtils.AreReferenceAssignable(method.ReturnType, body.Type) && !Expression.TryQuote(method.ReturnType, ref body))
        throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchReturn((object) body.Type, (object) method.ReturnType);
    }

    /// <summary>
    /// Creates a <see cref="P:System.Linq.Expressions.Expression.Type"/> object that represents a generic System.Func delegate type that has specific type arguments. The last type argument specifies the return type of the created delegate.
    /// </summary>
    /// 
    /// <returns>
    /// The type of a System.Func delegate that has the specified type arguments.
    /// </returns>
    /// <param name="typeArgs">An array of one to seventeen <see cref="T:System.Type"/> objects that specify the type arguments for the System.Func delegate type.</param><exception cref="T:System.ArgumentException"><paramref name="typeArgs"/> contains fewer than one or more than seventeen elements.</exception><exception cref="T:System.ArgumentNullException"><paramref name="typeArgs"/> is null.</exception>
    [__DynamicallyInvokable]
    public static Type GetFuncType(params Type[] typeArgs)
    {
      if (!Expression.ValidateTryGetFuncActionArgs(typeArgs))
        throw System.Linq.Expressions.Error.TypeMustNotBeByRef();
      Type funcType = DelegateHelpers.GetFuncType(typeArgs);
      if (funcType == (Type) null)
        throw System.Linq.Expressions.Error.IncorrectNumberOfTypeArgsForFunc();
      else
        return funcType;
    }

    /// <summary>
    /// Creates a <see cref="P:System.Linq.Expressions.Expression.Type"/> object that represents a generic System.Func delegate type that has specific type arguments. The last type argument specifies the return type of the created delegate.
    /// </summary>
    /// 
    /// <returns>
    /// true if generic System.Func delegate type was created for specific <paramref name="typeArgs"/>; false otherwise.
    /// </returns>
    /// <param name="typeArgs">An array of Type objects that specify the type arguments for the System.Func delegate type.</param><param name="funcType">When this method returns, contains the generic System.Func delegate type that has specific type arguments. Contains null if there is no generic System.Func delegate that matches the <paramref name="typeArgs"/>.This parameter is passed uninitialized.</param>
    [__DynamicallyInvokable]
    public static bool TryGetFuncType(Type[] typeArgs, out Type funcType)
    {
      if (Expression.ValidateTryGetFuncActionArgs(typeArgs))
        return (funcType = DelegateHelpers.GetFuncType(typeArgs)) != (Type) null;
      funcType = (Type) null;
      return false;
    }

    /// <summary>
    /// Creates a <see cref="T:System.Type"/> object that represents a generic System.Action delegate type that has specific type arguments.
    /// </summary>
    /// 
    /// <returns>
    /// The type of a System.Action delegate that has the specified type arguments.
    /// </returns>
    /// <param name="typeArgs">An array of up to sixteen <see cref="T:System.Type"/> objects that specify the type arguments for the System.Action delegate type.</param><exception cref="T:System.ArgumentException"><paramref name="typeArgs"/> contains more than sixteen elements.</exception><exception cref="T:System.ArgumentNullException"><paramref name="typeArgs"/> is null.</exception>
    [__DynamicallyInvokable]
    public static Type GetActionType(params Type[] typeArgs)
    {
      if (!Expression.ValidateTryGetFuncActionArgs(typeArgs))
        throw System.Linq.Expressions.Error.TypeMustNotBeByRef();
      Type actionType = DelegateHelpers.GetActionType(typeArgs);
      if (actionType == (Type) null)
        throw System.Linq.Expressions.Error.IncorrectNumberOfTypeArgsForAction();
      else
        return actionType;
    }

    /// <summary>
    /// Creates a <see cref="P:System.Linq.Expressions.Expression.Type"/> object that represents a generic System.Action delegate type that has specific type arguments.
    /// </summary>
    /// 
    /// <returns>
    /// true if generic System.Action delegate type was created for specific <paramref name="typeArgs"/>; false otherwise.
    /// </returns>
    /// <param name="typeArgs">An array of Type objects that specify the type arguments for the System.Action delegate type.</param><param name="actionType">When this method returns, contains the generic System.Action delegate type that has specific type arguments. Contains null if there is no generic System.Action delegate that matches the <paramref name="typeArgs"/>.This parameter is passed uninitialized.</param>
    [__DynamicallyInvokable]
    public static bool TryGetActionType(Type[] typeArgs, out Type actionType)
    {
      if (Expression.ValidateTryGetFuncActionArgs(typeArgs))
        return (actionType = DelegateHelpers.GetActionType(typeArgs)) != (Type) null;
      actionType = (Type) null;
      return false;
    }

    /// <summary>
    /// Gets a <see cref="P:System.Linq.Expressions.Expression.Type"/> object that represents a generic System.Func or System.Action delegate type that has specific type arguments.
    /// </summary>
    /// 
    /// <returns>
    /// The delegate type.
    /// </returns>
    /// <param name="typeArgs">The type arguments of the delegate.</param>
    [__DynamicallyInvokable]
    public static Type GetDelegateType(params Type[] typeArgs)
    {
      ContractUtils.RequiresNotEmpty<Type>((ICollection<Type>) typeArgs, "typeArgs");
      ContractUtils.RequiresNotNullItems<Type>((IList<Type>) typeArgs, "typeArgs");
      return DelegateHelpers.MakeDelegateType(typeArgs);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ListInitExpression"/> that uses a method named "Add" to add elements to a collection.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ListInitExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit"/> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property set to the specified value.
    /// </returns>
    /// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression"/> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property equal to.</param><param name="initializers">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="newExpression"/> or <paramref name="initializers"/> is null.-or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="newExpression"/>.Type does not implement <see cref="T:System.Collections.IEnumerable"/>.</exception><exception cref="T:System.InvalidOperationException">There is no instance method named "Add" (case insensitive) declared in <paramref name="newExpression"/>.Type or its base type.-or-The add method on <paramref name="newExpression"/>.Type or its base type does not take exactly one argument.-or-The type represented by the <see cref="P:System.Linq.Expressions.Expression.Type"/> property of the first element of <paramref name="initializers"/> is not assignable to the argument type of the add method on <paramref name="newExpression"/>.Type or its base type.-or-More than one argument-compatible method named "Add" (case-insensitive) exists on <paramref name="newExpression"/>.Type and/or its base type.</exception>
    [__DynamicallyInvokable]
    public static ListInitExpression ListInit(NewExpression newExpression, params Expression[] initializers)
    {
      ContractUtils.RequiresNotNull((object) newExpression, "newExpression");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      return Expression.ListInit(newExpression, (IEnumerable<Expression>) initializers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ListInitExpression"/> that uses a method named "Add" to add elements to a collection.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ListInitExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit"/> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property set to the specified value.
    /// </returns>
    /// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression"/> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property equal to.</param><param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="newExpression"/> or <paramref name="initializers"/> is null.-or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="newExpression"/>.Type does not implement <see cref="T:System.Collections.IEnumerable"/>.</exception><exception cref="T:System.InvalidOperationException">There is no instance method named "Add" (case insensitive) declared in <paramref name="newExpression"/>.Type or its base type.-or-The add method on <paramref name="newExpression"/>.Type or its base type does not take exactly one argument.-or-The type represented by the <see cref="P:System.Linq.Expressions.Expression.Type"/> property of the first element of <paramref name="initializers"/> is not assignable to the argument type of the add method on <paramref name="newExpression"/>.Type or its base type.-or-More than one argument-compatible method named "Add" (case-insensitive) exists on <paramref name="newExpression"/>.Type and/or its base type.</exception>
    [__DynamicallyInvokable]
    public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<Expression> initializers)
    {
      ContractUtils.RequiresNotNull((object) newExpression, "newExpression");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      ReadOnlyCollection<Expression> readOnlyCollection = CollectionExtensions.ToReadOnly<Expression>(initializers);
      if (readOnlyCollection.Count == 0)
        throw System.Linq.Expressions.Error.ListInitializerWithZeroMembers();
      MethodInfo method = Expression.FindMethod(newExpression.Type, "Add", (Type[]) null, new Expression[1]
      {
        readOnlyCollection[0]
      }, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      return Expression.ListInit(newExpression, method, initializers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ListInitExpression"/> that uses a specified method to add elements to a collection.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ListInitExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit"/> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property set to the specified value.
    /// </returns>
    /// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression"/> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property equal to.</param><param name="addMethod">A <see cref="T:System.Reflection.MethodInfo"/> that represents an instance method that takes one argument, that adds an element to a collection.</param><param name="initializers">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="newExpression"/> or <paramref name="initializers"/> is null.-or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="newExpression"/>.Type does not implement <see cref="T:System.Collections.IEnumerable"/>.-or-<paramref name="addMethod"/> is not null and it does not represent an instance method named "Add" (case insensitive) that takes exactly one argument.-or-<paramref name="addMethod"/> is not null and the type represented by the <see cref="P:System.Linq.Expressions.Expression.Type"/> property of one or more elements of <paramref name="initializers"/> is not assignable to the argument type of the method that <paramref name="addMethod"/> represents.</exception><exception cref="T:System.InvalidOperationException"><paramref name="addMethod"/> is null and no instance method named "Add" that takes one type-compatible argument exists on <paramref name="newExpression"/>.Type or its base type.</exception>
    [__DynamicallyInvokable]
    public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, params Expression[] initializers)
    {
      if (addMethod == (MethodInfo) null)
        return Expression.ListInit(newExpression, (IEnumerable<Expression>) initializers);
      ContractUtils.RequiresNotNull((object) newExpression, "newExpression");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      return Expression.ListInit(newExpression, addMethod, (IEnumerable<Expression>) initializers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ListInitExpression"/> that uses a specified method to add elements to a collection.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ListInitExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit"/> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property set to the specified value.
    /// </returns>
    /// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression"/> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property equal to.</param><param name="addMethod">A <see cref="T:System.Reflection.MethodInfo"/> that represents an instance method named "Add" (case insensitive), that adds an element to a collection.</param><param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="newExpression"/> or <paramref name="initializers"/> is null.-or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="newExpression"/>.Type does not implement <see cref="T:System.Collections.IEnumerable"/>.-or-<paramref name="addMethod"/> is not null and it does not represent an instance method named "Add" (case insensitive) that takes exactly one argument.-or-<paramref name="addMethod"/> is not null and the type represented by the <see cref="P:System.Linq.Expressions.Expression.Type"/> property of one or more elements of <paramref name="initializers"/> is not assignable to the argument type of the method that <paramref name="addMethod"/> represents.</exception><exception cref="T:System.InvalidOperationException"><paramref name="addMethod"/> is null and no instance method named "Add" that takes one type-compatible argument exists on <paramref name="newExpression"/>.Type or its base type.</exception>
    [__DynamicallyInvokable]
    public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, IEnumerable<Expression> initializers)
    {
      if (addMethod == (MethodInfo) null)
        return Expression.ListInit(newExpression, initializers);
      ContractUtils.RequiresNotNull((object) newExpression, "newExpression");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      ReadOnlyCollection<Expression> readOnlyCollection = CollectionExtensions.ToReadOnly<Expression>(initializers);
      if (readOnlyCollection.Count == 0)
        throw System.Linq.Expressions.Error.ListInitializerWithZeroMembers();
      ElementInit[] list = new ElementInit[readOnlyCollection.Count];
      for (int index = 0; index < readOnlyCollection.Count; ++index)
        list[index] = Expression.ElementInit(addMethod, new Expression[1]
        {
          readOnlyCollection[index]
        });
      return Expression.ListInit(newExpression, (IEnumerable<ElementInit>) new TrueReadOnlyCollection<ElementInit>(list));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ListInitExpression"/> that uses specified <see cref="T:System.Linq.Expressions.ElementInit"/> objects to initialize a collection.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ListInitExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit"/> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> and <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers"/> properties set to the specified values.
    /// </returns>
    /// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression"/> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property equal to.</param><param name="initializers">An array of <see cref="T:System.Linq.Expressions.ElementInit"/> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="newExpression"/> or <paramref name="initializers"/> is null.-or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="newExpression"/>.Type does not implement <see cref="T:System.Collections.IEnumerable"/>.</exception>
    [__DynamicallyInvokable]
    public static ListInitExpression ListInit(NewExpression newExpression, params ElementInit[] initializers)
    {
      return Expression.ListInit(newExpression, (IEnumerable<ElementInit>) initializers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ListInitExpression"/> that uses specified <see cref="T:System.Linq.Expressions.ElementInit"/> objects to initialize a collection.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ListInitExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ListInit"/> and the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> and <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers"/> properties set to the specified values.
    /// </returns>
    /// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression"/> to set the <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression"/> property equal to.</param><param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ElementInit"/> objects to use to populate the <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="newExpression"/> or <paramref name="initializers"/> is null.-or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="newExpression"/>.Type does not implement <see cref="T:System.Collections.IEnumerable"/>.</exception>
    [__DynamicallyInvokable]
    public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<ElementInit> initializers)
    {
      ContractUtils.RequiresNotNull((object) newExpression, "newExpression");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      ReadOnlyCollection<ElementInit> initializers1 = CollectionExtensions.ToReadOnly<ElementInit>(initializers);
      if (initializers1.Count == 0)
        throw System.Linq.Expressions.Error.ListInitializerWithZeroMembers();
      Expression.ValidateListInitArgs(newExpression.Type, initializers1);
      return new ListInitExpression(newExpression, initializers1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LoopExpression"/> with the given body.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.LoopExpression"/>.
    /// </returns>
    /// <param name="body">The body of the loop.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LoopExpression Loop(Expression body)
    {
      return Expression.Loop(body, (LabelTarget) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LoopExpression"/> with the given body and break target.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.LoopExpression"/>.
    /// </returns>
    /// <param name="body">The body of the loop.</param><param name="break">The break target used by the loop body.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static LoopExpression Loop(Expression body, LabelTarget @break)
    {
      return Expression.Loop(body, @break, (LabelTarget) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.LoopExpression"/> with the given body.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.LoopExpression"/>.
    /// </returns>
    /// <param name="body">The body of the loop.</param><param name="break">The break target used by the loop body.</param><param name="continue">The continue target used by the loop body.</param>
    [__DynamicallyInvokable]
    public static LoopExpression Loop(Expression body, LabelTarget @break, LabelTarget @continue)
    {
      Expression.RequiresCanRead(body, "body");
      if (@continue != null && @continue.Type != typeof (void))
        throw System.Linq.Expressions.Error.LabelTypeMustBeVoid();
      else
        return new LoopExpression(body, @break, @continue);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberAssignment"/> that represents the initialization of a field or property.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberAssignment"/> that has <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> equal to <see cref="F:System.Linq.Expressions.MemberBindingType.Assignment"/> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> and <see cref="P:System.Linq.Expressions.MemberAssignment.Expression"/> properties set to the specified values.
    /// </returns>
    /// <param name="member">A <see cref="T:System.Reflection.MemberInfo"/> to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property equal to.</param><param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.MemberAssignment.Expression"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="member"/> or <paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="member"/> does not represent a field or property.-or-The property represented by <paramref name="member"/> does not have a set accessor.-or-<paramref name="expression"/>.Type is not assignable to the type of the field or property that <paramref name="member"/> represents.</exception>
    [__DynamicallyInvokable]
    public static MemberAssignment Bind(MemberInfo member, Expression expression)
    {
      ContractUtils.RequiresNotNull((object) member, "member");
      Expression.RequiresCanRead(expression, "expression");
      Type memberType;
      Expression.ValidateSettableFieldOrPropertyMember(member, out memberType);
      if (!memberType.IsAssignableFrom(expression.Type))
        throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
      else
        return new MemberAssignment(member, expression);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberAssignment"/> that represents the initialization of a member by using a property accessor method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberAssignment"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.Assignment"/>, the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property set to the <see cref="T:System.Reflection.PropertyInfo"/> that represents the property accessed in <paramref name="propertyAccessor"/>, and the <see cref="P:System.Linq.Expressions.MemberAssignment.Expression"/> property set to <paramref name="expression"/>.
    /// </returns>
    /// <param name="propertyAccessor">A <see cref="T:System.Reflection.MethodInfo"/> that represents a property accessor method.</param><param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.MemberAssignment.Expression"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="propertyAccessor"/> or <paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="propertyAccessor"/> does not represent a property accessor method.-or-The property accessed by <paramref name="propertyAccessor"/> does not have a set accessor.-or-<paramref name="expression"/>.Type is not assignable to the type of the field or property that <paramref name="member"/> represents.</exception>
    [__DynamicallyInvokable]
    public static MemberAssignment Bind(MethodInfo propertyAccessor, Expression expression)
    {
      ContractUtils.RequiresNotNull((object) propertyAccessor, "propertyAccessor");
      ContractUtils.RequiresNotNull((object) expression, "expression");
      Expression.ValidateMethodInfo(propertyAccessor);
      return Expression.Bind((MemberInfo) Expression.GetProperty(propertyAccessor), expression);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> that represents accessing a field.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess"/> and the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> and <see cref="P:System.Linq.Expressions.MemberExpression.Member"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> property equal to. For static (Shared in Visual Basic), <paramref name="expression"/> must be null.</param><param name="field">The <see cref="T:System.Reflection.FieldInfo"/> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Member"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="field"/> is null.-or-The field represented by <paramref name="field"/> is not static (Shared in Visual Basic) and <paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="expression"/>.Type is not assignable to the declaring type of the field represented by <paramref name="field"/>.</exception>
    [__DynamicallyInvokable]
    public static MemberExpression Field(Expression expression, FieldInfo field)
    {
      ContractUtils.RequiresNotNull((object) field, "field");
      if (field.IsStatic)
      {
        if (expression != null)
          throw new ArgumentException(System.Linq.Expressions.Strings.OnlyStaticFieldsHaveNullInstance, "expression");
      }
      else
      {
        if (expression == null)
          throw new ArgumentException(System.Linq.Expressions.Strings.OnlyStaticFieldsHaveNullInstance, "field");
        Expression.RequiresCanRead(expression, "expression");
        if (!TypeUtils.AreReferenceAssignable(field.DeclaringType, expression.Type))
          throw System.Linq.Expressions.Error.FieldInfoNotDefinedForType((object) field.DeclaringType, (object) field.Name, (object) expression.Type);
      }
      return MemberExpression.Make(expression, (MemberInfo) field);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> that represents accessing a field given the name of the field.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess"/>, the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> property set to <paramref name="expression"/>, and the <see cref="P:System.Linq.Expressions.MemberExpression.Member"/> property set to the <see cref="T:System.Reflection.FieldInfo"/> that represents the field denoted by <paramref name="fieldName"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> whose <see cref="P:System.Linq.Expressions.Expression.Type"/> contains a field named <paramref name="fieldName"/>. This can be null for static fields.</param><param name="fieldName">The name of a field to be accessed.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="fieldName"/> is null.</exception><exception cref="T:System.ArgumentException">No field named <paramref name="fieldName"/> is defined in <paramref name="expression"/>.Type or its base types.</exception>
    [__DynamicallyInvokable]
    public static MemberExpression Field(Expression expression, string fieldName)
    {
      Expression.RequiresCanRead(expression, "expression");
      FieldInfo field = expression.Type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      if (field == (FieldInfo) null)
        field = expression.Type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
      if (field == (FieldInfo) null)
        throw System.Linq.Expressions.Error.InstanceFieldNotDefinedForType((object) fieldName, (object) expression.Type);
      else
        return Expression.Field(expression, field);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> that represents accessing a field.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.MemberExpression"/>.
    /// </returns>
    /// <param name="expression">The containing object of the field. This can be null for static fields.</param><param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type"/> that contains the field.</param><param name="fieldName">The field to be accessed.</param>
    [__DynamicallyInvokable]
    public static MemberExpression Field(Expression expression, Type type, string fieldName)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      FieldInfo field = type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      if (field == (FieldInfo) null)
        field = type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
      if (field == (FieldInfo) null)
        throw System.Linq.Expressions.Error.FieldNotDefinedForType((object) fieldName, (object) type);
      else
        return Expression.Field(expression, field);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> that represents accessing a property.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess"/>, the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> property set to <paramref name="expression"/>, and the <see cref="P:System.Linq.Expressions.MemberExpression.Member"/> property set to the <see cref="T:System.Reflection.PropertyInfo"/> that represents the property denoted by <paramref name="propertyName"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> whose <see cref="P:System.Linq.Expressions.Expression.Type"/> contains a property named <paramref name="propertyName"/>. This can be null for static properties.</param><param name="propertyName">The name of a property to be accessed.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="propertyName"/> is null.</exception><exception cref="T:System.ArgumentException">No property named <paramref name="propertyName"/> is defined in <paramref name="expression"/>.Type or its base types.</exception>
    [__DynamicallyInvokable]
    public static MemberExpression Property(Expression expression, string propertyName)
    {
      Expression.RequiresCanRead(expression, "expression");
      ContractUtils.RequiresNotNull((object) propertyName, "propertyName");
      PropertyInfo property = expression.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      if (property == (PropertyInfo) null)
        property = expression.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
      if (property == (PropertyInfo) null)
        throw System.Linq.Expressions.Error.InstancePropertyNotDefinedForType((object) propertyName, (object) expression.Type);
      else
        return Expression.Property(expression, property);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> accessing a property.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.MemberExpression"/>.
    /// </returns>
    /// <param name="expression">The containing object of the property. This can be null for static properties.</param><param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type"/> that contains the property.</param><param name="propertyName">The property to be accessed.</param>
    [__DynamicallyInvokable]
    public static MemberExpression Property(Expression expression, Type type, string propertyName)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      ContractUtils.RequiresNotNull((object) propertyName, "propertyName");
      PropertyInfo property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      if (property == (PropertyInfo) null)
        property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
      if (property == (PropertyInfo) null)
        throw System.Linq.Expressions.Error.PropertyNotDefinedForType((object) propertyName, (object) type);
      else
        return Expression.Property(expression, property);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> that represents accessing a property.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess"/> and the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> and <see cref="P:System.Linq.Expressions.MemberExpression.Member"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> property equal to. This can be null for static properties.</param><param name="property">The <see cref="T:System.Reflection.PropertyInfo"/> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Member"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="property"/> is null.-or-The property that <paramref name="property"/> represents is not static (Shared in Visual Basic) and <paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="expression"/>.Type is not assignable to the declaring type of the property that <paramref name="property"/> represents.</exception>
    [__DynamicallyInvokable]
    public static MemberExpression Property(Expression expression, PropertyInfo property)
    {
      ContractUtils.RequiresNotNull((object) property, "property");
      MethodInfo methodInfo = property.GetGetMethod(true) ?? property.GetSetMethod(true);
      if (methodInfo == (MethodInfo) null)
        throw System.Linq.Expressions.Error.PropertyDoesNotHaveAccessor((object) property);
      if (methodInfo.IsStatic)
      {
        if (expression != null)
          throw new ArgumentException(System.Linq.Expressions.Strings.OnlyStaticPropertiesHaveNullInstance, "expression");
      }
      else
      {
        if (expression == null)
          throw new ArgumentException(System.Linq.Expressions.Strings.OnlyStaticPropertiesHaveNullInstance, "property");
        Expression.RequiresCanRead(expression, "expression");
        if (!TypeUtils.IsValidInstanceType((MemberInfo) property, expression.Type))
          throw System.Linq.Expressions.Error.PropertyNotDefinedForType((object) property, (object) expression.Type);
      }
      return MemberExpression.Make(expression, (MemberInfo) property);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> that represents accessing a property by using a property accessor method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess"/>, the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> property set to <paramref name="expression"/> and the <see cref="P:System.Linq.Expressions.MemberExpression.Member"/> property set to the <see cref="T:System.Reflection.PropertyInfo"/> that represents the property accessed in <paramref name="propertyAccessor"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> property equal to. This can be null for static properties.</param><param name="propertyAccessor">The <see cref="T:System.Reflection.MethodInfo"/> that represents a property accessor method.</param><exception cref="T:System.ArgumentNullException"><paramref name="propertyAccessor"/> is null.-or-The method that <paramref name="propertyAccessor"/> represents is not static (Shared in Visual Basic) and <paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="expression"/>.Type is not assignable to the declaring type of the method represented by <paramref name="propertyAccessor"/>.-or-The method that <paramref name="propertyAccessor"/> represents is not a property accessor method.</exception>
    [__DynamicallyInvokable]
    public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
    {
      ContractUtils.RequiresNotNull((object) propertyAccessor, "propertyAccessor");
      Expression.ValidateMethodInfo(propertyAccessor);
      return Expression.Property(expression, Expression.GetProperty(propertyAccessor));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> that represents accessing a property or field.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberAccess"/>, the <see cref="P:System.Linq.Expressions.MemberExpression.Expression"/> property set to <paramref name="expression"/>, and the <see cref="P:System.Linq.Expressions.MemberExpression.Member"/> property set to the <see cref="T:System.Reflection.PropertyInfo"/> or <see cref="T:System.Reflection.FieldInfo"/> that represents the property or field denoted by <paramref name="propertyOrFieldName"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> whose <see cref="P:System.Linq.Expressions.Expression.Type"/> contains a property or field named <paramref name="propertyOrFieldName"/>. This can be null for static members.</param><param name="propertyOrFieldName">The name of a property or field to be accessed.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="propertyOrFieldName"/> is null.</exception><exception cref="T:System.ArgumentException">No property or field named <paramref name="propertyOrFieldName"/> is defined in <paramref name="expression"/>.Type or its base types.</exception>
    [__DynamicallyInvokable]
    public static MemberExpression PropertyOrField(Expression expression, string propertyOrFieldName)
    {
      Expression.RequiresCanRead(expression, "expression");
      PropertyInfo property1 = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      if (property1 != (PropertyInfo) null)
        return Expression.Property(expression, property1);
      FieldInfo field1 = expression.Type.GetField(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
      if (field1 != (FieldInfo) null)
        return Expression.Field(expression, field1);
      PropertyInfo property2 = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
      if (property2 != (PropertyInfo) null)
        return Expression.Property(expression, property2);
      FieldInfo field2 = expression.Type.GetField(propertyOrFieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
      if (field2 != (FieldInfo) null)
        return Expression.Field(expression, field2);
      else
        throw System.Linq.Expressions.Error.NotAMemberOfType((object) propertyOrFieldName, (object) expression.Type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression"/> that represents accessing either a field or a property.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Linq.Expressions.MemberExpression"/> that results from calling the appropriate factory method.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the object that the member belongs to. This can be null for static members.</param><param name="member">The <see cref="T:System.Reflection.MemberInfo"/> that describes the field or property to be accessed.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="member"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="member"/> does not represent a field or property.</exception>
    [__DynamicallyInvokable]
    public static MemberExpression MakeMemberAccess(Expression expression, MemberInfo member)
    {
      ContractUtils.RequiresNotNull((object) member, "member");
      FieldInfo field = member as FieldInfo;
      if (field != (FieldInfo) null)
        return Expression.Field(expression, field);
      PropertyInfo property = member as PropertyInfo;
      if (property != (PropertyInfo) null)
        return Expression.Property(expression, property);
      else
        throw System.Linq.Expressions.Error.MemberNotFieldOrProperty((object) member);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberInitExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberInitExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberInit"/> and the <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression"/> and <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings"/> properties set to the specified values.
    /// </returns>
    /// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression"/> to set the <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression"/> property equal to.</param><param name="bindings">An array of <see cref="T:System.Linq.Expressions.MemberBinding"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="newExpression"/> or <paramref name="bindings"/> is null.</exception><exception cref="T:System.ArgumentException">The <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property of an element of <paramref name="bindings"/> does not represent a member of the type that <paramref name="newExpression"/>.Type represents.</exception>
    [__DynamicallyInvokable]
    public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
    {
      return Expression.MemberInit(newExpression, (IEnumerable<MemberBinding>) bindings);
    }

    /// <summary>
    /// Represents an expression that creates a new object and initializes a property of the object.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberInitExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.MemberInit"/> and the <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression"/> and <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings"/> properties set to the specified values.
    /// </returns>
    /// <param name="newExpression">A <see cref="T:System.Linq.Expressions.NewExpression"/> to set the <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression"/> property equal to.</param><param name="bindings">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.MemberBinding"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="newExpression"/> or <paramref name="bindings"/> is null.</exception><exception cref="T:System.ArgumentException">The <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property of an element of <paramref name="bindings"/> does not represent a member of the type that <paramref name="newExpression"/>.Type represents.</exception>
    [__DynamicallyInvokable]
    public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
    {
      ContractUtils.RequiresNotNull((object) newExpression, "newExpression");
      ContractUtils.RequiresNotNull((object) bindings, "bindings");
      ReadOnlyCollection<MemberBinding> bindings1 = CollectionExtensions.ToReadOnly<MemberBinding>(bindings);
      Expression.ValidateMemberInitArgs(newExpression.Type, bindings1);
      return new MemberInitExpression(newExpression, bindings1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberListBinding"/> where the member is a field or property.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberListBinding"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.ListBinding"/> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> and <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers"/> properties set to the specified values.
    /// </returns>
    /// <param name="member">A <see cref="T:System.Reflection.MemberInfo"/> that represents a field or property to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property equal to.</param><param name="initializers">An array of <see cref="T:System.Linq.Expressions.ElementInit"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="member"/> is null. -or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="member"/> does not represent a field or property.-or-The <see cref="P:System.Reflection.FieldInfo.FieldType"/> or <see cref="P:System.Reflection.PropertyInfo.PropertyType"/> of the field or property that <paramref name="member"/> represents does not implement <see cref="T:System.Collections.IEnumerable"/>.</exception>
    [__DynamicallyInvokable]
    public static MemberListBinding ListBind(MemberInfo member, params ElementInit[] initializers)
    {
      ContractUtils.RequiresNotNull((object) member, "member");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      return Expression.ListBind(member, (IEnumerable<ElementInit>) initializers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberListBinding"/> where the member is a field or property.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberListBinding"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.ListBinding"/> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> and <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers"/> properties set to the specified values.
    /// </returns>
    /// <param name="member">A <see cref="T:System.Reflection.MemberInfo"/> that represents a field or property to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property equal to.</param><param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ElementInit"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="member"/> is null. -or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="member"/> does not represent a field or property.-or-The <see cref="P:System.Reflection.FieldInfo.FieldType"/> or <see cref="P:System.Reflection.PropertyInfo.PropertyType"/> of the field or property that <paramref name="member"/> represents does not implement <see cref="T:System.Collections.IEnumerable"/>.</exception>
    [__DynamicallyInvokable]
    public static MemberListBinding ListBind(MemberInfo member, IEnumerable<ElementInit> initializers)
    {
      ContractUtils.RequiresNotNull((object) member, "member");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      Type memberType;
      Expression.ValidateGettableFieldOrPropertyMember(member, out memberType);
      ReadOnlyCollection<ElementInit> initializers1 = CollectionExtensions.ToReadOnly<ElementInit>(initializers);
      Expression.ValidateListInitArgs(memberType, initializers1);
      return new MemberListBinding(member, initializers1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberListBinding"/> object based on a specified property accessor method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberListBinding"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.ListBinding"/>, the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property set to the <see cref="T:System.Reflection.MemberInfo"/> that represents the property accessed in <paramref name="propertyAccessor"/>, and <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers"/> populated with the elements of <paramref name="initializers"/>.
    /// </returns>
    /// <param name="propertyAccessor">A <see cref="T:System.Reflection.MethodInfo"/> that represents a property accessor method.</param><param name="initializers">An array of <see cref="T:System.Linq.Expressions.ElementInit"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="propertyAccessor"/> is null. -or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="propertyAccessor"/> does not represent a property accessor method.-or-The <see cref="P:System.Reflection.PropertyInfo.PropertyType"/> of the property that the method represented by <paramref name="propertyAccessor"/> accesses does not implement <see cref="T:System.Collections.IEnumerable"/>.</exception>
    [__DynamicallyInvokable]
    public static MemberListBinding ListBind(MethodInfo propertyAccessor, params ElementInit[] initializers)
    {
      ContractUtils.RequiresNotNull((object) propertyAccessor, "propertyAccessor");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      return Expression.ListBind(propertyAccessor, (IEnumerable<ElementInit>) initializers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberListBinding"/> based on a specified property accessor method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberListBinding"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.ListBinding"/>, the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property set to the <see cref="T:System.Reflection.MemberInfo"/> that represents the property accessed in <paramref name="propertyAccessor"/>, and <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers"/> populated with the elements of <paramref name="initializers"/>.
    /// </returns>
    /// <param name="propertyAccessor">A <see cref="T:System.Reflection.MethodInfo"/> that represents a property accessor method.</param><param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.ElementInit"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="propertyAccessor"/> is null. -or-One or more elements of <paramref name="initializers"/> are null.</exception><exception cref="T:System.ArgumentException"><paramref name="propertyAccessor"/> does not represent a property accessor method.-or-The <see cref="P:System.Reflection.PropertyInfo.PropertyType"/> of the property that the method represented by <paramref name="propertyAccessor"/> accesses does not implement <see cref="T:System.Collections.IEnumerable"/>.</exception>
    [__DynamicallyInvokable]
    public static MemberListBinding ListBind(MethodInfo propertyAccessor, IEnumerable<ElementInit> initializers)
    {
      ContractUtils.RequiresNotNull((object) propertyAccessor, "propertyAccessor");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      return Expression.ListBind((MemberInfo) Expression.GetProperty(propertyAccessor), initializers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberMemberBinding"/> that represents the recursive initialization of members of a field or property.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberMemberBinding"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.MemberBinding"/> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> and <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings"/> properties set to the specified values.
    /// </returns>
    /// <param name="member">The <see cref="T:System.Reflection.MemberInfo"/> to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property equal to.</param><param name="bindings">An array of <see cref="T:System.Linq.Expressions.MemberBinding"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="member"/> or <paramref name="bindings"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="member"/> does not represent a field or property.-or-The <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property of an element of <paramref name="bindings"/> does not represent a member of the type of the field or property that <paramref name="member"/> represents.</exception>
    [__DynamicallyInvokable]
    public static MemberMemberBinding MemberBind(MemberInfo member, params MemberBinding[] bindings)
    {
      ContractUtils.RequiresNotNull((object) member, "member");
      ContractUtils.RequiresNotNull((object) bindings, "bindings");
      return Expression.MemberBind(member, (IEnumerable<MemberBinding>) bindings);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberMemberBinding"/> that represents the recursive initialization of members of a field or property.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberMemberBinding"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.MemberBinding"/> and the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> and <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings"/> properties set to the specified values.
    /// </returns>
    /// <param name="member">The <see cref="T:System.Reflection.MemberInfo"/> to set the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property equal to.</param><param name="bindings">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.MemberBinding"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="member"/> or <paramref name="bindings"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="member"/> does not represent a field or property.-or-The <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property of an element of <paramref name="bindings"/> does not represent a member of the type of the field or property that <paramref name="member"/> represents.</exception>
    [__DynamicallyInvokable]
    public static MemberMemberBinding MemberBind(MemberInfo member, IEnumerable<MemberBinding> bindings)
    {
      ContractUtils.RequiresNotNull((object) member, "member");
      ContractUtils.RequiresNotNull((object) bindings, "bindings");
      ReadOnlyCollection<MemberBinding> bindings1 = CollectionExtensions.ToReadOnly<MemberBinding>(bindings);
      Type memberType;
      Expression.ValidateGettableFieldOrPropertyMember(member, out memberType);
      Expression.ValidateMemberInitArgs(memberType, bindings1);
      return new MemberMemberBinding(member, bindings1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberMemberBinding"/> that represents the recursive initialization of members of a member that is accessed by using a property accessor method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberMemberBinding"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.MemberBinding"/>, the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property set to the <see cref="T:System.Reflection.PropertyInfo"/> that represents the property accessed in <paramref name="propertyAccessor"/>, and <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings"/> properties set to the specified values.
    /// </returns>
    /// <param name="propertyAccessor">The <see cref="T:System.Reflection.MethodInfo"/> that represents a property accessor method.</param><param name="bindings">An array of <see cref="T:System.Linq.Expressions.MemberBinding"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="propertyAccessor"/> or <paramref name="bindings"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="propertyAccessor"/> does not represent a property accessor method.-or-The <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property of an element of <paramref name="bindings"/> does not represent a member of the type of the property accessed by the method that <paramref name="propertyAccessor"/> represents.</exception>
    [__DynamicallyInvokable]
    public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, params MemberBinding[] bindings)
    {
      ContractUtils.RequiresNotNull((object) propertyAccessor, "propertyAccessor");
      return Expression.MemberBind((MemberInfo) Expression.GetProperty(propertyAccessor), bindings);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberMemberBinding"/> that represents the recursive initialization of members of a member that is accessed by using a property accessor method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MemberMemberBinding"/> that has the <see cref="P:System.Linq.Expressions.MemberBinding.BindingType"/> property equal to <see cref="F:System.Linq.Expressions.MemberBindingType.MemberBinding"/>, the <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property set to the <see cref="T:System.Reflection.PropertyInfo"/> that represents the property accessed in <paramref name="propertyAccessor"/>, and <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings"/> properties set to the specified values.
    /// </returns>
    /// <param name="propertyAccessor">The <see cref="T:System.Reflection.MethodInfo"/> that represents a property accessor method.</param><param name="bindings">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.MemberBinding"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MemberMemberBinding.Bindings"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="propertyAccessor"/> or <paramref name="bindings"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="propertyAccessor"/> does not represent a property accessor method.-or-The <see cref="P:System.Linq.Expressions.MemberBinding.Member"/> property of an element of <paramref name="bindings"/> does not represent a member of the type of the property accessed by the method that <paramref name="propertyAccessor"/> represents.</exception>
    [__DynamicallyInvokable]
    public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, IEnumerable<MemberBinding> bindings)
    {
      ContractUtils.RequiresNotNull((object) propertyAccessor, "propertyAccessor");
      return Expression.MemberBind((MemberInfo) Expression.GetProperty(propertyAccessor), bindings);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a static (Shared in Visual Basic) method that takes one argument.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><param name="arg0">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the first argument.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(MethodInfo method, Expression arg0)
    {
      ContractUtils.RequiresNotNull((object) method, "method");
      ContractUtils.RequiresNotNull((object) arg0, "arg0");
      ParameterInfo[] parameters = Expression.ValidateMethodAndGetParameters((Expression) null, method);
      Expression.ValidateArgumentCount((MethodBase) method, ExpressionType.Call, 1, parameters);
      arg0 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg0, parameters[0]);
      return (MethodCallExpression) new MethodCallExpression1(method, arg0);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a static method that takes two arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><param name="arg0">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the first argument.</param><param name="arg1">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the second argument.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(MethodInfo method, Expression arg0, Expression arg1)
    {
      ContractUtils.RequiresNotNull((object) method, "method");
      ContractUtils.RequiresNotNull((object) arg0, "arg0");
      ContractUtils.RequiresNotNull((object) arg1, "arg1");
      ParameterInfo[] parameters = Expression.ValidateMethodAndGetParameters((Expression) null, method);
      Expression.ValidateArgumentCount((MethodBase) method, ExpressionType.Call, 2, parameters);
      arg0 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg0, parameters[0]);
      arg1 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg1, parameters[1]);
      return (MethodCallExpression) new MethodCallExpression2(method, arg0, arg1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a static method that takes three arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><param name="arg0">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the first argument.</param><param name="arg1">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the second argument.</param><param name="arg2">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the third argument.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(MethodInfo method, Expression arg0, Expression arg1, Expression arg2)
    {
      ContractUtils.RequiresNotNull((object) method, "method");
      ContractUtils.RequiresNotNull((object) arg0, "arg0");
      ContractUtils.RequiresNotNull((object) arg1, "arg1");
      ContractUtils.RequiresNotNull((object) arg2, "arg2");
      ParameterInfo[] parameters = Expression.ValidateMethodAndGetParameters((Expression) null, method);
      Expression.ValidateArgumentCount((MethodBase) method, ExpressionType.Call, 3, parameters);
      arg0 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg0, parameters[0]);
      arg1 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg1, parameters[1]);
      arg2 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg2, parameters[2]);
      return (MethodCallExpression) new MethodCallExpression3(method, arg0, arg1, arg2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a static method that takes four arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><param name="arg0">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the first argument.</param><param name="arg1">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the second argument.</param><param name="arg2">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the third argument.</param><param name="arg3">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the fourth argument.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(MethodInfo method, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
    {
      ContractUtils.RequiresNotNull((object) method, "method");
      ContractUtils.RequiresNotNull((object) arg0, "arg0");
      ContractUtils.RequiresNotNull((object) arg1, "arg1");
      ContractUtils.RequiresNotNull((object) arg2, "arg2");
      ContractUtils.RequiresNotNull((object) arg3, "arg3");
      ParameterInfo[] parameters = Expression.ValidateMethodAndGetParameters((Expression) null, method);
      Expression.ValidateArgumentCount((MethodBase) method, ExpressionType.Call, 4, parameters);
      arg0 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg0, parameters[0]);
      arg1 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg1, parameters[1]);
      arg2 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg2, parameters[2]);
      arg3 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg3, parameters[3]);
      return (MethodCallExpression) new MethodCallExpression4(method, arg0, arg1, arg2, arg3);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a static method that takes five arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><param name="arg0">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the first argument.</param><param name="arg1">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the second argument.</param><param name="arg2">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the third argument.</param><param name="arg3">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the fourth argument.</param><param name="arg4">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the fifth argument.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(MethodInfo method, Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
    {
      ContractUtils.RequiresNotNull((object) method, "method");
      ContractUtils.RequiresNotNull((object) arg0, "arg0");
      ContractUtils.RequiresNotNull((object) arg1, "arg1");
      ContractUtils.RequiresNotNull((object) arg2, "arg2");
      ContractUtils.RequiresNotNull((object) arg3, "arg3");
      ContractUtils.RequiresNotNull((object) arg4, "arg4");
      ParameterInfo[] parameters = Expression.ValidateMethodAndGetParameters((Expression) null, method);
      Expression.ValidateArgumentCount((MethodBase) method, ExpressionType.Call, 5, parameters);
      arg0 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg0, parameters[0]);
      arg1 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg1, parameters[1]);
      arg2 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg2, parameters[2]);
      arg3 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg3, parameters[3]);
      arg4 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg4, parameters[4]);
      return (MethodCallExpression) new MethodCallExpression5(method, arg0, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a static (Shared in Visual Basic) method that has arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents a static (Shared in Visual Basic) method to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.</exception><exception cref="T:System.ArgumentException">The number of elements in <paramref name="arguments"/> does not equal the number of parameters for the method represented by <paramref name="method"/>.-or-One or more of the elements of <paramref name="arguments"/> is not assignable to the corresponding parameter for the method represented by <paramref name="method"/>.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static MethodCallExpression Call(MethodInfo method, params Expression[] arguments)
    {
      return Expression.Call((Expression) null, method, arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a static (Shared in Visual Basic) method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="method">The <see cref="T:System.Reflection.MethodInfo"/> that represents the target method.</param><param name="arguments">A collection of <see cref="T:System.Linq.Expressions.Expression"/> that represents the call arguments.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static MethodCallExpression Call(MethodInfo method, IEnumerable<Expression> arguments)
    {
      return Expression.Call((Expression) null, method, arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to an instance method that takes no arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression"/> that specifies the instance for an instance method call (pass null for a static (Shared in Visual Basic) method).</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.-or-<paramref name="instance"/> is null and <paramref name="method"/> represents an instance method.</exception><exception cref="T:System.ArgumentException"><paramref name="instance"/>.Type is not assignable to the declaring type of the method represented by <paramref name="method"/>.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(Expression instance, MethodInfo method)
    {
      return Expression.Call(instance, method, (IEnumerable<Expression>) EmptyReadOnlyCollection<Expression>.Instance);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a method that takes arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/>, <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/>, and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression"/> that specifies the instance fo an instance method call (pass null for a static (Shared in Visual Basic) method).</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.-or-<paramref name="instance"/> is null and <paramref name="method"/> represents an instance method.-or-<paramref name="arguments"/> is not null and one or more of its elements is null.</exception><exception cref="T:System.ArgumentException"><paramref name="instance"/>.Type is not assignable to the declaring type of the method represented by <paramref name="method"/>.-or-The number of elements in <paramref name="arguments"/> does not equal the number of parameters for the method represented by <paramref name="method"/>.-or-One or more of the elements of <paramref name="arguments"/> is not assignable to the corresponding parameter for the method represented by <paramref name="method"/>.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(Expression instance, MethodInfo method, params Expression[] arguments)
    {
      return Expression.Call(instance, method, (IEnumerable<Expression>) arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to an instance method that takes two arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression"/> that specifies the instance for an instance call. (pass null for a static (Shared in Visual Basic) method).</param><param name="method">The <see cref="T:System.Reflection.MethodInfo"/> that represents the target method.</param><param name="arg0">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the first argument.</param><param name="arg1">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the second argument.</param>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(Expression instance, MethodInfo method, Expression arg0, Expression arg1)
    {
      ContractUtils.RequiresNotNull((object) method, "method");
      ContractUtils.RequiresNotNull((object) arg0, "arg0");
      ContractUtils.RequiresNotNull((object) arg1, "arg1");
      ParameterInfo[] parameters = Expression.ValidateMethodAndGetParameters(instance, method);
      Expression.ValidateArgumentCount((MethodBase) method, ExpressionType.Call, 2, parameters);
      arg0 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg0, parameters[0]);
      arg1 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg1, parameters[1]);
      if (instance != null)
        return (MethodCallExpression) new InstanceMethodCallExpression2(method, instance, arg0, arg1);
      else
        return (MethodCallExpression) new MethodCallExpression2(method, arg0, arg1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a method that takes three arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression"/> that specifies the instance for an instance call. (pass null for a static (Shared in Visual Basic) method).</param><param name="method">The <see cref="T:System.Reflection.MethodInfo"/> that represents the target method.</param><param name="arg0">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the first argument.</param><param name="arg1">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the second argument.</param><param name="arg2">The <see cref="T:System.Linq.Expressions.Expression"/> that represents the third argument.</param>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(Expression instance, MethodInfo method, Expression arg0, Expression arg1, Expression arg2)
    {
      ContractUtils.RequiresNotNull((object) method, "method");
      ContractUtils.RequiresNotNull((object) arg0, "arg0");
      ContractUtils.RequiresNotNull((object) arg1, "arg1");
      ContractUtils.RequiresNotNull((object) arg2, "arg2");
      ParameterInfo[] parameters = Expression.ValidateMethodAndGetParameters(instance, method);
      Expression.ValidateArgumentCount((MethodBase) method, ExpressionType.Call, 3, parameters);
      arg0 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg0, parameters[0]);
      arg1 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg1, parameters[1]);
      arg2 = Expression.ValidateOneArgument((MethodBase) method, ExpressionType.Call, arg2, parameters[2]);
      if (instance != null)
        return (MethodCallExpression) new InstanceMethodCallExpression3(method, instance, arg0, arg1, arg2);
      else
        return (MethodCallExpression) new MethodCallExpression3(method, arg0, arg1, arg2);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to an instance method by calling the appropriate factory method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/>, the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> property equal to <paramref name="instance"/>, <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> set to the <see cref="T:System.Reflection.MethodInfo"/> that represents the specified instance method, and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> set to the specified arguments.
    /// </returns>
    /// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression"/> whose <see cref="P:System.Linq.Expressions.Expression.Type"/> property value will be searched for a specific method.</param><param name="methodName">The name of the method.</param><param name="typeArguments">An array of <see cref="T:System.Type"/> objects that specify the type parameters of the generic method. This argument should be null when methodName specifies a non-generic method.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects that represents the arguments to the method.</param><exception cref="T:System.ArgumentNullException"><paramref name="instance"/> or <paramref name="methodName"/> is null.</exception><exception cref="T:System.InvalidOperationException">No method whose name is <paramref name="methodName"/>, whose type parameters match <paramref name="typeArguments"/>, and whose parameter types match <paramref name="arguments"/> is found in <paramref name="instance"/>.Type or its base types.-or-More than one method whose name is <paramref name="methodName"/>, whose type parameters match <paramref name="typeArguments"/>, and whose parameter types match <paramref name="arguments"/> is found in <paramref name="instance"/>.Type or its base types.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(Expression instance, string methodName, Type[] typeArguments, params Expression[] arguments)
    {
      ContractUtils.RequiresNotNull((object) instance, "instance");
      ContractUtils.RequiresNotNull((object) methodName, "methodName");
      if (arguments == null)
        arguments = new Expression[0];
      BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
      return Expression.Call(instance, Expression.FindMethod(instance.Type, methodName, typeArguments, arguments, flags), arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a static (Shared in Visual Basic) method by calling the appropriate factory method.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/>, the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property set to the <see cref="T:System.Reflection.MethodInfo"/> that represents the specified static (Shared in Visual Basic) method, and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> property set to the specified arguments.
    /// </returns>
    /// <param name="type">The <see cref="T:System.Type"/> that specifies the type that contains the specified static (Shared in Visual Basic) method.</param><param name="methodName">The name of the method.</param><param name="typeArguments">An array of <see cref="T:System.Type"/> objects that specify the type parameters of the generic method. This argument should be null when methodName specifies a non-generic method.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects that represent the arguments to the method.</param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> or <paramref name="methodName"/> is null.</exception><exception cref="T:System.InvalidOperationException">No method whose name is <paramref name="methodName"/>, whose type parameters match <paramref name="typeArguments"/>, and whose parameter types match <paramref name="arguments"/> is found in <paramref name="type"/> or its base types.-or-More than one method whose name is <paramref name="methodName"/>, whose type parameters match <paramref name="typeArguments"/>, and whose parameter types match <paramref name="arguments"/> is found in <paramref name="type"/> or its base types.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(Type type, string methodName, Type[] typeArguments, params Expression[] arguments)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      ContractUtils.RequiresNotNull((object) methodName, "methodName");
      if (arguments == null)
        arguments = new Expression[0];
      BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
      return Expression.Call((Expression) null, Expression.FindMethod(type, methodName, typeArguments, arguments, flags), arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents a call to a method that takes arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/>, <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/>, and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="instance">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> property equal to (pass null for a static (Shared in Visual Basic) method).</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Method"/> property equal to.</param><param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="method"/> is null.-or-<paramref name="instance"/> is null and <paramref name="method"/> represents an instance method.</exception><exception cref="T:System.ArgumentException"><paramref name="instance"/>.Type is not assignable to the declaring type of the method represented by <paramref name="method"/>.-or-The number of elements in <paramref name="arguments"/> does not equal the number of parameters for the method represented by <paramref name="method"/>.-or-One or more of the elements of <paramref name="arguments"/> is not assignable to the corresponding parameter for the method represented by <paramref name="method"/>.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
    {
      ContractUtils.RequiresNotNull((object) method, "method");
      ReadOnlyCollection<Expression> arguments1 = CollectionExtensions.ToReadOnly<Expression>(arguments);
      Expression.ValidateMethodInfo(method);
      Expression.ValidateStaticOrInstanceMethod(instance, method);
      Expression.ValidateArgumentTypes((MethodBase) method, ExpressionType.Call, ref arguments1);
      if (instance == null)
        return (MethodCallExpression) new MethodCallExpressionN(method, (IList<Expression>) arguments1);
      else
        return (MethodCallExpression) new InstanceMethodCallExpressionN(method, instance, (IList<Expression>) arguments1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents applying an array index operator to a multidimensional array.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="array">An array of <see cref="T:System.Linq.Expressions.Expression"/> instances - indexes for the array index operation.</param><param name="indexes">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> or <paramref name="indexes"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/>.Type does not represent an array type.-or-The rank of <paramref name="array"/>.Type does not match the number of elements in <paramref name="indexes"/>.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of one or more elements of <paramref name="indexes"/> does not represent the <see cref="T:System.Int32"/> type.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression ArrayIndex(Expression array, params Expression[] indexes)
    {
      return Expression.ArrayIndex(array, (IEnumerable<Expression>) indexes);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that represents applying an array index operator to an array of rank more than one.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.MethodCallExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Call"/> and the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> and <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="array">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.MethodCallExpression.Object"/> property equal to.</param><param name="indexes">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.MethodCallExpression.Arguments"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> or <paramref name="indexes"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/>.Type does not represent an array type.-or-The rank of <paramref name="array"/>.Type does not match the number of elements in <paramref name="indexes"/>.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of one or more elements of <paramref name="indexes"/> does not represent the <see cref="T:System.Int32"/> type.</exception>
    [__DynamicallyInvokable]
    public static MethodCallExpression ArrayIndex(Expression array, IEnumerable<Expression> indexes)
    {
      Expression.RequiresCanRead(array, "array");
      ContractUtils.RequiresNotNull((object) indexes, "indexes");
      Type type = array.Type;
      if (!type.IsArray)
        throw System.Linq.Expressions.Error.ArgumentMustBeArray();
      ReadOnlyCollection<Expression> readOnlyCollection = CollectionExtensions.ToReadOnly<Expression>(indexes);
      if (type.GetArrayRank() != readOnlyCollection.Count)
        throw System.Linq.Expressions.Error.IncorrectNumberOfIndexes();
      foreach (Expression expression in readOnlyCollection)
      {
        Expression.RequiresCanRead(expression, "indexes");
        if (expression.Type != typeof (int))
          throw System.Linq.Expressions.Error.ArgumentMustBeArrayIndexType();
      }
      MethodInfo method = array.Type.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public);
      return Expression.Call(array, method, (IEnumerable<Expression>) readOnlyCollection);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewArrayExpression"/> that represents creating a one-dimensional array and initializing it from a list of elements.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewArrayExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayInit"/> and the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions"/> property set to the specified value.
    /// </returns>
    /// <param name="type">A <see cref="T:System.Type"/> that represents the element type of the array.</param><param name="initializers">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> or <paramref name="initializers"/> is null.-or-An element of <paramref name="initializers"/> is null.</exception><exception cref="T:System.InvalidOperationException">The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="initializers"/> represents a type that is not assignable to the type <paramref name="type"/>.</exception>
    [__DynamicallyInvokable]
    public static NewArrayExpression NewArrayInit(Type type, params Expression[] initializers)
    {
      return Expression.NewArrayInit(type, (IEnumerable<Expression>) initializers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewArrayExpression"/> that represents creating a one-dimensional array and initializing it from a list of elements.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewArrayExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayInit"/> and the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions"/> property set to the specified value.
    /// </returns>
    /// <param name="type">A <see cref="T:System.Type"/> that represents the element type of the array.</param><param name="initializers">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> or <paramref name="initializers"/> is null.-or-An element of <paramref name="initializers"/> is null.</exception><exception cref="T:System.InvalidOperationException">The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="initializers"/> represents a type that is not assignable to the type that <paramref name="type"/> represents.</exception>
    [__DynamicallyInvokable]
    public static NewArrayExpression NewArrayInit(Type type, IEnumerable<Expression> initializers)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      ContractUtils.RequiresNotNull((object) initializers, "initializers");
      if (type.Equals(typeof (void)))
        throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
      ReadOnlyCollection<Expression> expressions = CollectionExtensions.ToReadOnly<Expression>(initializers);
      Expression[] list = (Expression[]) null;
      int index1 = 0;
      for (int count = expressions.Count; index1 < count; ++index1)
      {
        Expression expression = expressions[index1];
        Expression.RequiresCanRead(expression, "initializers");
        if (!TypeUtils.AreReferenceAssignable(type, expression.Type))
        {
          if (!Expression.TryQuote(type, ref expression))
            throw System.Linq.Expressions.Error.ExpressionTypeCannotInitializeArrayType((object) expression.Type, (object) type);
          if (list == null)
          {
            list = new Expression[expressions.Count];
            for (int index2 = 0; index2 < index1; ++index2)
              list[index2] = expressions[index2];
          }
        }
        if (list != null)
          list[index1] = expression;
      }
      if (list != null)
        expressions = (ReadOnlyCollection<Expression>) new TrueReadOnlyCollection<Expression>(list);
      return NewArrayExpression.Make(ExpressionType.NewArrayInit, type.MakeArrayType(), expressions);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewArrayExpression"/> that represents creating an array that has a specified rank.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewArrayExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayBounds"/> and the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions"/> property set to the specified value.
    /// </returns>
    /// <param name="type">A <see cref="T:System.Type"/> that represents the element type of the array.</param><param name="bounds">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> or <paramref name="bounds"/> is null.-or-An element of <paramref name="bounds"/> is null.</exception><exception cref="T:System.ArgumentException">The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="bounds"/> does not represent an integral type.</exception>
    [__DynamicallyInvokable]
    public static NewArrayExpression NewArrayBounds(Type type, params Expression[] bounds)
    {
      return Expression.NewArrayBounds(type, (IEnumerable<Expression>) bounds);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewArrayExpression"/> that represents creating an array that has a specified rank.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewArrayExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayBounds"/> and the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions"/> property set to the specified value.
    /// </returns>
    /// <param name="type">A <see cref="T:System.Type"/> that represents the element type of the array.</param><param name="bounds">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> or <paramref name="bounds"/> is null.-or-An element of <paramref name="bounds"/> is null.</exception><exception cref="T:System.ArgumentException">The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="bounds"/> does not represent an integral type.</exception>
    [__DynamicallyInvokable]
    public static NewArrayExpression NewArrayBounds(Type type, IEnumerable<Expression> bounds)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      ContractUtils.RequiresNotNull((object) bounds, "bounds");
      if (type.Equals(typeof (void)))
        throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
      ReadOnlyCollection<Expression> readOnlyCollection = CollectionExtensions.ToReadOnly<Expression>(bounds);
      int count = readOnlyCollection.Count;
      if (count <= 0)
        throw System.Linq.Expressions.Error.BoundsCannotBeLessThanOne();
      for (int index = 0; index < count; ++index)
      {
        Expression expression = readOnlyCollection[index];
        Expression.RequiresCanRead(expression, "bounds");
        if (!TypeUtils.IsInteger(expression.Type))
          throw System.Linq.Expressions.Error.ArgumentMustBeInteger();
      }
      return NewArrayExpression.Make(ExpressionType.NewArrayBounds, count != 1 ? type.MakeArrayType(count) : type.MakeArrayType(), CollectionExtensions.ToReadOnly<Expression>(bounds));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewExpression"/> that represents calling the specified constructor that takes no arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New"/> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> property set to the specified value.
    /// </returns>
    /// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo"/> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="constructor"/> is null.</exception><exception cref="T:System.ArgumentException">The constructor that <paramref name="constructor"/> represents has at least one parameter.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static NewExpression New(ConstructorInfo constructor)
    {
      return Expression.New(constructor, (IEnumerable<Expression>) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewExpression"/> that represents calling the specified constructor with the specified arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New"/> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> and <see cref="P:System.Linq.Expressions.NewExpression.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo"/> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> property equal to.</param><param name="arguments">An array of <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Arguments"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="constructor"/> is null.-or-An element of <paramref name="arguments"/> is null.</exception><exception cref="T:System.ArgumentException">The length of <paramref name="arguments"/> does match the number of parameters for the constructor that <paramref name="constructor"/> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the constructor that <paramref name="constructor"/> represents.</exception>
    [__DynamicallyInvokable]
    public static NewExpression New(ConstructorInfo constructor, params Expression[] arguments)
    {
      return Expression.New(constructor, (IEnumerable<Expression>) arguments);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewExpression"/> that represents calling the specified constructor with the specified arguments.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New"/> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> and <see cref="P:System.Linq.Expressions.NewExpression.Arguments"/> properties set to the specified values.
    /// </returns>
    /// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo"/> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> property equal to.</param><param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Arguments"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="constructor"/> is null.-or-An element of <paramref name="arguments"/> is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="arguments"/> parameter does not contain the same number of elements as the number of parameters for the constructor that <paramref name="constructor"/> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the constructor that <paramref name="constructor"/> represents.</exception>
    [__DynamicallyInvokable]
    public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments)
    {
      ContractUtils.RequiresNotNull((object) constructor, "constructor");
      ContractUtils.RequiresNotNull((object) constructor.DeclaringType, "constructor.DeclaringType");
      TypeUtils.ValidateType(constructor.DeclaringType);
      ReadOnlyCollection<Expression> arguments1 = CollectionExtensions.ToReadOnly<Expression>(arguments);
      Expression.ValidateArgumentTypes((MethodBase) constructor, ExpressionType.New, ref arguments1);
      return new NewExpression(constructor, (IList<Expression>) arguments1, (ReadOnlyCollection<MemberInfo>) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewExpression"/> that represents calling the specified constructor with the specified arguments. The members that access the constructor initialized fields are specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New"/> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/>, <see cref="P:System.Linq.Expressions.NewExpression.Arguments"/> and <see cref="P:System.Linq.Expressions.NewExpression.Members"/> properties set to the specified values.
    /// </returns>
    /// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo"/> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> property equal to.</param><param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Arguments"/> collection.</param><param name="members">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Reflection.MemberInfo"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Members"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="constructor"/> is null.-or-An element of <paramref name="arguments"/> is null.-or-An element of <paramref name="members"/> is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="arguments"/> parameter does not contain the same number of elements as the number of parameters for the constructor that <paramref name="constructor"/> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the constructor that <paramref name="constructor"/> represents.-or-The <paramref name="members"/> parameter does not have the same number of elements as <paramref name="arguments"/>.-or-An element of <paramref name="arguments"/> has a <see cref="P:System.Linq.Expressions.Expression.Type"/> property that represents a type that is not assignable to the type of the member that is represented by the corresponding element of <paramref name="members"/>.</exception>
    [__DynamicallyInvokable]
    public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, IEnumerable<MemberInfo> members)
    {
      ContractUtils.RequiresNotNull((object) constructor, "constructor");
      ReadOnlyCollection<MemberInfo> members1 = CollectionExtensions.ToReadOnly<MemberInfo>(members);
      ReadOnlyCollection<Expression> arguments1 = CollectionExtensions.ToReadOnly<Expression>(arguments);
      Expression.ValidateNewArgs(constructor, ref arguments1, ref members1);
      return new NewExpression(constructor, (IList<Expression>) arguments1, members1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewExpression"/> that represents calling the specified constructor with the specified arguments. The members that access the constructor initialized fields are specified as an array.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New"/> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/>, <see cref="P:System.Linq.Expressions.NewExpression.Arguments"/> and <see cref="P:System.Linq.Expressions.NewExpression.Members"/> properties set to the specified values.
    /// </returns>
    /// <param name="constructor">The <see cref="T:System.Reflection.ConstructorInfo"/> to set the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> property equal to.</param><param name="arguments">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <see cref="T:System.Linq.Expressions.Expression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Arguments"/> collection.</param><param name="members">An array of <see cref="T:System.Reflection.MemberInfo"/> objects to use to populate the <see cref="P:System.Linq.Expressions.NewExpression.Members"/> collection.</param><exception cref="T:System.ArgumentNullException"><paramref name="constructor"/> is null.-or-An element of <paramref name="arguments"/> is null.-or-An element of <paramref name="members"/> is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="arguments"/> parameter does not contain the same number of elements as the number of parameters for the constructor that <paramref name="constructor"/> represents.-or-The <see cref="P:System.Linq.Expressions.Expression.Type"/> property of an element of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the constructor that <paramref name="constructor"/> represents.-or-The <paramref name="members"/> parameter does not have the same number of elements as <paramref name="arguments"/>.-or-An element of <paramref name="arguments"/> has a <see cref="P:System.Linq.Expressions.Expression.Type"/> property that represents a type that is not assignable to the type of the member that is represented by the corresponding element of <paramref name="members"/>.</exception>
    [__DynamicallyInvokable]
    public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, params MemberInfo[] members)
    {
      return Expression.New(constructor, arguments, (IEnumerable<MemberInfo>) members);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.NewExpression"/> that represents calling the parameterless constructor of the specified type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.NewExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.New"/> and the <see cref="P:System.Linq.Expressions.NewExpression.Constructor"/> property set to the <see cref="T:System.Reflection.ConstructorInfo"/> that represents the constructor without parameters for the specified type.
    /// </returns>
    /// <param name="type">A <see cref="T:System.Type"/> that has a constructor that takes no arguments.</param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> is null.</exception><exception cref="T:System.ArgumentException">The type that <paramref name="type"/> represents does not have a constructor without parameters.</exception>
    [__DynamicallyInvokable]
    public static NewExpression New(Type type)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      if (type == typeof (void))
        throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
      if (type.IsValueType)
        return (NewExpression) new NewValueTypeExpression(type, EmptyReadOnlyCollection<Expression>.Instance, (ReadOnlyCollection<MemberInfo>) null);
      ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null);
      if (constructor == (ConstructorInfo) null)
        throw System.Linq.Expressions.Error.TypeMissingDefaultConstructor((object) type);
      else
        return Expression.New(constructor);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ParameterExpression"/> node that can be used to identify a parameter or a variable in an expression tree.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ParameterExpression"/> node with the specified name and type.
    /// </returns>
    /// <param name="type">The type of the parameter or variable.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static ParameterExpression Parameter(Type type)
    {
      return Expression.Parameter(type, (string) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ParameterExpression"/> node that can be used to identify a parameter or a variable in an expression tree.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ParameterExpression"/> node with the specified name and type
    /// </returns>
    /// <param name="type">The type of the parameter or variable.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static ParameterExpression Variable(Type type)
    {
      return Expression.Variable(type, (string) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ParameterExpression"/> node that can be used to identify a parameter or a variable in an expression tree.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ParameterExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Parameter"/> and the <see cref="P:System.Linq.Expressions.Expression.Type"/> and <see cref="P:System.Linq.Expressions.ParameterExpression.Name"/> properties set to the specified values.
    /// </returns>
    /// <param name="type">The type of the parameter or variable.</param><param name="name">The name of the parameter or variable, used for debugging or printing purpose only.</param><exception cref="T:System.ArgumentNullException"><paramref name="type"/> is null.</exception>
    [__DynamicallyInvokable]
    public static ParameterExpression Parameter(Type type, string name)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      if (type == typeof (void))
        throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
      bool isByRef = type.IsByRef;
      if (isByRef)
        type = type.GetElementType();
      return ParameterExpression.Make(type, name, isByRef);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.ParameterExpression"/> node that can be used to identify a parameter or a variable in an expression tree.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.ParameterExpression"/> node with the specified name and type.
    /// </returns>
    /// <param name="type">The type of the parameter or variable.</param><param name="name">The name of the parameter or variable. This name is used for debugging or printing purpose only.</param>
    [__DynamicallyInvokable]
    public static ParameterExpression Variable(Type type, string name)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      if (type == typeof (void))
        throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
      if (type.IsByRef)
        throw System.Linq.Expressions.Error.TypeMustNotBeByRef();
      else
        return ParameterExpression.Make(type, name, false);
    }

    /// <summary>
    /// Creates an instance of <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RuntimeVariables"/> and the <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables"/> property set to the specified value.
    /// </returns>
    /// <param name="variables">An array of <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables"/> collection.</param>
    [__DynamicallyInvokable]
    public static RuntimeVariablesExpression RuntimeVariables(params ParameterExpression[] variables)
    {
      return Expression.RuntimeVariables((IEnumerable<ParameterExpression>) variables);
    }

    /// <summary>
    /// Creates an instance of <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.RuntimeVariables"/> and the <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables"/> property set to the specified value.
    /// </returns>
    /// <param name="variables">A collection of <see cref="T:System.Linq.Expressions.ParameterExpression"/> objects to use to populate the <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables"/> collection.</param>
    [__DynamicallyInvokable]
    public static RuntimeVariablesExpression RuntimeVariables(IEnumerable<ParameterExpression> variables)
    {
      ContractUtils.RequiresNotNull((object) variables, "variables");
      ReadOnlyCollection<ParameterExpression> variables1 = CollectionExtensions.ToReadOnly<ParameterExpression>(variables);
      for (int index = 0; index < variables1.Count; ++index)
      {
        if ((Expression) variables1[index] == null)
          throw new ArgumentNullException("variables[" + (object) index + "]");
      }
      return new RuntimeVariablesExpression(variables1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.SwitchCase"/> for use in a <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.SwitchCase"/>.
    /// </returns>
    /// <param name="body">The body of the case.</param><param name="testValues">The test values of the case.</param>
    [__DynamicallyInvokable]
    public static SwitchCase SwitchCase(Expression body, params Expression[] testValues)
    {
      return Expression.SwitchCase(body, (IEnumerable<Expression>) testValues);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.SwitchCase"/> object to be used in a <see cref="T:System.Linq.Expressions.SwitchExpression"/> object.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.SwitchCase"/>.
    /// </returns>
    /// <param name="body">The body of the case.</param><param name="testValues">The test values of the case.</param>
    [__DynamicallyInvokable]
    public static SwitchCase SwitchCase(Expression body, IEnumerable<Expression> testValues)
    {
      Expression.RequiresCanRead(body, "body");
      ReadOnlyCollection<Expression> testValues1 = CollectionExtensions.ToReadOnly<Expression>(testValues);
      Expression.RequiresCanRead((IEnumerable<Expression>) testValues1, "testValues");
      ContractUtils.RequiresNotEmpty<Expression>((ICollection<Expression>) testValues1, "testValues");
      return new SwitchCase(body, testValues1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.SwitchExpression"/> that represents a switch statement without a default case.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
    /// </returns>
    /// <param name="switchValue">The value to be tested against each case.</param><param name="cases">The set of cases for this switch expression.</param>
    [__DynamicallyInvokable]
    public static SwitchExpression Switch(Expression switchValue, params SwitchCase[] cases)
    {
      return Expression.Switch(switchValue, (Expression) null, (MethodInfo) null, (IEnumerable<SwitchCase>) cases);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.SwitchExpression"/> that represents a switch statement that has a default case.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
    /// </returns>
    /// <param name="switchValue">The value to be tested against each case.</param><param name="defaultBody">The result of the switch if <paramref name="switchValue"/> does not match any of the cases.</param><param name="cases">The set of cases for this switch expression.</param>
    [__DynamicallyInvokable]
    public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, params SwitchCase[] cases)
    {
      return Expression.Switch(switchValue, defaultBody, (MethodInfo) null, (IEnumerable<SwitchCase>) cases);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.SwitchExpression"/> that represents a switch statement that has a default case.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
    /// </returns>
    /// <param name="switchValue">The value to be tested against each case.</param><param name="defaultBody">The result of the switch if <paramref name="switchValue"/> does not match any of the cases.</param><param name="comparison">The equality comparison method to use.</param><param name="cases">The set of cases for this switch expression.</param>
    [__DynamicallyInvokable]
    public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, MethodInfo comparison, params SwitchCase[] cases)
    {
      return Expression.Switch(switchValue, defaultBody, comparison, (IEnumerable<SwitchCase>) cases);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.SwitchExpression"/> that represents a switch statement that has a default case..
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
    /// </returns>
    /// <param name="type">The result type of the switch.</param><param name="switchValue">The value to be tested against each case.</param><param name="defaultBody">The result of the switch if <paramref name="switchValue"/> does not match any of the cases.</param><param name="comparison">The equality comparison method to use.</param><param name="cases">The set of cases for this switch expression.</param>
    [__DynamicallyInvokable]
    public static SwitchExpression Switch(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, params SwitchCase[] cases)
    {
      return Expression.Switch(type, switchValue, defaultBody, comparison, (IEnumerable<SwitchCase>) cases);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.SwitchExpression"/> that represents a switch statement that has a default case.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
    /// </returns>
    /// <param name="switchValue">The value to be tested against each case.</param><param name="defaultBody">The result of the switch if <paramref name="switchValue"/> does not match any of the cases.</param><param name="comparison">The equality comparison method to use.</param><param name="cases">The set of cases for this switch expression.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, MethodInfo comparison, IEnumerable<SwitchCase> cases)
    {
      return Expression.Switch((Type) null, switchValue, defaultBody, comparison, cases);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.SwitchExpression"/> that represents a switch statement that has a default case.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
    /// </returns>
    /// <param name="type">The result type of the switch.</param><param name="switchValue">The value to be tested against each case.</param><param name="defaultBody">The result of the switch if <paramref name="switchValue"/> does not match any of the cases.</param><param name="comparison">The equality comparison method to use.</param><param name="cases">The set of cases for this switch expression.</param>
    [__DynamicallyInvokable]
    public static SwitchExpression Switch(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, IEnumerable<SwitchCase> cases)
    {
      Expression.RequiresCanRead(switchValue, "switchValue");
      if (switchValue.Type == typeof (void))
        throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
      ReadOnlyCollection<SwitchCase> cases1 = CollectionExtensions.ToReadOnly<SwitchCase>(cases);
      ContractUtils.RequiresNotEmpty<SwitchCase>((ICollection<SwitchCase>) cases1, "cases");
      ContractUtils.RequiresNotNullItems<SwitchCase>((IList<SwitchCase>) cases1, "cases");
      Type type1 = type ?? cases1[0].Body.Type;
      bool customType = type != (Type) null;
      if (comparison != (MethodInfo) null)
      {
        ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) comparison);
        if (parametersCached.Length != 2)
          throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) comparison);
        ParameterInfo pi1 = parametersCached[0];
        bool flag = false;
        if (!Expression.ParameterIsAssignable(pi1, switchValue.Type))
        {
          flag = Expression.ParameterIsAssignable(pi1, TypeUtils.GetNonNullableType(switchValue.Type));
          if (!flag)
            throw System.Linq.Expressions.Error.SwitchValueTypeDoesNotMatchComparisonMethodParameter((object) switchValue.Type, (object) pi1.ParameterType);
        }
        ParameterInfo pi2 = parametersCached[1];
        foreach (SwitchCase switchCase in cases1)
        {
          ContractUtils.RequiresNotNull((object) switchCase, "cases");
          Expression.ValidateSwitchCaseType(switchCase.Body, customType, type1, "cases");
          for (int index = 0; index < switchCase.TestValues.Count; ++index)
          {
            Type type2 = switchCase.TestValues[index].Type;
            if (flag)
            {
              if (!TypeUtils.IsNullableType(type2))
                throw System.Linq.Expressions.Error.TestValueTypeDoesNotMatchComparisonMethodParameter((object) type2, (object) pi2.ParameterType);
              type2 = TypeUtils.GetNonNullableType(type2);
            }
            if (!Expression.ParameterIsAssignable(pi2, type2))
              throw System.Linq.Expressions.Error.TestValueTypeDoesNotMatchComparisonMethodParameter((object) type2, (object) pi2.ParameterType);
          }
        }
      }
      else
      {
        Expression right = cases1[0].TestValues[0];
        foreach (SwitchCase switchCase in cases1)
        {
          ContractUtils.RequiresNotNull((object) switchCase, "cases");
          Expression.ValidateSwitchCaseType(switchCase.Body, customType, type1, "cases");
          for (int index = 0; index < switchCase.TestValues.Count; ++index)
          {
            if (!TypeUtils.AreEquivalent(right.Type, switchCase.TestValues[index].Type))
              throw new ArgumentException(System.Linq.Expressions.Strings.AllTestValuesMustHaveSameType, "cases");
          }
        }
        comparison = Expression.Equal(switchValue, right, false, comparison).Method;
      }
      if (defaultBody == null)
      {
        if (type1 != typeof (void))
          throw System.Linq.Expressions.Error.DefaultBodyMustBeSupplied();
      }
      else
        Expression.ValidateSwitchCaseType(defaultBody, customType, type1, "defaultBody");
      if (comparison != (MethodInfo) null && comparison.ReturnType != typeof (bool))
        throw System.Linq.Expressions.Error.EqualityMustReturnBoolean((object) comparison);
      else
        return new SwitchExpression(type1, switchValue, defaultBody, comparison, cases1);
    }

    /// <summary>
    /// Creates an instance of <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/> that has the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName"/> property set to the specified value.
    /// </returns>
    /// <param name="fileName">A <see cref="T:System.String"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName"/> equal to.</param>
    [__DynamicallyInvokable]
    public static SymbolDocumentInfo SymbolDocument(string fileName)
    {
      return new SymbolDocumentInfo(fileName);
    }

    /// <summary>
    /// Creates an instance of <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/> that has the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName"/> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language"/> properties set to the specified value.
    /// </returns>
    /// <param name="fileName">A <see cref="T:System.String"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName"/> equal to.</param><param name="language">A <see cref="T:System.Guid"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language"/> equal to.</param>
    [__DynamicallyInvokable]
    public static SymbolDocumentInfo SymbolDocument(string fileName, Guid language)
    {
      return (SymbolDocumentInfo) new SymbolDocumentWithGuids(fileName, ref language);
    }

    /// <summary>
    /// Creates an instance of <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/> that has the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName"/> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language"/> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.LanguageVendor"/> properties set to the specified value.
    /// </returns>
    /// <param name="fileName">A <see cref="T:System.String"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName"/> equal to.</param><param name="language">A <see cref="T:System.Guid"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language"/> equal to.</param><param name="languageVendor">A <see cref="T:System.Guid"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.LanguageVendor"/> equal to.</param>
    [__DynamicallyInvokable]
    public static SymbolDocumentInfo SymbolDocument(string fileName, Guid language, Guid languageVendor)
    {
      return (SymbolDocumentInfo) new SymbolDocumentWithGuids(fileName, ref language, ref languageVendor);
    }

    /// <summary>
    /// Creates an instance of <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.SymbolDocumentInfo"/> that has the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName"/> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language"/> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.LanguageVendor"/> and <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.DocumentType"/> properties set to the specified value.
    /// </returns>
    /// <param name="fileName">A <see cref="T:System.String"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.FileName"/> equal to.</param><param name="language">A <see cref="T:System.Guid"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.Language"/> equal to.</param><param name="languageVendor">A <see cref="T:System.Guid"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.LanguageVendor"/> equal to.</param><param name="documentType">A <see cref="T:System.Guid"/> to set the <see cref="P:System.Linq.Expressions.SymbolDocumentInfo.DocumentType"/> equal to.</param>
    [__DynamicallyInvokable]
    public static SymbolDocumentInfo SymbolDocument(string fileName, Guid language, Guid languageVendor, Guid documentType)
    {
      return (SymbolDocumentInfo) new SymbolDocumentWithGuids(fileName, ref language, ref languageVendor, ref documentType);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.TryExpression"/> representing a try block with a fault block and no catch statements.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.TryExpression"/>.
    /// </returns>
    /// <param name="body">The body of the try block.</param><param name="fault">The body of the fault block.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static TryExpression TryFault(Expression body, Expression fault)
    {
      return Expression.MakeTry((Type) null, body, (Expression) null, fault, (IEnumerable<CatchBlock>) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.TryExpression"/> representing a try block with a finally block and no catch statements.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.TryExpression"/>.
    /// </returns>
    /// <param name="body">The body of the try block.</param><param name="finally">The body of the finally block.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static TryExpression TryFinally(Expression body, Expression @finally)
    {
      return Expression.MakeTry((Type) null, body, @finally, (Expression) null, (IEnumerable<CatchBlock>) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.TryExpression"/> representing a try block with any number of catch statements and neither a fault nor finally block.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.TryExpression"/>.
    /// </returns>
    /// <param name="body">The body of the try block.</param><param name="handlers">The array of zero or more <see cref="T:System.Linq.Expressions.CatchBlock"/> expressions representing the catch statements to be associated with the try block.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static TryExpression TryCatch(Expression body, params CatchBlock[] handlers)
    {
      return Expression.MakeTry((Type) null, body, (Expression) null, (Expression) null, (IEnumerable<CatchBlock>) handlers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.TryExpression"/> representing a try block with any number of catch statements and a finally block.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.TryExpression"/>.
    /// </returns>
    /// <param name="body">The body of the try block.</param><param name="finally">The body of the finally block.</param><param name="handlers">The array of zero or more <see cref="T:System.Linq.Expressions.CatchBlock"/> expressions representing the catch statements to be associated with the try block.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static TryExpression TryCatchFinally(Expression body, Expression @finally, params CatchBlock[] handlers)
    {
      return Expression.MakeTry((Type) null, body, @finally, (Expression) null, (IEnumerable<CatchBlock>) handlers);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.TryExpression"/> representing a try block with the specified elements.
    /// </summary>
    /// 
    /// <returns>
    /// The created <see cref="T:System.Linq.Expressions.TryExpression"/>.
    /// </returns>
    /// <param name="type">The result type of the try expression. If null, bodh and all handlers must have identical type.</param><param name="body">The body of the try block.</param><param name="finally">The body of the finally block. Pass null if the try block has no finally block associated with it.</param><param name="fault">The body of the try block. Pass null if the try block has no fault block associated with it.</param><param name="handlers">A collection of <see cref="T:System.Linq.Expressions.CatchBlock"/>s representing the catch statements to be associated with the try block.</param>
    [__DynamicallyInvokable]
    public static TryExpression MakeTry(Type type, Expression body, Expression @finally, Expression fault, IEnumerable<CatchBlock> handlers)
    {
      Expression.RequiresCanRead(body, "body");
      ReadOnlyCollection<CatchBlock> handlers1 = CollectionExtensions.ToReadOnly<CatchBlock>(handlers);
      ContractUtils.RequiresNotNullItems<CatchBlock>((IList<CatchBlock>) handlers1, "handlers");
      Expression.ValidateTryAndCatchHaveSameType(type, body, handlers1);
      if (fault != null)
      {
        if (@finally != null || handlers1.Count > 0)
          throw System.Linq.Expressions.Error.FaultCannotHaveCatchOrFinally();
        Expression.RequiresCanRead(fault, "fault");
      }
      else if (@finally != null)
        Expression.RequiresCanRead(@finally, "finally");
      else if (handlers1.Count == 0)
        throw System.Linq.Expressions.Error.TryMustHaveCatchFinallyOrFault();
      return new TryExpression(type ?? body.Type, body, @finally, fault, handlers1);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.TypeBinaryExpression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.TypeBinaryExpression"/> for which the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property is equal to <see cref="F:System.Linq.Expressions.ExpressionType.TypeIs"/> and for which the <see cref="P:System.Linq.Expressions.TypeBinaryExpression.Expression"/> and <see cref="P:System.Linq.Expressions.TypeBinaryExpression.TypeOperand"/> properties are set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.TypeBinaryExpression.Expression"/> property equal to.</param><param name="type">A <see cref="P:System.Linq.Expressions.Expression.Type"/> to set the <see cref="P:System.Linq.Expressions.TypeBinaryExpression.TypeOperand"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="type"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TypeBinaryExpression TypeIs(Expression expression, Type type)
    {
      Expression.RequiresCanRead(expression, "expression");
      ContractUtils.RequiresNotNull((object) type, "type");
      if (type.IsByRef)
        throw System.Linq.Expressions.Error.TypeMustNotBeByRef();
      else
        return new TypeBinaryExpression(expression, type, ExpressionType.TypeIs);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.TypeBinaryExpression"/> that compares run-time type identity.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.TypeBinaryExpression"/> for which the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property is equal to <see cref="M:System.Linq.Expressions.Expression.TypeEqual(System.Linq.Expressions.Expression,System.Type)"/> and for which the <see cref="T:System.Linq.Expressions.Expression"/> and <see cref="P:System.Linq.Expressions.TypeBinaryExpression.TypeOperand"/> properties are set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="T:System.Linq.Expressions.Expression"/> property equal to.</param><param name="type">A <see cref="P:System.Linq.Expressions.Expression.Type"/> to set the <see cref="P:System.Linq.Expressions.TypeBinaryExpression.TypeOperand"/> property equal to.</param>
    [__DynamicallyInvokable]
    public static TypeBinaryExpression TypeEqual(Expression expression, Type type)
    {
      Expression.RequiresCanRead(expression, "expression");
      ContractUtils.RequiresNotNull((object) type, "type");
      if (type.IsByRef)
        throw System.Linq.Expressions.Error.TypeMustNotBeByRef();
      else
        return new TypeBinaryExpression(expression, type, ExpressionType.TypeEqual);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/>, given an operand, by calling the appropriate factory method.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Linq.Expressions.UnaryExpression"/> that results from calling the appropriate factory method.
    /// </returns>
    /// <param name="unaryType">The <see cref="T:System.Linq.Expressions.ExpressionType"/> that specifies the type of unary operation.</param><param name="operand">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the operand.</param><param name="type">The <see cref="T:System.Type"/> that specifies the type to be converted to (pass null if not applicable).</param><exception cref="T:System.ArgumentNullException"><paramref name="operand"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="unaryType"/> does not correspond to a unary expression node.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type)
    {
      return Expression.MakeUnary(unaryType, operand, type, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/>, given an operand and implementing method, by calling the appropriate factory method.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Linq.Expressions.UnaryExpression"/> that results from calling the appropriate factory method.
    /// </returns>
    /// <param name="unaryType">The <see cref="T:System.Linq.Expressions.ExpressionType"/> that specifies the type of unary operation.</param><param name="operand">An <see cref="T:System.Linq.Expressions.Expression"/> that represents the operand.</param><param name="type">The <see cref="T:System.Type"/> that specifies the type to be converted to (pass null if not applicable).</param><param name="method">The <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param><exception cref="T:System.ArgumentNullException"><paramref name="operand"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="unaryType"/> does not correspond to a unary expression node.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type, MethodInfo method)
    {
      switch (unaryType)
      {
        case ExpressionType.Increment:
          return Expression.Increment(operand, method);
        case ExpressionType.Throw:
          return Expression.Throw(operand, type);
        case ExpressionType.Unbox:
          return Expression.Unbox(operand, type);
        case ExpressionType.PreIncrementAssign:
          return Expression.PreIncrementAssign(operand, method);
        case ExpressionType.PreDecrementAssign:
          return Expression.PreDecrementAssign(operand, method);
        case ExpressionType.PostIncrementAssign:
          return Expression.PostIncrementAssign(operand, method);
        case ExpressionType.PostDecrementAssign:
          return Expression.PostDecrementAssign(operand, method);
        case ExpressionType.OnesComplement:
          return Expression.OnesComplement(operand, method);
        case ExpressionType.IsTrue:
          return Expression.IsTrue(operand, method);
        case ExpressionType.IsFalse:
          return Expression.IsFalse(operand, method);
        case ExpressionType.TypeAs:
          return Expression.TypeAs(operand, type);
        case ExpressionType.Decrement:
          return Expression.Decrement(operand, method);
        case ExpressionType.Negate:
          return Expression.Negate(operand, method);
        case ExpressionType.UnaryPlus:
          return Expression.UnaryPlus(operand, method);
        case ExpressionType.NegateChecked:
          return Expression.NegateChecked(operand, method);
        case ExpressionType.Not:
          return Expression.Not(operand, method);
        case ExpressionType.Quote:
          return Expression.Quote(operand);
        case ExpressionType.ArrayLength:
          return Expression.ArrayLength(operand);
        case ExpressionType.Convert:
          return Expression.Convert(operand, type, method);
        case ExpressionType.ConvertChecked:
          return Expression.ConvertChecked(operand, type, method);
        default:
          throw System.Linq.Expressions.Error.UnhandledUnary((object) unaryType);
      }
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents an arithmetic negation operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Negate"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property set to the specified value.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.InvalidOperationException">The unary minus operator is not defined for <paramref name="expression"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression Negate(Expression expression)
    {
      return Expression.Negate(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents an arithmetic negation operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Negate"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> and <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly one argument.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the unary minus operator is not defined for <paramref name="expression"/>.Type.-or-<paramref name="expression"/>.Type (or its corresponding non-nullable type if it is a nullable value type) is not assignable to the argument type of the method represented by <paramref name="method"/>.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression Negate(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.Negate, expression, method);
      if (TypeUtils.IsArithmetic(expression.Type) && !TypeUtils.IsUnsignedInt(expression.Type))
        return new UnaryExpression(ExpressionType.Negate, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Negate, "op_UnaryNegation", expression);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a unary plus operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.UnaryPlus"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property set to the specified value.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.InvalidOperationException">The unary plus operator is not defined for <paramref name="expression"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression UnaryPlus(Expression expression)
    {
      return Expression.UnaryPlus(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a unary plus operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.UnaryPlus"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> and <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly one argument.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the unary plus operator is not defined for <paramref name="expression"/>.Type.-or-<paramref name="expression"/>.Type (or its corresponding non-nullable type if it is a nullable value type) is not assignable to the argument type of the method represented by <paramref name="method"/>.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression UnaryPlus(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.UnaryPlus, expression, method);
      if (TypeUtils.IsArithmetic(expression.Type))
        return new UnaryExpression(ExpressionType.UnaryPlus, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.UnaryPlus, "op_UnaryPlus", expression);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents an arithmetic negation operation that has overflow checking.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NegateChecked"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property set to the specified value.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.InvalidOperationException">The unary minus operator is not defined for <paramref name="expression"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression NegateChecked(Expression expression)
    {
      return Expression.NegateChecked(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents an arithmetic negation operation that has overflow checking. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.NegateChecked"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> and <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly one argument.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the unary minus operator is not defined for <paramref name="expression"/>.Type.-or-<paramref name="expression"/>.Type (or its corresponding non-nullable type if it is a nullable value type) is not assignable to the argument type of the method represented by <paramref name="method"/>.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression NegateChecked(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.NegateChecked, expression, method);
      if (TypeUtils.IsArithmetic(expression.Type) && !TypeUtils.IsUnsignedInt(expression.Type))
        return new UnaryExpression(ExpressionType.NegateChecked, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.NegateChecked, "op_UnaryNegation", expression);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a bitwise complement operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Not"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property set to the specified value.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.InvalidOperationException">The unary not operator is not defined for <paramref name="expression"/>.Type.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression Not(Expression expression)
    {
      return Expression.Not(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a bitwise complement operation. The implementing method can be specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Not"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> and <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly one argument.</exception><exception cref="T:System.InvalidOperationException"><paramref name="method"/> is null and the unary not operator is not defined for <paramref name="expression"/>.Type.-or-<paramref name="expression"/>.Type (or its corresponding non-nullable type if it is a nullable value type) is not assignable to the argument type of the method represented by <paramref name="method"/>.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression Not(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.Not, expression, method);
      if (TypeUtils.IsIntegerOrBool(expression.Type))
        return new UnaryExpression(ExpressionType.Not, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperator(ExpressionType.Not, "op_LogicalNot", expression) ?? Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Not, "op_OnesComplement", expression);
    }

    /// <summary>
    /// Returns whether the expression evaluates to false.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to evaluate.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression IsFalse(Expression expression)
    {
      return Expression.IsFalse(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Returns whether the expression evaluates to false.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to evaluate.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    public static UnaryExpression IsFalse(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.IsFalse, expression, method);
      if (TypeUtils.IsBool(expression.Type))
        return new UnaryExpression(ExpressionType.IsFalse, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.IsFalse, "op_False", expression);
    }

    /// <summary>
    /// Returns whether the expression evaluates to true.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to evaluate.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression IsTrue(Expression expression)
    {
      return Expression.IsTrue(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Returns whether the expression evaluates to true.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to evaluate.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    public static UnaryExpression IsTrue(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.IsTrue, expression, method);
      if (TypeUtils.IsBool(expression.Type))
        return new UnaryExpression(ExpressionType.IsTrue, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.IsTrue, "op_True", expression);
    }

    /// <summary>
    /// Returns the expression representing the ones complement.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/>.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression OnesComplement(Expression expression)
    {
      return Expression.OnesComplement(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Returns the expression representing the ones complement.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/>.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    public static UnaryExpression OnesComplement(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.OnesComplement, expression, method);
      if (TypeUtils.IsInteger(expression.Type))
        return new UnaryExpression(ExpressionType.OnesComplement, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.OnesComplement, "op_OnesComplement", expression);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents an explicit reference or boxing conversion where null is supplied if the conversion fails.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.TypeAs"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> and <see cref="P:System.Linq.Expressions.Expression.Type"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="type">A <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="type"/> is null.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression TypeAs(Expression expression, Type type)
    {
      Expression.RequiresCanRead(expression, "expression");
      ContractUtils.RequiresNotNull((object) type, "type");
      TypeUtils.ValidateType(type);
      if (type.IsValueType && !TypeUtils.IsNullableType(type))
        throw System.Linq.Expressions.Error.IncorrectTypeForTypeAs((object) type);
      else
        return new UnaryExpression(ExpressionType.TypeAs, expression, type, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents an explicit unboxing.
    /// </summary>
    /// 
    /// <returns>
    /// An instance of <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to unbox.</param><param name="type">The new <see cref="T:System.Type"/> of the expression.</param>
    [__DynamicallyInvokable]
    public static UnaryExpression Unbox(Expression expression, Type type)
    {
      Expression.RequiresCanRead(expression, "expression");
      ContractUtils.RequiresNotNull((object) type, "type");
      if (!expression.Type.IsInterface && expression.Type != typeof (object))
        throw System.Linq.Expressions.Error.InvalidUnboxType();
      if (!type.IsValueType)
        throw System.Linq.Expressions.Error.InvalidUnboxType();
      TypeUtils.ValidateType(type);
      return new UnaryExpression(ExpressionType.Unbox, expression, type, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a type conversion operation.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Convert"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> and <see cref="P:System.Linq.Expressions.Expression.Type"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="type">A <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="type"/> is null.</exception><exception cref="T:System.InvalidOperationException">No conversion operator is defined between <paramref name="expression"/>.Type and <paramref name="type"/>.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression Convert(Expression expression, Type type)
    {
      return Expression.Convert(expression, type, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a conversion operation for which the implementing method is specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Convert"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/>, <see cref="P:System.Linq.Expressions.Expression.Type"/>, and <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="type">A <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="type"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly one argument.</exception><exception cref="T:System.InvalidOperationException">No conversion operator is defined between <paramref name="expression"/>.Type and <paramref name="type"/>.-or-<paramref name="expression"/>.Type is not assignable to the argument type of the method represented by <paramref name="method"/>.-or-The return type of the method represented by <paramref name="method"/> is not assignable to <paramref name="type"/>.-or-<paramref name="expression"/>.Type or <paramref name="type"/> is a nullable value type and the corresponding non-nullable value type does not equal the argument type or the return type, respectively, of the method represented by <paramref name="method"/>.</exception><exception cref="T:System.Reflection.AmbiguousMatchException">More than one method that matches the <paramref name="method"/> description was found.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression Convert(Expression expression, Type type, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      ContractUtils.RequiresNotNull((object) type, "type");
      TypeUtils.ValidateType(type);
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedCoercionOperator(ExpressionType.Convert, expression, type, method);
      if (TypeUtils.HasIdentityPrimitiveOrNullableConversion(expression.Type, type) || TypeUtils.HasReferenceConversion(expression.Type, type))
        return new UnaryExpression(ExpressionType.Convert, expression, type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedCoercionOrThrow(ExpressionType.Convert, expression, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a conversion operation that throws an exception if the target type is overflowed.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ConvertChecked"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> and <see cref="P:System.Linq.Expressions.Expression.Type"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="type">A <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="type"/> is null.</exception><exception cref="T:System.InvalidOperationException">No conversion operator is defined between <paramref name="expression"/>.Type and <paramref name="type"/>.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression ConvertChecked(Expression expression, Type type)
    {
      return Expression.ConvertChecked(expression, type, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a conversion operation that throws an exception if the target type is overflowed and for which the implementing method is specified.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ConvertChecked"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/>, <see cref="P:System.Linq.Expressions.Expression.Type"/>, and <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> properties set to the specified values.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><param name="type">A <see cref="T:System.Type"/> to set the <see cref="P:System.Linq.Expressions.Expression.Type"/> property equal to.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Method"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> or <paramref name="type"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="method"/> is not null and the method it represents returns void, is not static (Shared in Visual Basic), or does not take exactly one argument.</exception><exception cref="T:System.InvalidOperationException">No conversion operator is defined between <paramref name="expression"/>.Type and <paramref name="type"/>.-or-<paramref name="expression"/>.Type is not assignable to the argument type of the method represented by <paramref name="method"/>.-or-The return type of the method represented by <paramref name="method"/> is not assignable to <paramref name="type"/>.-or-<paramref name="expression"/>.Type or <paramref name="type"/> is a nullable value type and the corresponding non-nullable value type does not equal the argument type or the return type, respectively, of the method represented by <paramref name="method"/>.</exception><exception cref="T:System.Reflection.AmbiguousMatchException">More than one method that matches the <paramref name="method"/> description was found.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression ConvertChecked(Expression expression, Type type, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      ContractUtils.RequiresNotNull((object) type, "type");
      TypeUtils.ValidateType(type);
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedCoercionOperator(ExpressionType.ConvertChecked, expression, type, method);
      if (TypeUtils.HasIdentityPrimitiveOrNullableConversion(expression.Type, type))
        return new UnaryExpression(ExpressionType.ConvertChecked, expression, type, (MethodInfo) null);
      if (TypeUtils.HasReferenceConversion(expression.Type, type))
        return new UnaryExpression(ExpressionType.Convert, expression, type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedCoercionOrThrow(ExpressionType.ConvertChecked, expression, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents an expression for obtaining the length of a one-dimensional array.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.ArrayLength"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to <paramref name="array"/>.
    /// </returns>
    /// <param name="array">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/>.Type does not represent an array type.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression ArrayLength(Expression array)
    {
      ContractUtils.RequiresNotNull((object) array, "array");
      if (!array.Type.IsArray || !typeof (Array).IsAssignableFrom(array.Type))
        throw System.Linq.Expressions.Error.ArgumentMustBeArray();
      if (array.Type.GetArrayRank() != 1)
        throw System.Linq.Expressions.Error.ArgumentMustBeSingleDimensionalArrayType();
      else
        return new UnaryExpression(ExpressionType.ArrayLength, array, typeof (int), (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents an expression that has a constant value of type <see cref="T:System.Linq.Expressions.Expression"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that has the <see cref="P:System.Linq.Expressions.Expression.NodeType"/> property equal to <see cref="F:System.Linq.Expressions.ExpressionType.Quote"/> and the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property set to the specified value.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to set the <see cref="P:System.Linq.Expressions.UnaryExpression.Operand"/> property equal to.</param><exception cref="T:System.ArgumentNullException"><paramref name="expression"/> is null.</exception>
    [__DynamicallyInvokable]
    public static UnaryExpression Quote(Expression expression)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(expression is LambdaExpression))
        throw System.Linq.Expressions.Error.QuotedExpressionMustBeLambda();
      else
        return new UnaryExpression(ExpressionType.Quote, expression, expression.GetType(), (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a rethrowing of an exception.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a rethrowing of an exception.
    /// </returns>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression Rethrow()
    {
      return Expression.Throw((Expression) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a rethrowing of an exception with a given type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a rethrowing of an exception.
    /// </returns>
    /// <param name="type">The new <see cref="T:System.Type"/> of the expression.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression Rethrow(Type type)
    {
      return Expression.Throw((Expression) null, type);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a throwing of an exception.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the exception.
    /// </returns>
    /// <param name="value">An <see cref="T:System.Linq.Expressions.Expression"/>.</param>
    [__DynamicallyInvokable]
    public static UnaryExpression Throw(Expression value)
    {
      return Expression.Throw(value, typeof (void));
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents a throwing of an exception with a given type.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the exception.
    /// </returns>
    /// <param name="value">An <see cref="T:System.Linq.Expressions.Expression"/>.</param><param name="type">The new <see cref="T:System.Type"/> of the expression.</param>
    [__DynamicallyInvokable]
    public static UnaryExpression Throw(Expression value, Type type)
    {
      ContractUtils.RequiresNotNull((object) type, "type");
      TypeUtils.ValidateType(type);
      if (value != null)
      {
        Expression.RequiresCanRead(value, "value");
        if (value.Type.IsValueType)
          throw System.Linq.Expressions.Error.ArgumentMustNotHaveValueType();
      }
      return new UnaryExpression(ExpressionType.Throw, value, type, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the incrementing of the expression value by 1.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the incremented expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to increment.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression Increment(Expression expression)
    {
      return Expression.Increment(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the incrementing of the expression by 1.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the incremented expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to increment.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    public static UnaryExpression Increment(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.Increment, expression, method);
      if (TypeUtils.IsArithmetic(expression.Type))
        return new UnaryExpression(ExpressionType.Increment, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Increment, "op_Increment", expression);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the decrementing of the expression by 1.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the decremented expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to decrement.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression Decrement(Expression expression)
    {
      return Expression.Decrement(expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the decrementing of the expression by 1.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the decremented expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to decrement.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    public static UnaryExpression Decrement(Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      if (!(method == (MethodInfo) null))
        return Expression.GetMethodBasedUnaryOperator(ExpressionType.Decrement, expression, method);
      if (TypeUtils.IsArithmetic(expression.Type))
        return new UnaryExpression(ExpressionType.Decrement, expression, expression.Type, (MethodInfo) null);
      else
        return Expression.GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Decrement, "op_Decrement", expression);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that increments the expression by 1 and assigns the result back to the expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the resultant expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to apply the operations on.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression PreIncrementAssign(Expression expression)
    {
      return Expression.MakeOpAssignUnary(ExpressionType.PreIncrementAssign, expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that increments the expression by 1 and assigns the result back to the expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the resultant expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to apply the operations on.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression PreIncrementAssign(Expression expression, MethodInfo method)
    {
      return Expression.MakeOpAssignUnary(ExpressionType.PreIncrementAssign, expression, method);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that decrements the expression by 1 and assigns the result back to the expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the resultant expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to apply the operations on.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression PreDecrementAssign(Expression expression)
    {
      return Expression.MakeOpAssignUnary(ExpressionType.PreDecrementAssign, expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that decrements the expression by 1 and assigns the result back to the expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the resultant expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to apply the operations on.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression PreDecrementAssign(Expression expression, MethodInfo method)
    {
      return Expression.MakeOpAssignUnary(ExpressionType.PreDecrementAssign, expression, method);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the assignment of the expression followed by a subsequent increment by 1 of the original expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the resultant expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to apply the operations on.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression PostIncrementAssign(Expression expression)
    {
      return Expression.MakeOpAssignUnary(ExpressionType.PostIncrementAssign, expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the assignment of the expression followed by a subsequent increment by 1 of the original expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the resultant expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to apply the operations on.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression PostIncrementAssign(Expression expression, MethodInfo method)
    {
      return Expression.MakeOpAssignUnary(ExpressionType.PostIncrementAssign, expression, method);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the assignment of the expression followed by a subsequent decrement by 1 of the original expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the resultant expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to apply the operations on.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression PostDecrementAssign(Expression expression)
    {
      return Expression.MakeOpAssignUnary(ExpressionType.PostDecrementAssign, expression, (MethodInfo) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the assignment of the expression followed by a subsequent decrement by 1 of the original expression.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Expressions.UnaryExpression"/> that represents the resultant expression.
    /// </returns>
    /// <param name="expression">An <see cref="T:System.Linq.Expressions.Expression"/> to apply the operations on.</param><param name="method">A <see cref="T:System.Reflection.MethodInfo"/> that represents the implementing method.</param>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static UnaryExpression PostDecrementAssign(Expression expression, MethodInfo method)
    {
      return Expression.MakeOpAssignUnary(ExpressionType.PostDecrementAssign, expression, method);
    }

    private static UnaryExpression MakeOpAssignUnary(ExpressionType kind, Expression expression, MethodInfo method)
    {
      Expression.RequiresCanRead(expression, "expression");
      Expression.RequiresCanWrite(expression, "expression");
      UnaryExpression unaryExpression;
      if (method == (MethodInfo) null)
      {
        if (TypeUtils.IsArithmetic(expression.Type))
          return new UnaryExpression(kind, expression, expression.Type, (MethodInfo) null);
        string name = kind == ExpressionType.PreIncrementAssign || kind == ExpressionType.PostIncrementAssign ? "op_Increment" : "op_Decrement";
        unaryExpression = Expression.GetUserDefinedUnaryOperatorOrThrow(kind, name, expression);
      }
      else
        unaryExpression = Expression.GetMethodBasedUnaryOperator(kind, expression, method);
      if (!TypeUtils.AreReferenceAssignable(expression.Type, unaryExpression.Type))
        throw System.Linq.Expressions.Error.UserDefinedOpMustHaveValidReturnType((object) kind, (object) method.Name);
      else
        return unaryExpression;
    }

    private static BinaryExpression GetUserDefinedBinaryOperator(ExpressionType binaryType, string name, Expression left, Expression right, bool liftToNull)
    {
      MethodInfo definedBinaryOperator1 = Expression.GetUserDefinedBinaryOperator(binaryType, left.Type, right.Type, name);
      if (definedBinaryOperator1 != (MethodInfo) null)
        return (BinaryExpression) new MethodBinaryExpression(binaryType, left, right, definedBinaryOperator1.ReturnType, definedBinaryOperator1);
      if (TypeUtils.IsNullableType(left.Type) && TypeUtils.IsNullableType(right.Type))
      {
        Type nonNullableType1 = TypeUtils.GetNonNullableType(left.Type);
        Type nonNullableType2 = TypeUtils.GetNonNullableType(right.Type);
        MethodInfo definedBinaryOperator2 = Expression.GetUserDefinedBinaryOperator(binaryType, nonNullableType1, nonNullableType2, name);
        if (definedBinaryOperator2 != (MethodInfo) null && definedBinaryOperator2.ReturnType.IsValueType && !TypeUtils.IsNullableType(definedBinaryOperator2.ReturnType))
        {
          if (definedBinaryOperator2.ReturnType != typeof (bool) || liftToNull)
            return (BinaryExpression) new MethodBinaryExpression(binaryType, left, right, TypeUtils.GetNullableType(definedBinaryOperator2.ReturnType), definedBinaryOperator2);
          else
            return (BinaryExpression) new MethodBinaryExpression(binaryType, left, right, typeof (bool), definedBinaryOperator2);
        }
      }
      return (BinaryExpression) null;
    }

    private static BinaryExpression GetMethodBasedBinaryOperator(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, bool liftToNull)
    {
      Expression.ValidateOperator(method);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) method);
      if (parametersCached.Length != 2)
        throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) method);
      if (Expression.ParameterIsAssignable(parametersCached[0], left.Type) && Expression.ParameterIsAssignable(parametersCached[1], right.Type))
      {
        Expression.ValidateParamswithOperandsOrThrow(parametersCached[0].ParameterType, left.Type, binaryType, method.Name);
        Expression.ValidateParamswithOperandsOrThrow(parametersCached[1].ParameterType, right.Type, binaryType, method.Name);
        return (BinaryExpression) new MethodBinaryExpression(binaryType, left, right, method.ReturnType, method);
      }
      else
      {
        if (!TypeUtils.IsNullableType(left.Type) || !TypeUtils.IsNullableType(right.Type) || (!Expression.ParameterIsAssignable(parametersCached[0], TypeUtils.GetNonNullableType(left.Type)) || !Expression.ParameterIsAssignable(parametersCached[1], TypeUtils.GetNonNullableType(right.Type))) || (!method.ReturnType.IsValueType || TypeUtils.IsNullableType(method.ReturnType)))
          throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) binaryType, (object) method.Name);
        if (method.ReturnType != typeof (bool) || liftToNull)
          return (BinaryExpression) new MethodBinaryExpression(binaryType, left, right, TypeUtils.GetNullableType(method.ReturnType), method);
        else
          return (BinaryExpression) new MethodBinaryExpression(binaryType, left, right, typeof (bool), method);
      }
    }

    private static BinaryExpression GetMethodBasedAssignOperator(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression conversion, bool liftToNull)
    {
      BinaryExpression binaryExpression = Expression.GetMethodBasedBinaryOperator(binaryType, left, right, method, liftToNull);
      if (conversion == null)
      {
        if (!TypeUtils.AreReferenceAssignable(left.Type, binaryExpression.Type))
          throw System.Linq.Expressions.Error.UserDefinedOpMustHaveValidReturnType((object) binaryType, (object) binaryExpression.Method.Name);
      }
      else
      {
        Expression.ValidateOpAssignConversionLambda(conversion, binaryExpression.Left, binaryExpression.Method, binaryExpression.NodeType);
        binaryExpression = (BinaryExpression) new OpAssignMethodConversionBinaryExpression(binaryExpression.NodeType, binaryExpression.Left, binaryExpression.Right, binaryExpression.Left.Type, binaryExpression.Method, conversion);
      }
      return binaryExpression;
    }

    private static BinaryExpression GetUserDefinedBinaryOperatorOrThrow(ExpressionType binaryType, string name, Expression left, Expression right, bool liftToNull)
    {
      BinaryExpression definedBinaryOperator = Expression.GetUserDefinedBinaryOperator(binaryType, name, left, right, liftToNull);
      if (definedBinaryOperator == null)
        throw System.Linq.Expressions.Error.BinaryOperatorNotDefined((object) binaryType, (object) left.Type, (object) right.Type);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) definedBinaryOperator.Method);
      Expression.ValidateParamswithOperandsOrThrow(parametersCached[0].ParameterType, left.Type, binaryType, name);
      Expression.ValidateParamswithOperandsOrThrow(parametersCached[1].ParameterType, right.Type, binaryType, name);
      return definedBinaryOperator;
    }

    private static BinaryExpression GetUserDefinedAssignOperatorOrThrow(ExpressionType binaryType, string name, Expression left, Expression right, LambdaExpression conversion, bool liftToNull)
    {
      BinaryExpression binaryExpression = Expression.GetUserDefinedBinaryOperatorOrThrow(binaryType, name, left, right, liftToNull);
      if (conversion == null)
      {
        if (!TypeUtils.AreReferenceAssignable(left.Type, binaryExpression.Type))
          throw System.Linq.Expressions.Error.UserDefinedOpMustHaveValidReturnType((object) binaryType, (object) binaryExpression.Method.Name);
      }
      else
      {
        Expression.ValidateOpAssignConversionLambda(conversion, binaryExpression.Left, binaryExpression.Method, binaryExpression.NodeType);
        binaryExpression = (BinaryExpression) new OpAssignMethodConversionBinaryExpression(binaryExpression.NodeType, binaryExpression.Left, binaryExpression.Right, binaryExpression.Left.Type, binaryExpression.Method, conversion);
      }
      return binaryExpression;
    }

    private static MethodInfo GetUserDefinedBinaryOperator(ExpressionType binaryType, Type leftType, Type rightType, string name)
    {
      Type[] types = new Type[2]
      {
        leftType,
        rightType
      };
      Type nonNullableType1 = TypeUtils.GetNonNullableType(leftType);
      Type nonNullableType2 = TypeUtils.GetNonNullableType(rightType);
      BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      MethodInfo method = TypeExtensions.GetMethodValidated(nonNullableType1, name, bindingAttr, (Binder) null, types, (ParameterModifier[]) null);
      if (method == (MethodInfo) null && !TypeUtils.AreEquivalent(leftType, rightType))
        method = TypeExtensions.GetMethodValidated(nonNullableType2, name, bindingAttr, (Binder) null, types, (ParameterModifier[]) null);
      if (Expression.IsLiftingConditionalLogicalOperator(leftType, rightType, method, binaryType))
        method = Expression.GetUserDefinedBinaryOperator(binaryType, nonNullableType1, nonNullableType2, name);
      return method;
    }

    private static bool IsLiftingConditionalLogicalOperator(Type left, Type right, MethodInfo method, ExpressionType binaryType)
    {
      if (!TypeUtils.IsNullableType(right) || !TypeUtils.IsNullableType(left) || !(method == (MethodInfo) null))
        return false;
      if (binaryType != ExpressionType.AndAlso)
        return binaryType == ExpressionType.OrElse;
      else
        return true;
    }

    internal static bool ParameterIsAssignable(ParameterInfo pi, Type argType)
    {
      Type dest = pi.ParameterType;
      if (dest.IsByRef)
        dest = dest.GetElementType();
      return TypeUtils.AreReferenceAssignable(dest, argType);
    }

    private static void ValidateParamswithOperandsOrThrow(Type paramType, Type operandType, ExpressionType exprType, string name)
    {
      if (TypeUtils.IsNullableType(paramType) && !TypeUtils.IsNullableType(operandType))
        throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) exprType, (object) name);
    }

    private static void ValidateOperator(MethodInfo method)
    {
      Expression.ValidateMethodInfo(method);
      if (!method.IsStatic)
        throw System.Linq.Expressions.Error.UserDefinedOperatorMustBeStatic((object) method);
      if (method.ReturnType == typeof (void))
        throw System.Linq.Expressions.Error.UserDefinedOperatorMustNotBeVoid((object) method);
    }

    private static void ValidateMethodInfo(MethodInfo method)
    {
      if (method.IsGenericMethodDefinition)
        throw System.Linq.Expressions.Error.MethodIsGeneric((object) method);
      if (method.ContainsGenericParameters)
        throw System.Linq.Expressions.Error.MethodContainsGenericParameters((object) method);
    }

    private static bool IsNullComparison(Expression left, Expression right)
    {
      return Expression.IsNullConstant(left) && !Expression.IsNullConstant(right) && TypeUtils.IsNullableType(right.Type) || Expression.IsNullConstant(right) && !Expression.IsNullConstant(left) && TypeUtils.IsNullableType(left.Type);
    }

    private static bool IsNullConstant(Expression e)
    {
      ConstantExpression constantExpression = e as ConstantExpression;
      if (constantExpression != null)
        return constantExpression.Value == null;
      else
        return false;
    }

    private static void ValidateUserDefinedConditionalLogicOperator(ExpressionType nodeType, Type left, Type right, MethodInfo method)
    {
      Expression.ValidateOperator(method);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) method);
      if (parametersCached.Length != 2)
        throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) method);
      if (!Expression.ParameterIsAssignable(parametersCached[0], left) && (!TypeUtils.IsNullableType(left) || !Expression.ParameterIsAssignable(parametersCached[0], TypeUtils.GetNonNullableType(left))))
        throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) nodeType, (object) method.Name);
      if (!Expression.ParameterIsAssignable(parametersCached[1], right) && (!TypeUtils.IsNullableType(right) || !Expression.ParameterIsAssignable(parametersCached[1], TypeUtils.GetNonNullableType(right))))
        throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) nodeType, (object) method.Name);
      if (parametersCached[0].ParameterType != parametersCached[1].ParameterType)
        throw System.Linq.Expressions.Error.UserDefinedOpMustHaveConsistentTypes((object) nodeType, (object) method.Name);
      if (method.ReturnType != parametersCached[0].ParameterType)
        throw System.Linq.Expressions.Error.UserDefinedOpMustHaveConsistentTypes((object) nodeType, (object) method.Name);
      if (Expression.IsValidLiftedConditionalLogicalOperator(left, right, parametersCached))
      {
        left = TypeUtils.GetNonNullableType(left);
        right = TypeUtils.GetNonNullableType(left);
      }
      MethodInfo booleanOperator1 = TypeUtils.GetBooleanOperator(method.DeclaringType, "op_True");
      MethodInfo booleanOperator2 = TypeUtils.GetBooleanOperator(method.DeclaringType, "op_False");
      if (booleanOperator1 == (MethodInfo) null || booleanOperator1.ReturnType != typeof (bool) || (booleanOperator2 == (MethodInfo) null || booleanOperator2.ReturnType != typeof (bool)))
        throw System.Linq.Expressions.Error.LogicalOperatorMustHaveBooleanOperators((object) nodeType, (object) method.Name);
      Expression.VerifyOpTrueFalse(nodeType, left, booleanOperator2);
      Expression.VerifyOpTrueFalse(nodeType, left, booleanOperator1);
    }

    private static void VerifyOpTrueFalse(ExpressionType nodeType, Type left, MethodInfo opTrue)
    {
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) opTrue);
      if (parametersCached.Length != 1)
        throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) opTrue);
      if (!Expression.ParameterIsAssignable(parametersCached[0], left) && (!TypeUtils.IsNullableType(left) || !Expression.ParameterIsAssignable(parametersCached[0], TypeUtils.GetNonNullableType(left))))
        throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) nodeType, (object) opTrue.Name);
    }

    private static bool IsValidLiftedConditionalLogicalOperator(Type left, Type right, ParameterInfo[] pms)
    {
      if (TypeUtils.AreEquivalent(left, right) && TypeUtils.IsNullableType(right))
        return TypeUtils.AreEquivalent(pms[1].ParameterType, TypeUtils.GetNonNullableType(right));
      else
        return false;
    }

    private static BinaryExpression GetEqualityComparisonOperator(ExpressionType binaryType, string opName, Expression left, Expression right, bool liftToNull)
    {
      if (left.Type == right.Type && (TypeUtils.IsNumeric(left.Type) || left.Type == typeof (object) || (TypeUtils.IsBool(left.Type) || TypeUtils.GetNonNullableType(left.Type).IsEnum)))
      {
        if (TypeUtils.IsNullableType(left.Type) && liftToNull)
          return (BinaryExpression) new SimpleBinaryExpression(binaryType, left, right, typeof (bool?));
        else
          return (BinaryExpression) new LogicalBinaryExpression(binaryType, left, right);
      }
      else
      {
        BinaryExpression definedBinaryOperator = Expression.GetUserDefinedBinaryOperator(binaryType, opName, left, right, liftToNull);
        if (definedBinaryOperator != null)
          return definedBinaryOperator;
        if (!TypeUtils.HasBuiltInEqualityOperator(left.Type, right.Type) && !Expression.IsNullComparison(left, right))
          throw System.Linq.Expressions.Error.BinaryOperatorNotDefined((object) binaryType, (object) left.Type, (object) right.Type);
        if (TypeUtils.IsNullableType(left.Type) && liftToNull)
          return (BinaryExpression) new SimpleBinaryExpression(binaryType, left, right, typeof (bool?));
        else
          return (BinaryExpression) new LogicalBinaryExpression(binaryType, left, right);
      }
    }

    private static BinaryExpression GetComparisonOperator(ExpressionType binaryType, string opName, Expression left, Expression right, bool liftToNull)
    {
      if (!(left.Type == right.Type) || !TypeUtils.IsNumeric(left.Type))
        return Expression.GetUserDefinedBinaryOperatorOrThrow(binaryType, opName, left, right, liftToNull);
      if (TypeUtils.IsNullableType(left.Type) && liftToNull)
        return (BinaryExpression) new SimpleBinaryExpression(binaryType, left, right, typeof (bool?));
      else
        return (BinaryExpression) new LogicalBinaryExpression(binaryType, left, right);
    }

    private static Type ValidateCoalesceArgTypes(Type left, Type right)
    {
      Type nonNullableType = TypeUtils.GetNonNullableType(left);
      if (left.IsValueType && !TypeUtils.IsNullableType(left))
        throw System.Linq.Expressions.Error.CoalesceUsedOnNonNullType();
      if (TypeUtils.IsNullableType(left) && TypeUtils.IsImplicitlyConvertible(right, nonNullableType))
        return nonNullableType;
      if (TypeUtils.IsImplicitlyConvertible(right, left))
        return left;
      if (TypeUtils.IsImplicitlyConvertible(nonNullableType, right))
        return right;
      else
        throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
    }

    private static void ValidateOpAssignConversionLambda(LambdaExpression conversion, Expression left, MethodInfo method, ExpressionType nodeType)
    {
      MethodInfo method1 = conversion.Type.GetMethod("Invoke");
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) method1);
      if (parametersCached.Length != 1)
        throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) conversion);
      if (!TypeUtils.AreEquivalent(method1.ReturnType, left.Type))
        throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) nodeType, (object) conversion.ToString());
      if (method != (MethodInfo) null && !TypeUtils.AreEquivalent(parametersCached[0].ParameterType, method.ReturnType))
        throw System.Linq.Expressions.Error.OverloadOperatorTypeDoesNotMatchConversionType((object) nodeType, (object) conversion.ToString());
    }

    private static bool IsSimpleShift(Type left, Type right)
    {
      if (TypeUtils.IsInteger(left))
        return TypeUtils.GetNonNullableType(right) == typeof (int);
      else
        return false;
    }

    private static Type GetResultTypeOfShift(Type left, Type right)
    {
      if (TypeUtils.IsNullableType(left) || !TypeUtils.IsNullableType(right))
        return left;
      return typeof (Nullable<>).MakeGenericType(new Type[1]
      {
        left
      });
    }

    internal static void ValidateVariables(ReadOnlyCollection<ParameterExpression> varList, string collectionName)
    {
      if (varList.Count == 0)
        return;
      int count = varList.Count;
      Set<ParameterExpression> set = new Set<ParameterExpression>(count);
      for (int index = 0; index < count; ++index)
      {
        ParameterExpression parameterExpression = varList[index];
        if (parameterExpression == null)
        {
          throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}[{1}]", new object[2]
          {
            (object) collectionName,
            (object) set.Count
          }));
        }
        else
        {
          if (parameterExpression.IsByRef)
            throw System.Linq.Expressions.Error.VariableMustNotBeByRef((object) parameterExpression, (object) parameterExpression.Type);
          if (set.Contains(parameterExpression))
            throw System.Linq.Expressions.Error.DuplicateVariable((object) parameterExpression);
          set.Add(parameterExpression);
        }
      }
    }

    private static void ValidateSpan(int startLine, int startColumn, int endLine, int endColumn)
    {
      if (startLine < 1)
        throw System.Linq.Expressions.Error.OutOfRange((object) "startLine", (object) 1);
      if (startColumn < 1)
        throw System.Linq.Expressions.Error.OutOfRange((object) "startColumn", (object) 1);
      if (endLine < 1)
        throw System.Linq.Expressions.Error.OutOfRange((object) "endLine", (object) 1);
      if (endColumn < 1)
        throw System.Linq.Expressions.Error.OutOfRange((object) "endColumn", (object) 1);
      if (startLine > endLine)
        throw System.Linq.Expressions.Error.StartEndMustBeOrdered();
      if (startLine == endLine && startColumn > endColumn)
        throw System.Linq.Expressions.Error.StartEndMustBeOrdered();
    }

    private static MethodInfo GetValidMethodForDynamic(Type delegateType)
    {
      MethodInfo method = delegateType.GetMethod("Invoke");
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) method);
      if (parametersCached.Length == 0 || parametersCached[0].ParameterType != typeof (CallSite))
        throw System.Linq.Expressions.Error.FirstArgumentMustBeCallSite();
      else
        return method;
    }

    private static DynamicExpression MakeDynamic(CallSiteBinder binder, Type returnType, ReadOnlyCollection<Expression> args)
    {
      ContractUtils.RequiresNotNull((object) binder, "binder");
      for (int index = 0; index < args.Count; ++index)
        Expression.ValidateDynamicArgument(args[index]);
      Type delegateType = DelegateHelpers.MakeCallSiteDelegate(args, returnType);
      switch (args.Count)
      {
        case 1:
          return DynamicExpression.Make(returnType, delegateType, binder, args[0]);
        case 2:
          return DynamicExpression.Make(returnType, delegateType, binder, args[0], args[1]);
        case 3:
          return DynamicExpression.Make(returnType, delegateType, binder, args[0], args[1], args[2]);
        case 4:
          return DynamicExpression.Make(returnType, delegateType, binder, args[0], args[1], args[2], args[3]);
        default:
          return DynamicExpression.Make(returnType, delegateType, binder, args);
      }
    }

    private static void ValidateDynamicArgument(Expression arg)
    {
      Expression.RequiresCanRead(arg, "arguments");
      Type type = arg.Type;
      ContractUtils.RequiresNotNull((object) type, "type");
      TypeUtils.ValidateType(type);
      if (type == typeof (void))
        throw System.Linq.Expressions.Error.ArgumentTypeCannotBeVoid();
    }

    private static void ValidateElementInitAddMethodInfo(MethodInfo addMethod)
    {
      Expression.ValidateMethodInfo(addMethod);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) addMethod);
      if (parametersCached.Length == 0)
        throw System.Linq.Expressions.Error.ElementInitializerMethodWithZeroArgs();
      if (!addMethod.Name.Equals("Add", StringComparison.OrdinalIgnoreCase))
        throw System.Linq.Expressions.Error.ElementInitializerMethodNotAdd();
      if (addMethod.IsStatic)
        throw System.Linq.Expressions.Error.ElementInitializerMethodStatic();
      foreach (ParameterInfo parameterInfo in parametersCached)
      {
        if (parameterInfo.ParameterType.IsByRef)
          throw System.Linq.Expressions.Error.ElementInitializerMethodNoRefOutParam((object) parameterInfo.Name, (object) addMethod.Name);
      }
    }

    internal static ReadOnlyCollection<Expression> ReturnReadOnly(IArgumentProvider provider, ref object collection)
    {
      Expression expression = collection as Expression;
      if (expression != null)
        Interlocked.CompareExchange(ref collection, (object) new ReadOnlyCollection<Expression>((IList<Expression>) new ListArgumentProvider(provider, expression)), (object) expression);
      return (ReadOnlyCollection<Expression>) collection;
    }

    private static void RequiresCanRead(Expression expression, string paramName)
    {
      if (expression == null)
        throw new ArgumentNullException(paramName);
      switch (expression.NodeType)
      {
        case ExpressionType.MemberAccess:
          MemberInfo member = ((MemberExpression) expression).Member;
          if (member.MemberType != MemberTypes.Property || ((PropertyInfo) member).CanRead)
            break;
          else
            throw new ArgumentException(System.Linq.Expressions.Strings.ExpressionMustBeReadable, paramName);
        case ExpressionType.Index:
          IndexExpression indexExpression = (IndexExpression) expression;
          if (!(indexExpression.Indexer != (PropertyInfo) null) || indexExpression.Indexer.CanRead)
            break;
          else
            throw new ArgumentException(System.Linq.Expressions.Strings.ExpressionMustBeReadable, paramName);
      }
    }

    private static void RequiresCanRead(IEnumerable<Expression> items, string paramName)
    {
      if (items == null)
        return;
      IList<Expression> list = items as IList<Expression>;
      if (list != null)
      {
        for (int index = 0; index < list.Count; ++index)
          Expression.RequiresCanRead(list[index], paramName);
      }
      else
      {
        foreach (Expression expression in items)
          Expression.RequiresCanRead(expression, paramName);
      }
    }

    private static void RequiresCanWrite(Expression expression, string paramName)
    {
      if (expression == null)
        throw new ArgumentNullException(paramName);
      bool flag = false;
      switch (expression.NodeType)
      {
        case ExpressionType.MemberAccess:
          MemberExpression memberExpression = (MemberExpression) expression;
          switch (memberExpression.Member.MemberType)
          {
            case MemberTypes.Field:
              FieldInfo fieldInfo = (FieldInfo) memberExpression.Member;
              flag = !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral;
              break;
            case MemberTypes.Property:
              flag = ((PropertyInfo) memberExpression.Member).CanWrite;
              break;
          }
        case ExpressionType.Parameter:
          flag = true;
          break;
        case ExpressionType.Index:
          IndexExpression indexExpression = (IndexExpression) expression;
          flag = !(indexExpression.Indexer != (PropertyInfo) null) || indexExpression.Indexer.CanWrite;
          break;
      }
      if (!flag)
        throw new ArgumentException(System.Linq.Expressions.Strings.ExpressionMustBeWriteable, paramName);
    }

    private static void ValidateGoto(LabelTarget target, ref Expression value, string targetParameter, string valueParameter)
    {
      ContractUtils.RequiresNotNull((object) target, targetParameter);
      if (value == null)
      {
        if (target.Type != typeof (void))
          throw System.Linq.Expressions.Error.LabelMustBeVoidOrHaveExpression();
      }
      else
        Expression.ValidateGotoType(target.Type, ref value, valueParameter);
    }

    private static void ValidateGotoType(Type expectedType, ref Expression value, string paramName)
    {
      Expression.RequiresCanRead(value, paramName);
      if (expectedType != typeof (void) && !TypeUtils.AreReferenceAssignable(expectedType, value.Type) && !Expression.TryQuote(expectedType, ref value))
        throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchLabel((object) value.Type, (object) expectedType);
    }

    private static PropertyInfo FindInstanceProperty(Type type, string propertyName, Expression[] arguments)
    {
      BindingFlags flags1 = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
      PropertyInfo property = Expression.FindProperty(type, propertyName, arguments, flags1);
      if (property == (PropertyInfo) null)
      {
        BindingFlags flags2 = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        property = Expression.FindProperty(type, propertyName, arguments, flags2);
      }
      if (!(property == (PropertyInfo) null))
        return property;
      if (arguments == null || arguments.Length == 0)
        throw System.Linq.Expressions.Error.InstancePropertyWithoutParameterNotDefinedForType((object) propertyName, (object) type);
      else
        throw System.Linq.Expressions.Error.InstancePropertyWithSpecifiedParametersNotDefinedForType((object) propertyName, (object) Expression.GetArgTypesString(arguments), (object) type);
    }

    private static string GetArgTypesString(Expression[] arguments)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      stringBuilder.Append("(");
      foreach (Type type in Enumerable.Select<Expression, Type>((IEnumerable<Expression>) arguments, (Func<Expression, Type>) (arg => arg.Type)))
      {
        if (!flag)
          stringBuilder.Append(", ");
        stringBuilder.Append(type.Name);
        flag = false;
      }
      stringBuilder.Append(")");
      return ((object) stringBuilder).ToString();
    }

    private static PropertyInfo FindProperty(Type type, string propertyName, Expression[] arguments, BindingFlags flags)
    {
      MemberInfo[] members = type.FindMembers(MemberTypes.Property, flags, Type.FilterNameIgnoreCase, (object) propertyName);
      if (members == null || members.Length == 0)
        return (PropertyInfo) null;
      PropertyInfo property;
      int bestProperty = Expression.FindBestProperty((IEnumerable<PropertyInfo>) CollectionExtensions.Map<MemberInfo, PropertyInfo>((ICollection<MemberInfo>) members, (Func<MemberInfo, PropertyInfo>) (t => (PropertyInfo) t)), arguments, out property);
      if (bestProperty == 0)
        return (PropertyInfo) null;
      if (bestProperty > 1)
        throw System.Linq.Expressions.Error.PropertyWithMoreThanOneMatch((object) propertyName, (object) type);
      else
        return property;
    }

    private static int FindBestProperty(IEnumerable<PropertyInfo> properties, Expression[] args, out PropertyInfo property)
    {
      int num = 0;
      property = (PropertyInfo) null;
      foreach (PropertyInfo pi in properties)
      {
        if (pi != (PropertyInfo) null && Expression.IsCompatible(pi, args))
        {
          if (property == (PropertyInfo) null)
          {
            property = pi;
            num = 1;
          }
          else
            ++num;
        }
      }
      return num;
    }

    private static bool IsCompatible(PropertyInfo pi, Expression[] args)
    {
      MethodInfo methodInfo = pi.GetGetMethod(true);
      ParameterInfo[] parameterInfoArray;
      if (methodInfo != (MethodInfo) null)
      {
        parameterInfoArray = TypeExtensions.GetParametersCached((MethodBase) methodInfo);
      }
      else
      {
        methodInfo = pi.GetSetMethod(true);
        parameterInfoArray = CollectionExtensions.RemoveLast<ParameterInfo>(TypeExtensions.GetParametersCached((MethodBase) methodInfo));
      }
      if (methodInfo == (MethodInfo) null)
        return false;
      if (args == null)
        return parameterInfoArray.Length == 0;
      if (parameterInfoArray.Length != args.Length)
        return false;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] == null || !TypeUtils.AreReferenceAssignable(parameterInfoArray[index].ParameterType, args[index].Type))
          return false;
      }
      return true;
    }

    private static void ValidateIndexedProperty(Expression instance, PropertyInfo property, ref ReadOnlyCollection<Expression> argList)
    {
      ContractUtils.RequiresNotNull((object) property, "property");
      if (property.PropertyType.IsByRef)
        throw System.Linq.Expressions.Error.PropertyCannotHaveRefType();
      if (property.PropertyType == typeof (void))
        throw System.Linq.Expressions.Error.PropertyTypeCannotBeVoid();
      ParameterInfo[] indexes = (ParameterInfo[]) null;
      MethodInfo getMethod = property.GetGetMethod(true);
      if (getMethod != (MethodInfo) null)
      {
        indexes = TypeExtensions.GetParametersCached((MethodBase) getMethod);
        Expression.ValidateAccessor(instance, getMethod, indexes, ref argList);
      }
      MethodInfo setMethod = property.GetSetMethod(true);
      if (setMethod != (MethodInfo) null)
      {
        ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) setMethod);
        if (parametersCached.Length == 0)
          throw System.Linq.Expressions.Error.SetterHasNoParams();
        Type parameterType = parametersCached[parametersCached.Length - 1].ParameterType;
        if (parameterType.IsByRef)
          throw System.Linq.Expressions.Error.PropertyCannotHaveRefType();
        if (setMethod.ReturnType != typeof (void))
          throw System.Linq.Expressions.Error.SetterMustBeVoid();
        if (property.PropertyType != parameterType)
          throw System.Linq.Expressions.Error.PropertyTyepMustMatchSetter();
        if (getMethod != (MethodInfo) null)
        {
          if (getMethod.IsStatic ^ setMethod.IsStatic)
            throw System.Linq.Expressions.Error.BothAccessorsMustBeStatic();
          if (indexes.Length != parametersCached.Length - 1)
            throw System.Linq.Expressions.Error.IndexesOfSetGetMustMatch();
          for (int index = 0; index < indexes.Length; ++index)
          {
            if (indexes[index].ParameterType != parametersCached[index].ParameterType)
              throw System.Linq.Expressions.Error.IndexesOfSetGetMustMatch();
          }
        }
        else
          Expression.ValidateAccessor(instance, setMethod, CollectionExtensions.RemoveLast<ParameterInfo>(parametersCached), ref argList);
      }
      if (getMethod == (MethodInfo) null && setMethod == (MethodInfo) null)
        throw System.Linq.Expressions.Error.PropertyDoesNotHaveAccessor((object) property);
    }

    private static void ValidateAccessor(Expression instance, MethodInfo method, ParameterInfo[] indexes, ref ReadOnlyCollection<Expression> arguments)
    {
      ContractUtils.RequiresNotNull((object) arguments, "arguments");
      Expression.ValidateMethodInfo(method);
      if ((method.CallingConvention & CallingConventions.VarArgs) != (CallingConventions) 0)
        throw System.Linq.Expressions.Error.AccessorsCannotHaveVarArgs();
      if (method.IsStatic)
      {
        if (instance != null)
          throw System.Linq.Expressions.Error.OnlyStaticMethodsHaveNullInstance();
      }
      else
      {
        if (instance == null)
          throw System.Linq.Expressions.Error.OnlyStaticMethodsHaveNullInstance();
        Expression.RequiresCanRead(instance, "instance");
        Expression.ValidateCallInstanceType(instance.Type, method);
      }
      Expression.ValidateAccessorArgumentTypes(method, indexes, ref arguments);
    }

    private static void ValidateAccessorArgumentTypes(MethodInfo method, ParameterInfo[] indexes, ref ReadOnlyCollection<Expression> arguments)
    {
      if (indexes.Length > 0)
      {
        if (indexes.Length != arguments.Count)
          throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) method);
        Expression[] list = (Expression[]) null;
        int index1 = 0;
        for (int length = indexes.Length; index1 < length; ++index1)
        {
          Expression expression = arguments[index1];
          ParameterInfo parameterInfo = indexes[index1];
          Expression.RequiresCanRead(expression, "arguments");
          Type parameterType = parameterInfo.ParameterType;
          if (parameterType.IsByRef)
            throw System.Linq.Expressions.Error.AccessorsCannotHaveByRefArgs();
          TypeUtils.ValidateType(parameterType);
          if (!TypeUtils.AreReferenceAssignable(parameterType, expression.Type) && !Expression.TryQuote(parameterType, ref expression))
            throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchMethodParameter((object) expression.Type, (object) parameterType, (object) method);
          if (list == null && expression != arguments[index1])
          {
            list = new Expression[arguments.Count];
            for (int index2 = 0; index2 < index1; ++index2)
              list[index2] = arguments[index2];
          }
          if (list != null)
            list[index1] = expression;
        }
        if (list == null)
          return;
        arguments = (ReadOnlyCollection<Expression>) new TrueReadOnlyCollection<Expression>(list);
      }
      else if (arguments.Count > 0)
        throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) method);
    }

    internal static MethodInfo GetInvokeMethod(Expression expression)
    {
      Type type = expression.Type;
      if (!expression.Type.IsSubclassOf(typeof (MulticastDelegate)))
      {
        Type genericType = TypeUtils.FindGenericType(typeof (Expression<>), expression.Type);
        if (genericType == (Type) null)
          throw System.Linq.Expressions.Error.ExpressionTypeNotInvocable((object) expression.Type);
        type = genericType.GetGenericArguments()[0];
      }
      return type.GetMethod("Invoke");
    }

    internal static LambdaExpression CreateLambda(Type delegateType, Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
    {
      CacheDict<Type, Expression.LambdaFactory> cacheDict = Expression._LambdaFactories;
      if (cacheDict == null)
        Expression._LambdaFactories = cacheDict = new CacheDict<Type, Expression.LambdaFactory>(50);
      MethodInfo method = (MethodInfo) null;
      Expression.LambdaFactory lambdaFactory;
      if (!cacheDict.TryGetValue(delegateType, out lambdaFactory))
      {
        method = typeof (Expression<>).MakeGenericType(new Type[1]
        {
          delegateType
        }).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic);
        if (TypeUtils.CanCache(delegateType))
          cacheDict[delegateType] = lambdaFactory = (Expression.LambdaFactory) Delegate.CreateDelegate(typeof (Expression.LambdaFactory), method);
      }
      if (lambdaFactory != null)
        return lambdaFactory(body, name, tailCall, parameters);
      return (LambdaExpression) method.Invoke((object) null, new object[4]
      {
        (object) body,
        (object) name,
        (object) (bool) (tailCall ? 1 : 0),
        (object) parameters
      });
    }

    private static bool ValidateTryGetFuncActionArgs(Type[] typeArgs)
    {
      if (typeArgs == null)
        throw new ArgumentNullException("typeArgs");
      int index = 0;
      for (int length = typeArgs.Length; index < length; ++index)
      {
        Type type = typeArgs[index];
        if (type == (Type) null)
          throw new ArgumentNullException("typeArgs");
        if (type.IsByRef)
          return false;
      }
      return true;
    }

    private static void ValidateSettableFieldOrPropertyMember(MemberInfo member, out Type memberType)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo == (FieldInfo) null)
      {
        PropertyInfo propertyInfo = member as PropertyInfo;
        if (propertyInfo == (PropertyInfo) null)
          throw System.Linq.Expressions.Error.ArgumentMustBeFieldInfoOrPropertInfo();
        if (!propertyInfo.CanWrite)
          throw System.Linq.Expressions.Error.PropertyDoesNotHaveSetter((object) propertyInfo);
        memberType = propertyInfo.PropertyType;
      }
      else
        memberType = fieldInfo.FieldType;
    }

    private static PropertyInfo GetProperty(MethodInfo mi)
    {
      foreach (PropertyInfo propertyInfo in mi.DeclaringType.GetProperties((BindingFlags) (48 | (mi.IsStatic ? 8 : 4))))
      {
        if (propertyInfo.CanRead && Expression.CheckMethod(mi, propertyInfo.GetGetMethod(true)) || propertyInfo.CanWrite && Expression.CheckMethod(mi, propertyInfo.GetSetMethod(true)))
          return propertyInfo;
      }
      throw System.Linq.Expressions.Error.MethodNotPropertyAccessor((object) mi.DeclaringType, (object) mi.Name);
    }

    private static bool CheckMethod(MethodInfo method, MethodInfo propertyMethod)
    {
      if (method == propertyMethod)
        return true;
      Type declaringType = method.DeclaringType;
      return declaringType.IsInterface && method.Name == propertyMethod.Name && declaringType.GetMethod(method.Name) == propertyMethod;
    }

    private static void ValidateListInitArgs(Type listType, ReadOnlyCollection<ElementInit> initializers)
    {
      if (!typeof (IEnumerable).IsAssignableFrom(listType))
        throw System.Linq.Expressions.Error.TypeNotIEnumerable((object) listType);
      int index = 0;
      for (int count = initializers.Count; index < count; ++index)
      {
        ElementInit elementInit = initializers[index];
        ContractUtils.RequiresNotNull((object) elementInit, "initializers");
        Expression.ValidateCallInstanceType(listType, elementInit.AddMethod);
      }
    }

    private static void ValidateGettableFieldOrPropertyMember(MemberInfo member, out Type memberType)
    {
      FieldInfo fieldInfo = member as FieldInfo;
      if (fieldInfo == (FieldInfo) null)
      {
        PropertyInfo propertyInfo = member as PropertyInfo;
        if (propertyInfo == (PropertyInfo) null)
          throw System.Linq.Expressions.Error.ArgumentMustBeFieldInfoOrPropertInfo();
        if (!propertyInfo.CanRead)
          throw System.Linq.Expressions.Error.PropertyDoesNotHaveGetter((object) propertyInfo);
        memberType = propertyInfo.PropertyType;
      }
      else
        memberType = fieldInfo.FieldType;
    }

    private static void ValidateMemberInitArgs(Type type, ReadOnlyCollection<MemberBinding> bindings)
    {
      int index = 0;
      for (int count = bindings.Count; index < count; ++index)
      {
        MemberBinding memberBinding = bindings[index];
        ContractUtils.RequiresNotNull((object) memberBinding, "bindings");
        if (!memberBinding.Member.DeclaringType.IsAssignableFrom(type))
          throw System.Linq.Expressions.Error.NotAMemberOfType((object) memberBinding.Member.Name, (object) type);
      }
    }

    private static ParameterInfo[] ValidateMethodAndGetParameters(Expression instance, MethodInfo method)
    {
      Expression.ValidateMethodInfo(method);
      Expression.ValidateStaticOrInstanceMethod(instance, method);
      return Expression.GetParametersForValidation((MethodBase) method, ExpressionType.Call);
    }

    private static void ValidateStaticOrInstanceMethod(Expression instance, MethodInfo method)
    {
      if (method.IsStatic)
      {
        if (instance != null)
          throw new ArgumentException(System.Linq.Expressions.Strings.OnlyStaticMethodsHaveNullInstance, "instance");
      }
      else
      {
        if (instance == null)
          throw new ArgumentException(System.Linq.Expressions.Strings.OnlyStaticMethodsHaveNullInstance, "method");
        Expression.RequiresCanRead(instance, "instance");
        Expression.ValidateCallInstanceType(instance.Type, method);
      }
    }

    private static void ValidateCallInstanceType(Type instanceType, MethodInfo method)
    {
      if (!TypeUtils.IsValidInstanceType((MemberInfo) method, instanceType))
        throw System.Linq.Expressions.Error.InstanceAndMethodTypeMismatch((object) method, (object) method.DeclaringType, (object) instanceType);
    }

    private static void ValidateArgumentTypes(MethodBase method, ExpressionType nodeKind, ref ReadOnlyCollection<Expression> arguments)
    {
      ParameterInfo[] parametersForValidation = Expression.GetParametersForValidation(method, nodeKind);
      Expression.ValidateArgumentCount(method, nodeKind, arguments.Count, parametersForValidation);
      Expression[] list = (Expression[]) null;
      int index1 = 0;
      for (int length = parametersForValidation.Length; index1 < length; ++index1)
      {
        Expression expression1 = arguments[index1];
        ParameterInfo pi = parametersForValidation[index1];
        Expression expression2 = Expression.ValidateOneArgument(method, nodeKind, expression1, pi);
        if (list == null && expression2 != arguments[index1])
        {
          list = new Expression[arguments.Count];
          for (int index2 = 0; index2 < index1; ++index2)
            list[index2] = arguments[index2];
        }
        if (list != null)
          list[index1] = expression2;
      }
      if (list == null)
        return;
      arguments = (ReadOnlyCollection<Expression>) new TrueReadOnlyCollection<Expression>(list);
    }

    private static ParameterInfo[] GetParametersForValidation(MethodBase method, ExpressionType nodeKind)
    {
      ParameterInfo[] array = TypeExtensions.GetParametersCached(method);
      if (nodeKind == ExpressionType.Dynamic)
        array = CollectionExtensions.RemoveFirst<ParameterInfo>(array);
      return array;
    }

    private static void ValidateArgumentCount(MethodBase method, ExpressionType nodeKind, int count, ParameterInfo[] pis)
    {
      if (pis.Length == count)
        return;
      switch (nodeKind)
      {
        case ExpressionType.New:
          throw System.Linq.Expressions.Error.IncorrectNumberOfConstructorArguments();
        case ExpressionType.Dynamic:
        case ExpressionType.Call:
          throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) method);
        case ExpressionType.Invoke:
          throw System.Linq.Expressions.Error.IncorrectNumberOfLambdaArguments();
        default:
          throw ContractUtils.Unreachable;
      }
    }

    private static Expression ValidateOneArgument(MethodBase method, ExpressionType nodeKind, Expression arg, ParameterInfo pi)
    {
      Expression.RequiresCanRead(arg, "arguments");
      Type type = pi.ParameterType;
      if (type.IsByRef)
        type = type.GetElementType();
      TypeUtils.ValidateType(type);
      if (TypeUtils.AreReferenceAssignable(type, arg.Type) || Expression.TryQuote(type, ref arg))
        return arg;
      switch (nodeKind)
      {
        case ExpressionType.New:
          throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchConstructorParameter((object) arg.Type, (object) type);
        case ExpressionType.Dynamic:
        case ExpressionType.Call:
          throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchMethodParameter((object) arg.Type, (object) type, (object) method);
        case ExpressionType.Invoke:
          throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchParameter((object) arg.Type, (object) type);
        default:
          throw ContractUtils.Unreachable;
      }
    }

    private static bool TryQuote(Type parameterType, ref Expression argument)
    {
      if (!TypeUtils.IsSameOrSubclass(typeof (LambdaExpression), parameterType) || !parameterType.IsAssignableFrom(argument.GetType()))
        return false;
      argument = (Expression) Expression.Quote(argument);
      return true;
    }

    private static MethodInfo FindMethod(Type type, string methodName, Type[] typeArgs, Expression[] args, BindingFlags flags)
    {
      MemberInfo[] members = type.FindMembers(MemberTypes.Method, flags, Type.FilterNameIgnoreCase, (object) methodName);
      if (members == null || members.Length == 0)
        throw System.Linq.Expressions.Error.MethodDoesNotExistOnType((object) methodName, (object) type);
      MethodInfo method;
      int bestMethod = Expression.FindBestMethod((IEnumerable<MethodInfo>) CollectionExtensions.Map<MemberInfo, MethodInfo>((ICollection<MemberInfo>) members, (Func<MemberInfo, MethodInfo>) (t => (MethodInfo) t)), typeArgs, args, out method);
      if (bestMethod == 0)
      {
        if (typeArgs != null && typeArgs.Length > 0)
          throw System.Linq.Expressions.Error.GenericMethodWithArgsDoesNotExistOnType((object) methodName, (object) type);
        else
          throw System.Linq.Expressions.Error.MethodWithArgsDoesNotExistOnType((object) methodName, (object) type);
      }
      else if (bestMethod > 1)
        throw System.Linq.Expressions.Error.MethodWithMoreThanOneMatch((object) methodName, (object) type);
      else
        return method;
    }

    private static int FindBestMethod(IEnumerable<MethodInfo> methods, Type[] typeArgs, Expression[] args, out MethodInfo method)
    {
      int num = 0;
      method = (MethodInfo) null;
      foreach (MethodInfo m in methods)
      {
        MethodInfo methodInfo = Expression.ApplyTypeArgs(m, typeArgs);
        if (methodInfo != (MethodInfo) null && Expression.IsCompatible((MethodBase) methodInfo, args))
        {
          if (method == (MethodInfo) null || !method.IsPublic && methodInfo.IsPublic)
          {
            method = methodInfo;
            num = 1;
          }
          else if (method.IsPublic == methodInfo.IsPublic)
            ++num;
        }
      }
      return num;
    }

    private static bool IsCompatible(MethodBase m, Expression[] args)
    {
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached(m);
      if (parametersCached.Length != args.Length)
        return false;
      for (int index = 0; index < args.Length; ++index)
      {
        Expression expression = args[index];
        ContractUtils.RequiresNotNull((object) expression, "argument");
        Type type1 = expression.Type;
        Type type2 = parametersCached[index].ParameterType;
        if (type2.IsByRef)
          type2 = type2.GetElementType();
        if (!TypeUtils.AreReferenceAssignable(type2, type1) && (!TypeUtils.IsSameOrSubclass(typeof (LambdaExpression), type2) || !type2.IsAssignableFrom(expression.GetType())))
          return false;
      }
      return true;
    }

    private static MethodInfo ApplyTypeArgs(MethodInfo m, Type[] typeArgs)
    {
      if (typeArgs == null || typeArgs.Length == 0)
      {
        if (!m.IsGenericMethodDefinition)
          return m;
      }
      else if (m.IsGenericMethodDefinition && m.GetGenericArguments().Length == typeArgs.Length)
        return m.MakeGenericMethod(typeArgs);
      return (MethodInfo) null;
    }

    private static void ValidateNewArgs(ConstructorInfo constructor, ref ReadOnlyCollection<Expression> arguments, ref ReadOnlyCollection<MemberInfo> members)
    {
      ParameterInfo[] parametersCached;
      if ((parametersCached = TypeExtensions.GetParametersCached((MethodBase) constructor)).Length > 0)
      {
        if (arguments.Count != parametersCached.Length)
          throw System.Linq.Expressions.Error.IncorrectNumberOfConstructorArguments();
        if (arguments.Count != members.Count)
          throw System.Linq.Expressions.Error.IncorrectNumberOfArgumentsForMembers();
        Expression[] list1 = (Expression[]) null;
        MemberInfo[] list2 = (MemberInfo[]) null;
        int index1 = 0;
        for (int count = arguments.Count; index1 < count; ++index1)
        {
          Expression expression = arguments[index1];
          Expression.RequiresCanRead(expression, "argument");
          MemberInfo member = members[index1];
          ContractUtils.RequiresNotNull((object) member, "member");
          if (!TypeUtils.AreEquivalent(member.DeclaringType, constructor.DeclaringType))
            throw System.Linq.Expressions.Error.ArgumentMemberNotDeclOnType((object) member.Name, (object) constructor.DeclaringType.Name);
          Type memberType;
          Expression.ValidateAnonymousTypeMember(ref member, out memberType);
          if (!TypeUtils.AreReferenceAssignable(memberType, expression.Type) && !Expression.TryQuote(memberType, ref expression))
            throw System.Linq.Expressions.Error.ArgumentTypeDoesNotMatchMember((object) expression.Type, (object) memberType);
          Type type = parametersCached[index1].ParameterType;
          if (type.IsByRef)
            type = type.GetElementType();
          if (!TypeUtils.AreReferenceAssignable(type, expression.Type) && !Expression.TryQuote(type, ref expression))
            throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchConstructorParameter((object) expression.Type, (object) type);
          if (list1 == null && expression != arguments[index1])
          {
            list1 = new Expression[arguments.Count];
            for (int index2 = 0; index2 < index1; ++index2)
              list1[index2] = arguments[index2];
          }
          if (list1 != null)
            list1[index1] = expression;
          if (list2 == null && member != members[index1])
          {
            list2 = new MemberInfo[members.Count];
            for (int index2 = 0; index2 < index1; ++index2)
              list2[index2] = members[index2];
          }
          if (list2 != null)
            list2[index1] = member;
        }
        if (list1 != null)
          arguments = (ReadOnlyCollection<Expression>) new TrueReadOnlyCollection<Expression>(list1);
        if (list2 == null)
          return;
        members = (ReadOnlyCollection<MemberInfo>) new TrueReadOnlyCollection<MemberInfo>(list2);
      }
      else
      {
        if (arguments != null && arguments.Count > 0)
          throw System.Linq.Expressions.Error.IncorrectNumberOfConstructorArguments();
        if (members != null && members.Count > 0)
          throw System.Linq.Expressions.Error.IncorrectNumberOfMembersForGivenConstructor();
      }
    }

    private static void ValidateAnonymousTypeMember(ref MemberInfo member, out Type memberType)
    {
      switch (member.MemberType)
      {
        case MemberTypes.Field:
          FieldInfo fieldInfo = member as FieldInfo;
          if (fieldInfo.IsStatic)
            throw System.Linq.Expressions.Error.ArgumentMustBeInstanceMember();
          memberType = fieldInfo.FieldType;
          break;
        case MemberTypes.Method:
          MethodInfo mi = member as MethodInfo;
          if (mi.IsStatic)
            throw System.Linq.Expressions.Error.ArgumentMustBeInstanceMember();
          PropertyInfo property = Expression.GetProperty(mi);
          member = (MemberInfo) property;
          memberType = property.PropertyType;
          break;
        case MemberTypes.Property:
          PropertyInfo propertyInfo = member as PropertyInfo;
          if (!propertyInfo.CanRead)
            throw System.Linq.Expressions.Error.PropertyDoesNotHaveGetter((object) propertyInfo);
          if (propertyInfo.GetGetMethod().IsStatic)
            throw System.Linq.Expressions.Error.ArgumentMustBeInstanceMember();
          memberType = propertyInfo.PropertyType;
          break;
        default:
          throw System.Linq.Expressions.Error.ArgumentMustBeFieldInfoOrPropertInfoOrMethod();
      }
    }

    private static void ValidateSwitchCaseType(Expression @case, bool customType, Type resultType, string parameterName)
    {
      if (customType)
      {
        if (resultType != typeof (void) && !TypeUtils.AreReferenceAssignable(resultType, @case.Type))
          throw new ArgumentException(System.Linq.Expressions.Strings.ArgumentTypesMustMatch, parameterName);
      }
      else if (!TypeUtils.AreEquivalent(resultType, @case.Type))
        throw new ArgumentException(System.Linq.Expressions.Strings.AllCaseBodiesMustHaveSameType, parameterName);
    }

    private static void ValidateTryAndCatchHaveSameType(Type type, Expression tryBody, ReadOnlyCollection<CatchBlock> handlers)
    {
      if (type != (Type) null)
      {
        if (!(type != typeof (void)))
          return;
        if (!TypeUtils.AreReferenceAssignable(type, tryBody.Type))
          throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
        foreach (CatchBlock catchBlock in handlers)
        {
          if (!TypeUtils.AreReferenceAssignable(type, catchBlock.Body.Type))
            throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
        }
      }
      else if (tryBody == null || tryBody.Type == typeof (void))
      {
        foreach (CatchBlock catchBlock in handlers)
        {
          if (catchBlock.Body != null && catchBlock.Body.Type != typeof (void))
            throw System.Linq.Expressions.Error.BodyOfCatchMustHaveSameTypeAsBodyOfTry();
        }
      }
      else
      {
        type = tryBody.Type;
        foreach (CatchBlock catchBlock in handlers)
        {
          if (catchBlock.Body == null || !TypeUtils.AreEquivalent(catchBlock.Body.Type, type))
            throw System.Linq.Expressions.Error.BodyOfCatchMustHaveSameTypeAsBodyOfTry();
        }
      }
    }

    private static UnaryExpression GetUserDefinedUnaryOperatorOrThrow(ExpressionType unaryType, string name, Expression operand)
    {
      UnaryExpression definedUnaryOperator = Expression.GetUserDefinedUnaryOperator(unaryType, name, operand);
      if (definedUnaryOperator == null)
        throw System.Linq.Expressions.Error.UnaryOperatorNotDefined((object) unaryType, (object) operand.Type);
      Expression.ValidateParamswithOperandsOrThrow(TypeExtensions.GetParametersCached((MethodBase) definedUnaryOperator.Method)[0].ParameterType, operand.Type, unaryType, name);
      return definedUnaryOperator;
    }

    private static UnaryExpression GetUserDefinedUnaryOperator(ExpressionType unaryType, string name, Expression operand)
    {
      Type type = operand.Type;
      Type[] types = new Type[1]
      {
        type
      };
      Type nonNullableType = TypeUtils.GetNonNullableType(type);
      BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      MethodInfo methodValidated1 = TypeExtensions.GetMethodValidated(nonNullableType, name, bindingAttr, (Binder) null, types, (ParameterModifier[]) null);
      if (methodValidated1 != (MethodInfo) null)
        return new UnaryExpression(unaryType, operand, methodValidated1.ReturnType, methodValidated1);
      if (TypeUtils.IsNullableType(type))
      {
        types[0] = nonNullableType;
        MethodInfo methodValidated2 = TypeExtensions.GetMethodValidated(nonNullableType, name, bindingAttr, (Binder) null, types, (ParameterModifier[]) null);
        if (methodValidated2 != (MethodInfo) null && methodValidated2.ReturnType.IsValueType && !TypeUtils.IsNullableType(methodValidated2.ReturnType))
          return new UnaryExpression(unaryType, operand, TypeUtils.GetNullableType(methodValidated2.ReturnType), methodValidated2);
      }
      return (UnaryExpression) null;
    }

    private static UnaryExpression GetMethodBasedUnaryOperator(ExpressionType unaryType, Expression operand, MethodInfo method)
    {
      Expression.ValidateOperator(method);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) method);
      if (parametersCached.Length != 1)
        throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) method);
      if (Expression.ParameterIsAssignable(parametersCached[0], operand.Type))
      {
        Expression.ValidateParamswithOperandsOrThrow(parametersCached[0].ParameterType, operand.Type, unaryType, method.Name);
        return new UnaryExpression(unaryType, operand, method.ReturnType, method);
      }
      else if (TypeUtils.IsNullableType(operand.Type) && Expression.ParameterIsAssignable(parametersCached[0], TypeUtils.GetNonNullableType(operand.Type)) && (method.ReturnType.IsValueType && !TypeUtils.IsNullableType(method.ReturnType)))
        return new UnaryExpression(unaryType, operand, TypeUtils.GetNullableType(method.ReturnType), method);
      else
        throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) unaryType, (object) method.Name);
    }

    private static UnaryExpression GetUserDefinedCoercionOrThrow(ExpressionType coercionType, Expression expression, Type convertToType)
    {
      UnaryExpression userDefinedCoercion = Expression.GetUserDefinedCoercion(coercionType, expression, convertToType);
      if (userDefinedCoercion != null)
        return userDefinedCoercion;
      else
        throw System.Linq.Expressions.Error.CoercionOperatorNotDefined((object) expression.Type, (object) convertToType);
    }

    private static UnaryExpression GetUserDefinedCoercion(ExpressionType coercionType, Expression expression, Type convertToType)
    {
      MethodInfo definedCoercionMethod = TypeUtils.GetUserDefinedCoercionMethod(expression.Type, convertToType, false);
      if (definedCoercionMethod != (MethodInfo) null)
        return new UnaryExpression(coercionType, expression, convertToType, definedCoercionMethod);
      else
        return (UnaryExpression) null;
    }

    private static UnaryExpression GetMethodBasedCoercionOperator(ExpressionType unaryType, Expression operand, Type convertToType, MethodInfo method)
    {
      Expression.ValidateOperator(method);
      ParameterInfo[] parametersCached = TypeExtensions.GetParametersCached((MethodBase) method);
      if (parametersCached.Length != 1)
        throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments((object) method);
      if (Expression.ParameterIsAssignable(parametersCached[0], operand.Type) && TypeUtils.AreEquivalent(method.ReturnType, convertToType))
        return new UnaryExpression(unaryType, operand, method.ReturnType, method);
      if ((TypeUtils.IsNullableType(operand.Type) || TypeUtils.IsNullableType(convertToType)) && (Expression.ParameterIsAssignable(parametersCached[0], TypeUtils.GetNonNullableType(operand.Type)) && TypeUtils.AreEquivalent(method.ReturnType, TypeUtils.GetNonNullableType(convertToType))))
        return new UnaryExpression(unaryType, operand, convertToType, method);
      else
        throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters((object) unaryType, (object) method.Name);
    }

    private delegate LambdaExpression LambdaFactory(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters);

    private class ExtensionInfo
    {
      internal readonly ExpressionType NodeType;
      internal readonly Type Type;

      public ExtensionInfo(ExpressionType nodeType, Type type)
      {
        this.NodeType = nodeType;
        this.Type = type;
      }
    }

    internal class BinaryExpressionProxy
    {
      private readonly BinaryExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public LambdaExpression Conversion
      {
        get
        {
          return this._node.Conversion;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public bool IsLifted
      {
        get
        {
          return this._node.IsLifted;
        }
      }

      public bool IsLiftedToNull
      {
        get
        {
          return this._node.IsLiftedToNull;
        }
      }

      public Expression Left
      {
        get
        {
          return this._node.Left;
        }
      }

      public MethodInfo Method
      {
        get
        {
          return this._node.Method;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Expression Right
      {
        get
        {
          return this._node.Right;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public BinaryExpressionProxy(BinaryExpression node)
      {
        this._node = node;
      }
    }

    internal class BlockExpressionProxy
    {
      private readonly BlockExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public ReadOnlyCollection<Expression> Expressions
      {
        get
        {
          return this._node.Expressions;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Expression Result
      {
        get
        {
          return this._node.Result;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public ReadOnlyCollection<ParameterExpression> Variables
      {
        get
        {
          return this._node.Variables;
        }
      }

      public BlockExpressionProxy(BlockExpression node)
      {
        this._node = node;
      }
    }

    internal class CatchBlockProxy
    {
      private readonly CatchBlock _node;

      public Expression Body
      {
        get
        {
          return this._node.Body;
        }
      }

      public Expression Filter
      {
        get
        {
          return this._node.Filter;
        }
      }

      public Type Test
      {
        get
        {
          return this._node.Test;
        }
      }

      public ParameterExpression Variable
      {
        get
        {
          return this._node.Variable;
        }
      }

      public CatchBlockProxy(CatchBlock node)
      {
        this._node = node;
      }
    }

    internal class ConditionalExpressionProxy
    {
      private readonly ConditionalExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public Expression IfFalse
      {
        get
        {
          return this._node.IfFalse;
        }
      }

      public Expression IfTrue
      {
        get
        {
          return this._node.IfTrue;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Expression Test
      {
        get
        {
          return this._node.Test;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public ConditionalExpressionProxy(ConditionalExpression node)
      {
        this._node = node;
      }
    }

    internal class ConstantExpressionProxy
    {
      private readonly ConstantExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public object Value
      {
        get
        {
          return this._node.Value;
        }
      }

      public ConstantExpressionProxy(ConstantExpression node)
      {
        this._node = node;
      }
    }

    internal class DebugInfoExpressionProxy
    {
      private readonly DebugInfoExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public SymbolDocumentInfo Document
      {
        get
        {
          return this._node.Document;
        }
      }

      public int EndColumn
      {
        get
        {
          return this._node.EndColumn;
        }
      }

      public int EndLine
      {
        get
        {
          return this._node.EndLine;
        }
      }

      public bool IsClear
      {
        get
        {
          return this._node.IsClear;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public int StartColumn
      {
        get
        {
          return this._node.StartColumn;
        }
      }

      public int StartLine
      {
        get
        {
          return this._node.StartLine;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public DebugInfoExpressionProxy(DebugInfoExpression node)
      {
        this._node = node;
      }
    }

    internal class DefaultExpressionProxy
    {
      private readonly DefaultExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public DefaultExpressionProxy(DefaultExpression node)
      {
        this._node = node;
      }
    }

    internal class DynamicExpressionProxy
    {
      private readonly DynamicExpression _node;

      public ReadOnlyCollection<Expression> Arguments
      {
        get
        {
          return this._node.Arguments;
        }
      }

      public CallSiteBinder Binder
      {
        get
        {
          return this._node.Binder;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public Type DelegateType
      {
        get
        {
          return this._node.DelegateType;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public DynamicExpressionProxy(DynamicExpression node)
      {
        this._node = node;
      }
    }

    internal class GotoExpressionProxy
    {
      private readonly GotoExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public GotoExpressionKind Kind
      {
        get
        {
          return this._node.Kind;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public LabelTarget Target
      {
        get
        {
          return this._node.Target;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public Expression Value
      {
        get
        {
          return this._node.Value;
        }
      }

      public GotoExpressionProxy(GotoExpression node)
      {
        this._node = node;
      }
    }

    internal class IndexExpressionProxy
    {
      private readonly IndexExpression _node;

      public ReadOnlyCollection<Expression> Arguments
      {
        get
        {
          return this._node.Arguments;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public PropertyInfo Indexer
      {
        get
        {
          return this._node.Indexer;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Expression Object
      {
        get
        {
          return this._node.Object;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public IndexExpressionProxy(IndexExpression node)
      {
        this._node = node;
      }
    }

    internal class InvocationExpressionProxy
    {
      private readonly InvocationExpression _node;

      public ReadOnlyCollection<Expression> Arguments
      {
        get
        {
          return this._node.Arguments;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public Expression Expression
      {
        get
        {
          return this._node.Expression;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public InvocationExpressionProxy(InvocationExpression node)
      {
        this._node = node;
      }
    }

    internal class LabelExpressionProxy
    {
      private readonly LabelExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public Expression DefaultValue
      {
        get
        {
          return this._node.DefaultValue;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public LabelTarget Target
      {
        get
        {
          return this._node.Target;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public LabelExpressionProxy(LabelExpression node)
      {
        this._node = node;
      }
    }

    internal class LambdaExpressionProxy
    {
      private readonly LambdaExpression _node;

      public Expression Body
      {
        get
        {
          return this._node.Body;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public string Name
      {
        get
        {
          return this._node.Name;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public ReadOnlyCollection<ParameterExpression> Parameters
      {
        get
        {
          return this._node.Parameters;
        }
      }

      public Type ReturnType
      {
        get
        {
          return this._node.ReturnType;
        }
      }

      public bool TailCall
      {
        get
        {
          return this._node.TailCall;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public LambdaExpressionProxy(LambdaExpression node)
      {
        this._node = node;
      }
    }

    internal class ListInitExpressionProxy
    {
      private readonly ListInitExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public ReadOnlyCollection<ElementInit> Initializers
      {
        get
        {
          return this._node.Initializers;
        }
      }

      public NewExpression NewExpression
      {
        get
        {
          return this._node.NewExpression;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public ListInitExpressionProxy(ListInitExpression node)
      {
        this._node = node;
      }
    }

    internal class LoopExpressionProxy
    {
      private readonly LoopExpression _node;

      public Expression Body
      {
        get
        {
          return this._node.Body;
        }
      }

      public LabelTarget BreakLabel
      {
        get
        {
          return this._node.BreakLabel;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public LabelTarget ContinueLabel
      {
        get
        {
          return this._node.ContinueLabel;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public LoopExpressionProxy(LoopExpression node)
      {
        this._node = node;
      }
    }

    internal class MemberExpressionProxy
    {
      private readonly MemberExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public Expression Expression
      {
        get
        {
          return this._node.Expression;
        }
      }

      public MemberInfo Member
      {
        get
        {
          return this._node.Member;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public MemberExpressionProxy(MemberExpression node)
      {
        this._node = node;
      }
    }

    internal class MemberInitExpressionProxy
    {
      private readonly MemberInitExpression _node;

      public ReadOnlyCollection<MemberBinding> Bindings
      {
        get
        {
          return this._node.Bindings;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public NewExpression NewExpression
      {
        get
        {
          return this._node.NewExpression;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public MemberInitExpressionProxy(MemberInitExpression node)
      {
        this._node = node;
      }
    }

    internal class MethodCallExpressionProxy
    {
      private readonly MethodCallExpression _node;

      public ReadOnlyCollection<Expression> Arguments
      {
        get
        {
          return this._node.Arguments;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public MethodInfo Method
      {
        get
        {
          return this._node.Method;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Expression Object
      {
        get
        {
          return this._node.Object;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public MethodCallExpressionProxy(MethodCallExpression node)
      {
        this._node = node;
      }
    }

    internal class NewArrayExpressionProxy
    {
      private readonly NewArrayExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public ReadOnlyCollection<Expression> Expressions
      {
        get
        {
          return this._node.Expressions;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public NewArrayExpressionProxy(NewArrayExpression node)
      {
        this._node = node;
      }
    }

    internal class NewExpressionProxy
    {
      private readonly NewExpression _node;

      public ReadOnlyCollection<Expression> Arguments
      {
        get
        {
          return this._node.Arguments;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public ConstructorInfo Constructor
      {
        get
        {
          return this._node.Constructor;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public ReadOnlyCollection<MemberInfo> Members
      {
        get
        {
          return this._node.Members;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public NewExpressionProxy(NewExpression node)
      {
        this._node = node;
      }
    }

    internal class ParameterExpressionProxy
    {
      private readonly ParameterExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public bool IsByRef
      {
        get
        {
          return this._node.IsByRef;
        }
      }

      public string Name
      {
        get
        {
          return this._node.Name;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public ParameterExpressionProxy(ParameterExpression node)
      {
        this._node = node;
      }
    }

    internal class RuntimeVariablesExpressionProxy
    {
      private readonly RuntimeVariablesExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public ReadOnlyCollection<ParameterExpression> Variables
      {
        get
        {
          return this._node.Variables;
        }
      }

      public RuntimeVariablesExpressionProxy(RuntimeVariablesExpression node)
      {
        this._node = node;
      }
    }

    internal class SwitchCaseProxy
    {
      private readonly SwitchCase _node;

      public Expression Body
      {
        get
        {
          return this._node.Body;
        }
      }

      public ReadOnlyCollection<Expression> TestValues
      {
        get
        {
          return this._node.TestValues;
        }
      }

      public SwitchCaseProxy(SwitchCase node)
      {
        this._node = node;
      }
    }

    internal class SwitchExpressionProxy
    {
      private readonly SwitchExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public ReadOnlyCollection<SwitchCase> Cases
      {
        get
        {
          return this._node.Cases;
        }
      }

      public MethodInfo Comparison
      {
        get
        {
          return this._node.Comparison;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public Expression DefaultBody
      {
        get
        {
          return this._node.DefaultBody;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Expression SwitchValue
      {
        get
        {
          return this._node.SwitchValue;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public SwitchExpressionProxy(SwitchExpression node)
      {
        this._node = node;
      }
    }

    internal class TryExpressionProxy
    {
      private readonly TryExpression _node;

      public Expression Body
      {
        get
        {
          return this._node.Body;
        }
      }

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public Expression Fault
      {
        get
        {
          return this._node.Fault;
        }
      }

      public Expression Finally
      {
        get
        {
          return this._node.Finally;
        }
      }

      public ReadOnlyCollection<CatchBlock> Handlers
      {
        get
        {
          return this._node.Handlers;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public TryExpressionProxy(TryExpression node)
      {
        this._node = node;
      }
    }

    internal class TypeBinaryExpressionProxy
    {
      private readonly TypeBinaryExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public Expression Expression
      {
        get
        {
          return this._node.Expression;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public Type TypeOperand
      {
        get
        {
          return this._node.TypeOperand;
        }
      }

      public TypeBinaryExpressionProxy(TypeBinaryExpression node)
      {
        this._node = node;
      }
    }

    internal class UnaryExpressionProxy
    {
      private readonly UnaryExpression _node;

      public bool CanReduce
      {
        get
        {
          return this._node.CanReduce;
        }
      }

      public string DebugView
      {
        get
        {
          return this._node.DebugView;
        }
      }

      public bool IsLifted
      {
        get
        {
          return this._node.IsLifted;
        }
      }

      public bool IsLiftedToNull
      {
        get
        {
          return this._node.IsLiftedToNull;
        }
      }

      public MethodInfo Method
      {
        get
        {
          return this._node.Method;
        }
      }

      public ExpressionType NodeType
      {
        get
        {
          return this._node.NodeType;
        }
      }

      public Expression Operand
      {
        get
        {
          return this._node.Operand;
        }
      }

      public Type Type
      {
        get
        {
          return this._node.Type;
        }
      }

      public UnaryExpressionProxy(UnaryExpression node)
      {
        this._node = node;
      }
    }
  }
}

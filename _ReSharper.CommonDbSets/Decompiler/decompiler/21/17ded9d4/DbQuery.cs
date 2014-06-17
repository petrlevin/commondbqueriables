// Type: System.Data.Entity.Infrastructure.DbQuery`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: D:\VSProjects\CommonDbSets\packages\EntityFramework.6.1.0\lib\net40\EntityFramework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Internal.Linq;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Represents a LINQ to Entities query against a DbContext.
  /// 
  /// </summary>
  /// <typeparam name="TResult">The type of entity to query for. </typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Name is intentional")]
  public class DbQuery<TResult> : IOrderedQueryable<TResult>, IQueryable<TResult>, IEnumerable<TResult>, IOrderedQueryable, IQueryable, IEnumerable, IListSource, IInternalQueryAdapter
  {
    private readonly IInternalQuery<TResult> _internalQuery;
    private IQueryProvider _provider;

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    bool IListSource.ContainsListCollection
    {
      get
      {
        return false;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    Type IQueryable.ElementType
    {
      get
      {
        return this.GetInternalQueryWithCheck("IQueryable.ElementType").ElementType;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    Expression IQueryable.Expression
    {
      get
      {
        return this.GetInternalQueryWithCheck("IQueryable.Expression").Expression;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IQueryProvider IQueryable.Provider
    {
      get
      {
        return this._provider ?? (this._provider = (IQueryProvider) new DbQueryProvider(this.GetInternalQueryWithCheck("IQueryable.Provider").InternalContext, (IInternalQuery) this.GetInternalQueryWithCheck("IQueryable.Provider")));
      }
    }

    IInternalQuery IInternalQueryAdapter.InternalQuery
    {
      get
      {
        return (IInternalQuery) this._internalQuery;
      }
    }

    internal IInternalQuery<TResult> InternalQuery
    {
      get
      {
        return this._internalQuery;
      }
    }

    internal DbQuery(IInternalQuery<TResult> internalQuery)
    {
      this._internalQuery = internalQuery;
    }

    /// <summary>
    /// Returns a new instance of the non-generic <see cref="T:System.Data.Entity.Infrastructure.DbQuery"/> class for this query.
    /// 
    /// </summary>
    /// <param name="entry">The query.</param>
    /// <returns>
    /// A non-generic version.
    /// </returns>
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Intentionally just implicit to reduce API clutter.")]
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public static implicit operator DbQuery(DbQuery<TResult> entry)
    {
      if (entry._internalQuery == null)
        throw new NotSupportedException(Strings.TestDoublesCannotBeConverted);
      else
        return (DbQuery) new InternalDbQuery<TResult>(entry._internalQuery);
    }

    /// <summary>
    /// Specifies the related objects to include in the query results.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// Paths are all-inclusive. For example, if an include call indicates Include("Orders.OrderLines"), not only will
    ///             OrderLines be included, but also Orders.  When you call the Include method, the query path is only valid on
    ///             the returned instance of the DbQuery&lt;T&gt;. Other instances of DbQuery&lt;T&gt; and the object context itself are not affected.
    ///             Because the Include method returns the query object, you can call this method multiple times on an DbQuery&lt;T&gt; to
    ///             specify multiple paths for the query.
    /// 
    /// </remarks>
    /// <param name="path">The dot-separated list of related objects to return in the query results. </param>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Infrastructure.DbQuery`1"/> with the defined query path.
    /// 
    /// </returns>
    public virtual DbQuery<TResult> Include(string path)
    {
      Check.NotEmpty(path, "path");
      if (this._internalQuery != null)
        return new DbQuery<TResult>(this._internalQuery.Include(path));
      else
        return this;
    }

    /// <summary>
    /// Returns a new query where the entities returned will not be cached in the <see cref="T:System.Data.Entity.DbContext"/>.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A new query with NoTracking applied.
    /// </returns>
    public virtual DbQuery<TResult> AsNoTracking()
    {
      if (this._internalQuery != null)
        return new DbQuery<TResult>(this._internalQuery.AsNoTracking());
      else
        return this;
    }

    /// <summary>
    /// Returns a new query that will stream the results instead of buffering.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A new query with AsStreaming applied.
    /// </returns>
    [Obsolete("Queries are now streaming by default unless a retrying ExecutionStrategy is used. Calling this method will have no effect.")]
    public virtual DbQuery<TResult> AsStreaming()
    {
      if (this._internalQuery != null)
        return new DbQuery<TResult>(this._internalQuery.AsStreaming());
      else
        return this;
    }

    internal virtual DbQuery<TResult> WithExecutionStrategy(IDbExecutionStrategy executionStrategy)
    {
      if (this._internalQuery != null)
        return new DbQuery<TResult>(this._internalQuery.WithExecutionStrategy(executionStrategy));
      else
        return this;
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IList IListSource.GetList()
    {
      throw Error.DbQuery_BindingToDbQueryNotSupported();
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
    {
      return this.GetInternalQueryWithCheck("IEnumerable<TResult>.GetEnumerator").GetEnumerator();
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetInternalQueryWithCheck("IEnumerable.GetEnumerator").GetEnumerator();
    }

    private IInternalQuery<TResult> GetInternalQueryWithCheck(string memberName)
    {
      if (this._internalQuery == null)
        throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) memberName, (object) this.GetType().Name, (object) typeof (DbSet<>).Name));
      else
        return this._internalQuery;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> representation of the underlying query.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// The query string.
    /// </returns>
    public override string ToString()
    {
      if (this._internalQuery != null)
        return this._internalQuery.ToString();
      else
        return base.ToString();
    }

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}

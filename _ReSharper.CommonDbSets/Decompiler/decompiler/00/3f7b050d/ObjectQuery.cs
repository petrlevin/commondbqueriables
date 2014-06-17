// Type: System.Data.Entity.Core.Objects.ObjectQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: D:\VSProjects\CommonDbSets\packages\EntityFramework.6.1.0\lib\net40\EntityFramework.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Core.Objects.Internal;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// This class implements untyped queries at the object-layer.
  /// 
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
  public abstract class ObjectQuery : IOrderedQueryable, IQueryable, IEnumerable, IListSource
  {
    private readonly ObjectQueryState _state;
    private TypeUsage _resultType;
    private ObjectQueryProvider _provider;

    internal ObjectQueryState QueryState
    {
      get
      {
        return this._state;
      }
    }

    internal virtual ObjectQueryProvider ObjectQueryProvider
    {
      get
      {
        if (this._provider == null)
          this._provider = new ObjectQueryProvider(this);
        return this._provider;
      }
    }

    internal IDbExecutionStrategy ExecutionStrategy
    {
      get
      {
        return this.QueryState.ExecutionStrategy;
      }
      set
      {
        this.QueryState.ExecutionStrategy = value;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    bool IListSource.ContainsListCollection
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the command text for the query.
    /// </summary>
    /// 
    /// <returns>
    /// A string value.
    /// </returns>
    public string CommandText
    {
      get
      {
        string commandText;
        if (!this._state.TryGetCommandText(out commandText))
          return string.Empty;
        else
          return commandText;
      }
    }

    /// <summary>
    /// Gets the object context associated with this object query.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.ObjectContext"/> associated with this
    ///             <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1"/>
    ///             instance.
    /// 
    /// </returns>
    public ObjectContext Context
    {
      get
      {
        return this._state.ObjectContext;
      }
    }

    /// <summary>
    /// Gets or sets how objects returned from a query are added to the object context.
    /// </summary>
    /// 
    /// <returns>
    /// The query <see cref="T:System.Data.Entity.Core.Objects.MergeOption"/>.
    /// 
    /// </returns>
    public MergeOption MergeOption
    {
      get
      {
        return this._state.EffectiveMergeOption;
      }
      set
      {
        EntityUtil.CheckArgumentMergeOption(value);
        this._state.UserSpecifiedMergeOption = new MergeOption?(value);
      }
    }

    /// <summary>
    /// Whether the query is streaming or buffering
    /// 
    /// </summary>
    public bool Streaming
    {
      get
      {
        return this._state.EffectiveStreamingBehaviour;
      }
      set
      {
        this._state.UserSpecifiedStreamingBehaviour = new bool?(value);
      }
    }

    /// <summary>
    /// Gets the parameter collection for this object query.
    /// </summary>
    /// 
    /// <returns>
    /// The parameter collection for this <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1"/>.
    /// 
    /// </returns>
    public ObjectParameterCollection Parameters
    {
      get
      {
        return this._state.EnsureParameters();
      }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the query plan should be cached.
    /// </summary>
    /// 
    /// <returns>
    /// A value that indicates whether the query plan should be cached.
    /// </returns>
    public bool EnablePlanCaching
    {
      get
      {
        return this._state.PlanCachingEnabled;
      }
      set
      {
        this._state.PlanCachingEnabled = value;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    Type IQueryable.ElementType
    {
      get
      {
        return this._state.ElementType;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    Expression IQueryable.Expression
    {
      get
      {
        return this.GetExpression();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IQueryProvider IQueryable.Provider
    {
      get
      {
        return (IQueryProvider) this.ObjectQueryProvider;
      }
    }

    internal ObjectQuery(ObjectQueryState queryState)
    {
      this._state = queryState;
    }

    internal ObjectQuery()
    {
    }

    /// <summary>
    /// Returns the commands to execute against the data source.
    /// </summary>
    /// 
    /// <returns>
    /// A string that represents the commands that the query executes against the data source.
    /// </returns>
    [Browsable(false)]
    public string ToTraceString()
    {
      return this._state.GetExecutionPlan(new MergeOption?()).ToTraceString();
    }

    /// <summary>
    /// Returns information about the result type of the query.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.TypeUsage"/> value that contains information about the result type of the query.
    /// 
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public TypeUsage GetResultType()
    {
      if (this._resultType == null)
      {
        TypeUsage resultType = this._state.ResultType;
        TypeUsage elementType;
        if (!TypeHelpers.TryGetCollectionElementType(resultType, out elementType))
          elementType = resultType;
        TypeUsage ospaceTypeUsage = this._state.ObjectContext.Perspective.MetadataWorkspace.GetOSpaceTypeUsage(elementType);
        if (ospaceTypeUsage == null)
          throw new InvalidOperationException(Strings.ObjectQuery_UnableToMapResultType);
        this._resultType = ospaceTypeUsage;
      }
      return this._resultType;
    }

    /// <summary>
    /// Executes the untyped object query with the specified merge option.
    /// </summary>
    /// <param name="mergeOption">The <see cref="T:System.Data.Entity.Core.Objects.MergeOption"/> to use when executing the query.
    ///             The default is <see cref="F:System.Data.Entity.Core.Objects.MergeOption.AppendOnly"/>.
    ///             </param>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1"/> that contains a collection of entity objects returned by the query.
    /// 
    /// </returns>
    public ObjectResult Execute(MergeOption mergeOption)
    {
      EntityUtil.CheckArgumentMergeOption(mergeOption);
      return this.ExecuteInternal(mergeOption);
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IList IListSource.GetList()
    {
      return this.GetIListSourceListInternal();
    }

    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumeratorInternal();
    }

    internal abstract Expression GetExpression();

    internal abstract IEnumerator GetEnumeratorInternal();

    internal abstract IList GetIListSourceListInternal();

    internal abstract ObjectResult ExecuteInternal(MergeOption mergeOption);
  }
}

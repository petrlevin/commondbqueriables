// Type: System.Data.Entity.Core.Objects.ObjectResult`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: D:\VSProjects\CommonDbSets\packages\EntityFramework.6.1.0\lib\net40\EntityFramework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// This class represents the result of the <see cref="M:System.Data.Entity.Core.Objects.ObjectQuery`1.Execute(System.Data.Entity.Core.Objects.MergeOption)"/> method.
  /// 
  /// </summary>
  /// <typeparam name="T">The type of the result.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public class ObjectResult<T> : ObjectResult, IEnumerable<T>, IEnumerable
  {
    private Shaper<T> _shaper;
    private DbDataReader _reader;
    private readonly EntitySet _singleEntitySet;
    private readonly TypeUsage _resultItemType;
    private readonly bool _readerOwned;
    private readonly bool _shouldReleaseConnection;
    private IBindingList _cachedBindingList;
    private NextResultGenerator _nextResultGenerator;
    private Action<object, EventArgs> _onReaderDispose;

    /// <summary>
    /// Gets the type of the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1"/>.
    /// 
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Type"/> that is the type of the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1"/>.
    /// 
    /// </returns>
    public override Type ElementType
    {
      get
      {
        return typeof (T);
      }
    }

    internal ObjectResult(Shaper<T> shaper, EntitySet singleEntitySet, TypeUsage resultItemType)
      : this(shaper, singleEntitySet, resultItemType, true, true)
    {
    }

    internal ObjectResult(Shaper<T> shaper, EntitySet singleEntitySet, TypeUsage resultItemType, bool readerOwned, bool shouldReleaseConnection)
      : this(shaper, singleEntitySet, resultItemType, readerOwned, shouldReleaseConnection, (NextResultGenerator) null, (Action<object, EventArgs>) null)
    {
    }

    internal ObjectResult(Shaper<T> shaper, EntitySet singleEntitySet, TypeUsage resultItemType, bool readerOwned, bool shouldReleaseConnection, NextResultGenerator nextResultGenerator, Action<object, EventArgs> onReaderDispose)
    {
      this._shaper = shaper;
      this._reader = this._shaper.Reader;
      this._singleEntitySet = singleEntitySet;
      this._resultItemType = resultItemType;
      this._readerOwned = readerOwned;
      this._shouldReleaseConnection = shouldReleaseConnection;
      this._nextResultGenerator = nextResultGenerator;
      this._onReaderDispose = onReaderDispose;
    }

    private void EnsureCanEnumerateResults()
    {
      if (this._shaper == null)
        throw new InvalidOperationException(Strings.Materializer_CannotReEnumerateQueryResults);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the query results.
    /// </summary>
    /// 
    /// <returns>
    /// An enumerator that iterates through the query results.
    /// </returns>
    public IEnumerator<T> GetEnumerator()
    {
      return (IEnumerator<T>) this.GetDbEnumerator();
    }

    internal virtual IDbEnumerator<T> GetDbEnumerator()
    {
      this.EnsureCanEnumerateResults();
      Shaper<T> shaper = this._shaper;
      this._shaper = (Shaper<T>) null;
      return shaper.GetEnumerator();
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="T:System.Data.Entity.Core.Objects.ObjectResult`1"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      DbDataReader dbDataReader = this._reader;
      this._reader = (DbDataReader) null;
      this._nextResultGenerator = (NextResultGenerator) null;
      if (dbDataReader != null && this._readerOwned)
      {
        dbDataReader.Dispose();
        if (this._onReaderDispose != null)
        {
          this._onReaderDispose((object) this, new EventArgs());
          this._onReaderDispose = (Action<object, EventArgs>) null;
        }
      }
      if (this._shaper == null)
        return;
      if (this._shaper.Context != null && this._readerOwned && this._shouldReleaseConnection)
        this._shaper.Context.ReleaseConnection();
      this._shaper = (Shaper<T>) null;
    }

    internal override IEnumerator GetEnumeratorInternal()
    {
      return (IEnumerator) this.GetDbEnumerator();
    }

    internal override IList GetIListSourceListInternal()
    {
      if (this._cachedBindingList == null)
      {
        this.EnsureCanEnumerateResults();
        this._cachedBindingList = ObjectViewFactory.CreateViewForQuery<T>(this._resultItemType, (IEnumerable<T>) this, this._shaper.Context, this._shaper.MergeOption == MergeOption.NoTracking, this._singleEntitySet);
      }
      return (IList) this._cachedBindingList;
    }

    internal override ObjectResult<TElement> GetNextResultInternal<TElement>()
    {
      if (this._nextResultGenerator == null)
        return (ObjectResult<TElement>) null;
      else
        return this._nextResultGenerator.GetNextResult<TElement>(this._reader);
    }
  }
}

// Type: System.Data.Entity.Core.Common.Internal.Materialization.Shaper`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: D:\VSProjects\CommonDbSets\packages\EntityFramework.6.1.0\lib\net40\EntityFramework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal class Shaper<T> : Shaper
  {
    private readonly bool _isObjectQuery;
    private bool _isActive;
    private IDbEnumerator<T> _rootEnumerator;
    private readonly bool _readerOwned;
    private EventHandler OnDone;
    internal readonly Coordinator<T> RootCoordinator;

    internal bool DataWaiting { get; set; }

    internal IDbEnumerator<T> RootEnumerator
    {
      get
      {
        if (this._rootEnumerator == null)
        {
          this.InitializeRecordStates(this.RootCoordinator.CoordinatorFactory);
          this._rootEnumerator = this.GetEnumerator();
        }
        return this._rootEnumerator;
      }
    }

    internal event EventHandler OnDone
    {
      add
      {
        EventHandler eventHandler = this.OnDone;
        EventHandler comparand;
        do
        {
          comparand = eventHandler;
          eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.OnDone, comparand + value, comparand);
        }
        while (eventHandler != comparand);
      }
      remove
      {
        EventHandler eventHandler = this.OnDone;
        EventHandler comparand;
        do
        {
          comparand = eventHandler;
          eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.OnDone, comparand - value, comparand);
        }
        while (eventHandler != comparand);
      }
    }

    internal Shaper(DbDataReader reader, ObjectContext context, MetadataWorkspace workspace, MergeOption mergeOption, int stateCount, CoordinatorFactory<T> rootCoordinatorFactory, bool readerOwned, bool streaming)
      : base(reader, context, workspace, mergeOption, stateCount, streaming)
    {
      this.RootCoordinator = (Coordinator<T>) rootCoordinatorFactory.CreateCoordinator((Coordinator) null, (Coordinator) null);
      this._isObjectQuery = !(typeof (T) == typeof (RecordState));
      this._isActive = true;
      this.RootCoordinator.Initialize((Shaper) this);
      this._readerOwned = readerOwned;
    }

    private void InitializeRecordStates(CoordinatorFactory coordinatorFactory)
    {
      foreach (RecordStateFactory recordStateFactory in coordinatorFactory.RecordStateFactories)
        this.State[recordStateFactory.StateSlotNumber] = (object) recordStateFactory.Create(coordinatorFactory);
      foreach (CoordinatorFactory coordinatorFactory1 in coordinatorFactory.NestedCoordinators)
        this.InitializeRecordStates(coordinatorFactory1);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public virtual IDbEnumerator<T> GetEnumerator()
    {
      if (this.RootCoordinator.CoordinatorFactory.IsSimple)
        return (IDbEnumerator<T>) new Shaper<T>.SimpleEnumerator(this);
      Shaper<T>.RowNestedResultEnumerator rowEnumerator = new Shaper<T>.RowNestedResultEnumerator(this);
      if (this._isObjectQuery)
        return (IDbEnumerator<T>) new Shaper<T>.ObjectQueryNestedEnumerator(rowEnumerator);
      else
        return (IDbEnumerator<T>) new Shaper<T>.RecordStateEnumerator(rowEnumerator);
    }

    private void Finally()
    {
      if (!this._isActive)
        return;
      this._isActive = false;
      if (this._readerOwned)
      {
        if (this._isObjectQuery)
          this.Reader.Dispose();
        if (this.Context != null && this.Streaming)
          this.Context.ReleaseConnection();
      }
      if (this.OnDone == null)
        return;
      this.OnDone((object) this, new EventArgs());
    }

    private bool StoreRead()
    {
      try
      {
        return this.Reader.Read();
      }
      catch (Exception ex)
      {
        this.HandleReaderException(ex);
        throw;
      }
    }

    private void HandleReaderException(Exception e)
    {
      if (!ExceptionExtensions.IsCatchableEntityExceptionType(e))
        return;
      if (this.Reader.IsClosed)
        throw new EntityCommandExecutionException(Strings.ADP_DataReaderClosed((object) "Read"), e);
      else
        throw new EntityCommandExecutionException(Strings.EntityClient_StoreReaderFailed, e);
    }

    private void StartMaterializingElement()
    {
      if (this.Context == null)
        return;
      this.Context.InMaterialization = true;
      this.InitializeForOnMaterialize();
    }

    private void StopMaterializingElement()
    {
      if (this.Context == null)
        return;
      this.Context.InMaterialization = false;
      this.RaiseMaterializedEvents();
    }

    private class SimpleEnumerator : IDbEnumerator<T>, IEnumerator<T>, IDisposable, IEnumerator
    {
      private readonly Shaper<T> _shaper;

      public T Current
      {
        get
        {
          return this._shaper.RootCoordinator.Current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this._shaper.RootCoordinator.Current;
        }
      }

      internal SimpleEnumerator(Shaper<T> shaper)
      {
        this._shaper = shaper;
      }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
        this._shaper.RootCoordinator.SetCurrentToDefault();
        this._shaper.Finally();
      }

      public bool MoveNext()
      {
        if (!this._shaper._isActive)
          return false;
        if (this._shaper.StoreRead())
        {
          try
          {
            this._shaper.StartMaterializingElement();
            this._shaper.RootCoordinator.ReadNextElement((Shaper) this._shaper);
          }
          finally
          {
            this._shaper.StopMaterializingElement();
          }
          return true;
        }
        else
        {
          this.Dispose();
          return false;
        }
      }

      public void Reset()
      {
        throw new NotSupportedException();
      }
    }

    private class RowNestedResultEnumerator : IDbEnumerator<Coordinator[]>, IEnumerator<Coordinator[]>, IDisposable, IEnumerator
    {
      private readonly Shaper<T> _shaper;
      private readonly Coordinator[] _current;

      public Coordinator[] Current
      {
        get
        {
          return this._current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this._current;
        }
      }

      internal Coordinator<T> RootCoordinator
      {
        get
        {
          return this._shaper.RootCoordinator;
        }
      }

      internal RowNestedResultEnumerator(Shaper<T> shaper)
      {
        this._shaper = shaper;
        this._current = new Coordinator[this._shaper.RootCoordinator.MaxDistanceToLeaf() + 1];
      }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
        this._shaper.Finally();
      }

      public bool MoveNext()
      {
        try
        {
          this._shaper.StartMaterializingElement();
          if (!this._shaper.StoreRead())
          {
            this.RootCoordinator.ResetCollection((Shaper) this._shaper);
            return false;
          }
          else
            this.MaterializeRow();
        }
        finally
        {
          this._shaper.StopMaterializingElement();
        }
        return true;
      }

      private void MaterializeRow()
      {
        Coordinator coordinator = (Coordinator) this._shaper.RootCoordinator;
        int index = 0;
        bool flag = false;
        for (; index < this._current.Length; ++index)
        {
          while (coordinator != null && !coordinator.CoordinatorFactory.HasData((Shaper) this._shaper))
            coordinator = coordinator.Next;
          if (coordinator != null)
          {
            if (coordinator.HasNextElement((Shaper) this._shaper))
            {
              if (!flag && coordinator.Child != null)
                coordinator.Child.ResetCollection((Shaper) this._shaper);
              flag = true;
              coordinator.ReadNextElement((Shaper) this._shaper);
              this._current[index] = coordinator;
            }
            else
              this._current[index] = (Coordinator) null;
            coordinator = coordinator.Child;
          }
          else
            break;
        }
        for (; index < this._current.Length; ++index)
          this._current[index] = (Coordinator) null;
      }

      public void Reset()
      {
        throw new NotSupportedException();
      }
    }

    private class ObjectQueryNestedEnumerator : IDbEnumerator<T>, IEnumerator<T>, IDisposable, IEnumerator
    {
      private readonly Shaper<T>.RowNestedResultEnumerator _rowEnumerator;
      private T _previousElement;
      private Shaper<T>.ObjectQueryNestedEnumerator.State _state;

      public T Current
      {
        get
        {
          return this._previousElement;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      internal ObjectQueryNestedEnumerator(Shaper<T>.RowNestedResultEnumerator rowEnumerator)
      {
        this._rowEnumerator = rowEnumerator;
        this._previousElement = default (T);
        this._state = (Shaper<T>.ObjectQueryNestedEnumerator.State) 0;
      }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
        this._rowEnumerator.Dispose();
      }

      public bool MoveNext()
      {
        switch ((int) this._state)
        {
          case 0:
            if (this.TryReadToNextElement())
            {
              this.ReadElement();
              break;
            }
            else
            {
              this._state = (Shaper<T>.ObjectQueryNestedEnumerator.State) 3;
              break;
            }
          case 1:
            this.ReadElement();
            break;
          case 2:
            this._state = (Shaper<T>.ObjectQueryNestedEnumerator.State) 3;
            break;
        }
        bool flag;
        if (this._state == 3)
        {
          this._previousElement = default (T);
          flag = false;
        }
        else
          flag = true;
        return flag;
      }

      private void ReadElement()
      {
        this._previousElement = this._rowEnumerator.RootCoordinator.Current;
        if (this.TryReadToNextElement())
          this._state = (Shaper<T>.ObjectQueryNestedEnumerator.State) 1;
        else
          this._state = (Shaper<T>.ObjectQueryNestedEnumerator.State) 2;
      }

      private bool TryReadToNextElement()
      {
        while (this._rowEnumerator.MoveNext())
        {
          if (this._rowEnumerator.Current[0] != null)
            return true;
        }
        return false;
      }

      public void Reset()
      {
        this._rowEnumerator.Reset();
      }

      private enum State
      {
        Start,
        Reading,
        NoRowsLastElementPending,
        NoRows,
      }
    }

    private class RecordStateEnumerator : IDbEnumerator<RecordState>, IEnumerator<RecordState>, IDisposable, IEnumerator
    {
      private readonly Shaper<T>.RowNestedResultEnumerator _rowEnumerator;
      private RecordState _current;
      private int _depth;
      private bool _readerConsumed;

      public RecordState Current
      {
        get
        {
          return this._current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this._current;
        }
      }

      internal RecordStateEnumerator(Shaper<T>.RowNestedResultEnumerator rowEnumerator)
      {
        this._rowEnumerator = rowEnumerator;
        this._current = (RecordState) null;
        this._depth = -1;
        this._readerConsumed = false;
      }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
        this._rowEnumerator.Dispose();
      }

      public bool MoveNext()
      {
        if (!this._readerConsumed)
        {
          Coordinator coordinator;
          while (true)
          {
            if (-1 == this._depth || this._rowEnumerator.Current.Length == this._depth)
            {
              if (this._rowEnumerator.MoveNext())
                this._depth = 0;
              else
                break;
            }
            coordinator = this._rowEnumerator.Current[this._depth];
            if (coordinator == null)
              ++this._depth;
            else
              goto label_6;
          }
          this._current = (RecordState) null;
          this._readerConsumed = true;
          goto label_8;
label_6:
          this._current = ((Coordinator<RecordState>) coordinator).Current;
          ++this._depth;
        }
label_8:
        return !this._readerConsumed;
      }

      public void Reset()
      {
        this._rowEnumerator.Reset();
      }
    }
  }
}

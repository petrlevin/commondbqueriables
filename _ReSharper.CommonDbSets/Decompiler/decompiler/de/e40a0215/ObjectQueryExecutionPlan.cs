// Type: System.Data.Entity.Core.Objects.Internal.ObjectQueryExecutionPlan
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: D:\VSProjects\CommonDbSets\packages\EntityFramework.6.1.0\lib\net40\EntityFramework.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.EntityClient.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class ObjectQueryExecutionPlan
  {
    internal readonly DbCommandDefinition CommandDefinition;
    internal readonly bool Streaming;
    internal readonly ShaperFactory ResultShaperFactory;
    internal readonly TypeUsage ResultType;
    internal readonly MergeOption MergeOption;
    internal readonly IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>> CompiledQueryParameters;
    private readonly EntitySet _singleEntitySet;

    public ObjectQueryExecutionPlan(DbCommandDefinition commandDefinition, ShaperFactory resultShaperFactory, TypeUsage resultType, MergeOption mergeOption, bool streaming, EntitySet singleEntitySet, IEnumerable<Tuple<ObjectParameter, QueryParameterExpression>> compiledQueryParameters)
    {
      this.CommandDefinition = commandDefinition;
      this.ResultShaperFactory = resultShaperFactory;
      this.ResultType = resultType;
      this.MergeOption = mergeOption;
      this.Streaming = streaming;
      this._singleEntitySet = singleEntitySet;
      this.CompiledQueryParameters = compiledQueryParameters;
    }

    internal string ToTraceString()
    {
      EntityCommandDefinition commandDefinition = this.CommandDefinition as EntityCommandDefinition;
      if (commandDefinition == null)
        return string.Empty;
      else
        return commandDefinition.ToTraceString();
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Buffer disposed by the returned ObjectResult")]
    internal virtual ObjectResult<TResultType> Execute<TResultType>(ObjectContext context, ObjectParameterCollection parameterValues)
    {
      DbDataReader reader = (DbDataReader) null;
      BufferedDataReader bufferedDataReader = (BufferedDataReader) null;
      try
      {
        using (EntityCommand entityCommand = this.PrepareEntityCommand(context, parameterValues))
          reader = entityCommand.GetCommandDefinition().ExecuteStoreCommands(entityCommand, this.Streaming ? CommandBehavior.Default : CommandBehavior.SequentialAccess);
        ShaperFactory<TResultType> shaperFactory = (ShaperFactory<TResultType>) this.ResultShaperFactory;
        Shaper<TResultType> shaper;
        if (this.Streaming)
        {
          shaper = shaperFactory.Create(reader, context, context.MetadataWorkspace, this.MergeOption, true, this.Streaming);
        }
        else
        {
          StoreItemCollection storeItemCollection = (StoreItemCollection) context.MetadataWorkspace.GetItemCollection(DataSpace.SSpace);
          DbProviderServices service = DbDependencyResolverExtensions.GetService<DbProviderServices>(DbConfiguration.DependencyResolver, (object) storeItemCollection.ProviderInvariantName);
          bufferedDataReader = new BufferedDataReader(reader);
          bufferedDataReader.Initialize(storeItemCollection.ProviderManifestToken, service, shaperFactory.ColumnTypes, shaperFactory.NullableColumns);
          shaper = shaperFactory.Create((DbDataReader) bufferedDataReader, context, context.MetadataWorkspace, this.MergeOption, true, this.Streaming);
        }
        TypeUsage resultItemType = this.ResultType.EdmType.BuiltInTypeKind != BuiltInTypeKind.CollectionType ? this.ResultType : ((CollectionType) this.ResultType.EdmType).TypeUsage;
        return new ObjectResult<TResultType>(shaper, this._singleEntitySet, resultItemType);
      }
      catch (Exception ex)
      {
        if (this.Streaming && reader != null)
          reader.Dispose();
        if (!this.Streaming && bufferedDataReader != null)
          bufferedDataReader.Dispose();
        throw;
      }
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by caller")]
    private EntityCommand PrepareEntityCommand(ObjectContext context, ObjectParameterCollection parameterValues)
    {
      EntityCommandDefinition entityCommandDefinition = (EntityCommandDefinition) this.CommandDefinition;
      EntityConnection connection = (EntityConnection) context.Connection;
      EntityCommand entityCommand = new EntityCommand(connection, entityCommandDefinition, context.InterceptionContext, (EntityCommand.EntityDataReaderFactory) null);
      if (context.CommandTimeout.HasValue)
        entityCommand.CommandTimeout = context.CommandTimeout.Value;
      if (parameterValues != null)
      {
        foreach (ObjectParameter objectParameter in parameterValues)
        {
          int index = ((DbParameterCollection) entityCommand.Parameters).IndexOf(objectParameter.Name);
          if (index != -1)
            entityCommand.Parameters[index].Value = objectParameter.Value ?? (object) DBNull.Value;
        }
      }
      if (connection.CurrentTransaction != null)
        entityCommand.Transaction = connection.CurrentTransaction;
      return entityCommand;
    }
  }
}

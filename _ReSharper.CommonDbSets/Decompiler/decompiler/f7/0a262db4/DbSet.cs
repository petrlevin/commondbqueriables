// Type: System.Data.Entity.DbSet`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: D:\VSProjects\CommonDbSets\packages\EntityFramework.6.1.0\lib\net40\EntityFramework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal;
using System.Data.Entity.Internal.Linq;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity
{
  /// <summary>
  /// A DbSet represents the collection of all entities in the context, or that can be queried from the
  ///             database, of a given type.  DbSet objects are created from a DbContext using the DbContext.Set method.
  /// 
  /// </summary>
  /// 
  /// <remarks>
  /// Note that DbSet does not support MEST (Multiple Entity Sets per Type) meaning that there is always a
  ///             one-to-one correlation between a type and a set.
  /// 
  /// </remarks>
  /// <typeparam name="TEntity">The type that defines the set. </typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Name is intentional")]
  public class DbSet<TEntity> : DbQuery<TEntity>, IDbSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, IQueryable, IEnumerable, IInternalSetAdapter where TEntity : class
  {
    private readonly InternalSet<TEntity> _internalSet;

    /// <inheritdoc/>
    public virtual ObservableCollection<TEntity> Local
    {
      get
      {
        return this.GetInternalSetWithCheck("Local").Local;
      }
    }

    IInternalSet IInternalSetAdapter.InternalSet
    {
      get
      {
        return (IInternalSet) this._internalSet;
      }
    }

    internal DbSet(InternalSet<TEntity> internalSet)
      : base((IInternalQuery<TEntity>) internalSet)
    {
      this._internalSet = internalSet;
    }

    /// <summary>
    /// Creates an instance of a <see cref="T:System.Data.Entity.DbSet`1"/> when called from the constructor of a derived
    ///             type that will be used as a test double for DbSets. Methods and properties that will be used by the
    ///             test double must be implemented by the test double except AsNoTracking, AsStreaming, an Include where
    ///             the default implementation is a no-op.
    /// 
    /// </summary>
    protected DbSet()
      : this((InternalSet<TEntity>) null)
    {
    }

    /// <summary>
    /// Returns the equivalent non-generic <see cref="T:System.Data.Entity.DbSet"/> object.
    /// 
    /// </summary>
    /// <param name="entry">The generic set object.</param>
    /// <returns>
    /// The non-generic set object.
    /// </returns>
    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Intentionally just implicit to reduce API clutter.")]
    public static implicit operator DbSet(DbSet<TEntity> entry)
    {
      Check.NotNull<DbSet<TEntity>>(entry, "entry");
      if (entry._internalSet == null)
        throw new NotSupportedException(Strings.TestDoublesCannotBeConverted);
      else
        return (DbSet) entry._internalSet.InternalContext.Set(entry._internalSet.ElementType);
    }

    /// <summary>
    /// Finds an entity with the given primary key values.
    ///             If an entity with the given primary key values exists in the context, then it is
    ///             returned immediately without making a request to the store.  Otherwise, a request
    ///             is made to the store for an entity with the given primary key values and this entity,
    ///             if found, is attached to the context and returned.  If no entity is found in the
    ///             context or the store, then null is returned.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// The ordering of composite key values is as defined in the EDM, which is in turn as defined in
    ///             the designer, by the Code First fluent API, or by the DataMember attribute.
    /// 
    /// </remarks>
    /// <param name="keyValues">The values of the primary key for the entity to be found. </param>
    /// <returns>
    /// The entity found, or null.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if multiple entities exist in the context with the primary key values given.</exception><exception cref="T:System.InvalidOperationException">Thrown if the type of entity is not part of the data model for this context.</exception><exception cref="T:System.InvalidOperationException">Thrown if the types of the key values do not match the types of the key values for the entity type to be found.</exception><exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
    public virtual TEntity Find(params object[] keyValues)
    {
      return this.GetInternalSetWithCheck("Find").Find(keyValues);
    }

    /// <inheritdoc/>
    public virtual TEntity Attach(TEntity entity)
    {
      Check.NotNull<TEntity>(entity, "entity");
      this.GetInternalSetWithCheck("Attach").Attach((object) entity);
      return entity;
    }

    /// <inheritdoc/>
    public virtual TEntity Add(TEntity entity)
    {
      Check.NotNull<TEntity>(entity, "entity");
      this.GetInternalSetWithCheck("Add").Add((object) entity);
      return entity;
    }

    /// <summary>
    /// Adds the given collection of entities into context underlying the set with each entity being put into
    ///             the Added state such that it will be inserted into the database when SaveChanges is called.
    /// 
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <returns>
    /// The collection of entities.
    /// 
    /// </returns>
    /// 
    /// <remarks>
    /// Note that if <see cref="P:System.Data.Entity.Infrastructure.DbContextConfiguration.AutoDetectChangesEnabled"/> is set to true (which is
    ///             the default), then DetectChanges will be called once before adding any entities and will not be called
    ///             again. This means that in some situations AddRange may perform significantly better than calling
    ///             Add multiple times would do.
    ///             Note that entities that are already in the context in some other state will have their state set to
    ///             Added.  AddRange is a no-op for entities that are already in the context in the Added state.
    /// 
    /// </remarks>
    public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
    {
      Check.NotNull<IEnumerable<TEntity>>(entities, "entities");
      this.GetInternalSetWithCheck("AddRange").AddRange((IEnumerable) entities);
      return entities;
    }

    /// <inheritdoc/>
    public virtual TEntity Remove(TEntity entity)
    {
      Check.NotNull<TEntity>(entity, "entity");
      this.GetInternalSetWithCheck("Remove").Remove((object) entity);
      return entity;
    }

    /// <summary>
    /// Removes the given collection of entities from the context underlying the set with each entity being put into
    ///             the Deleted state such that it will be deleted from the database when SaveChanges is called.
    /// 
    /// </summary>
    /// <param name="entities">The collection of entities to delete.</param>
    /// <returns>
    /// The collection of entities.
    /// 
    /// </returns>
    /// 
    /// <remarks>
    /// Note that if <see cref="P:System.Data.Entity.Infrastructure.DbContextConfiguration.AutoDetectChangesEnabled"/> is set to true (which is
    ///             the default), then DetectChanges will be called once before delete any entities and will not be called
    ///             again. This means that in some situations RemoveRange may perform significantly better than calling
    ///             Remove multiple times would do.
    ///             Note that if any entity exists in the context in the Added state, then this method
    ///             will cause it to be detached from the context.  This is because an Added entity is assumed not to
    ///             exist in the database such that trying to delete it does not make sense.
    /// 
    /// </remarks>
    public virtual IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities)
    {
      Check.NotNull<IEnumerable<TEntity>>(entities, "entities");
      this.GetInternalSetWithCheck("RemoveRange").RemoveRange((IEnumerable) entities);
      return entities;
    }

    /// <inheritdoc/>
    public virtual TEntity Create()
    {
      return this.GetInternalSetWithCheck("Create").Create();
    }

    /// <inheritdoc/>
    public virtual TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
    {
      return (TDerivedEntity) (object) this.GetInternalSetWithCheck("Create").Create(typeof (TDerivedEntity));
    }

    private InternalSet<TEntity> GetInternalSetWithCheck(string memberName)
    {
      if (this._internalSet == null)
        throw new NotImplementedException(Strings.TestDoubleNotImplemented((object) memberName, (object) this.GetType().Name, (object) typeof (DbSet<>).Name));
      else
        return this._internalSet;
    }

    /// <summary>
    /// Creates a raw SQL query that will return entities in this set.  By default, the
    ///              entities returned are tracked by the context; this can be changed by calling
    ///              AsNoTracking on the <see cref="T:System.Data.Entity.Infrastructure.DbSqlQuery`1"/> returned.
    ///              Note that the entities returned are always of the type for this set and never of
    ///              a derived type.  If the table or tables queried may contain data for other entity
    ///              types, then the SQL query must be written appropriately to ensure that only entities of
    ///              the correct type are returned.
    /// 
    ///              As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
    ///              context.Blogs.SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor);
    ///              Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named parameters in the SQL query string.
    ///              context.Blogs.SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
    /// 
    /// </summary>
    /// <param name="sql">The SQL query string. </param><param name="parameters">The parameters to apply to the SQL query string. </param>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Infrastructure.DbSqlQuery`1"/> object that will execute the query when it is enumerated.
    /// 
    /// </returns>
    public virtual DbSqlQuery<TEntity> SqlQuery(string sql, params object[] parameters)
    {
      Check.NotEmpty(sql, "sql");
      Check.NotNull<object[]>(parameters, "parameters");
      return new DbSqlQuery<TEntity>(this._internalSet != null ? (InternalSqlQuery) new InternalSqlSetQuery((IInternalSet) this._internalSet, sql, false, parameters) : (InternalSqlQuery) null);
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
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}

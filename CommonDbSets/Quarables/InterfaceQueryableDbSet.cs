using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Reflection;
using CommonDbSets.Contracts;

namespace CommonDbSets.Quarables
{
    public class InterfaceQueryableDbSet<TInterface> : InterfaceQueryable<TInterface>, IQueryableDbSet<TInterface>
    {

    


        private static IQueryable Cast(DbSet dbSet, Type entityType)
        {
            try
            {
              return (IQueryable)
                typeof(DbSet).GetMethod("Cast")
                              .MakeGenericMethod(new Type[] { entityType })
                              .Invoke(dbSet, new object[] { });
               
            }
            catch(TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            
        }

        private readonly DbSet _inner;
        internal InterfaceQueryableDbSet(DbSet dbSet, Type entityType)
            : base(Cast(dbSet, entityType))
        {
            _inner = dbSet;
        }

        public TInterface Add(TInterface entity)
        {
            return (TInterface)_inner.Add(entity);
        }

        public TInterface Attach(TInterface entity)
        {
            return (TInterface)_inner.Attach(entity);
        }

        public TInterface Create()
        {
            return (TInterface)_inner.Create();
        }

        public TInterface Remove(TInterface entity)
        {
            return (TInterface)_inner.Remove(entity);
        }


    }
}

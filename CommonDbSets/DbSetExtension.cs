using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using CommonDbSets.Contracts;
using CommonDbSets.Quarables;

namespace CommonDbSets
{
    public static  class DbSetExtension
    {
        public static IQueryableDbSet<TCommon> ToCommon<TCommon>(this DbSet dbSet,Type entityType)
        {
            
            return new InterfaceQueryableDbSet<TCommon>(dbSet,entityType);
        }

        public static IQueryableDbSet<TCommon> ToCommon<TCommon, TEntity>(this DbSet dbSet)
        {
            return ToCommon<TCommon>(dbSet, typeof(TEntity));
        }



    }
}

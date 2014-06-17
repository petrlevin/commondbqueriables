using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using CommonDbSets.Contracts;
using CommonDbSets.Quarables;

namespace CommonDbSets
{
    public static  class DbContextExtension
    {
        public static IQueryableDbSet<TCommon> Set<TCommon>(this DbContext dbContext,Type concreteEntityType)
        {

            return dbContext.Set(concreteEntityType).ToCommon<TCommon>(concreteEntityType);
        }

        public static IQueryableDbSet<TCommon> Set<TCommon>(this DbContext dbContext, Func<Type> concreteEntityTypeProvider)
        {

            return dbContext.Set<TCommon>(concreteEntityTypeProvider());
        }

        public static IQueryableDbSet<TCommon> Set<TCommon, TConcreteEntity>(this DbContext dbContext)
        {
            return Set<TCommon>(dbContext, typeof(TConcreteEntity));
        }



    }
}

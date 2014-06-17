using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CommonDbSets.Quarables
{
    public class InterfaceQueryable<TInterface> : QuerableDecorator, IQueryable<TInterface>
    {
        private ExpressionVisitor modifier;

        internal InterfaceQueryable(IQueryable inner) :base(inner)
        {
            
        }

        public new  IEnumerator<TInterface> GetEnumerator()
        {
            //if (modifier==null)
            //{
            //    modifier = new Modifier();
            //    modifier.Visit()
            //}
            return (IEnumerator<TInterface>) base.GetEnumerator();
        }

      

     

        public Type ElementType
        {
            get { return typeof (TInterface); }
        }

    }
}

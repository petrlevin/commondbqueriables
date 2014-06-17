using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CommonDbSets.Quarables
{
    public class QuerableDecorator : IQueryable
    {
        public virtual IEnumerator GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        public Expression Expression
        {
            get
            {
                return _inner.Expression;
            }
        }

        public Type ElementType
        {
            get { return _inner.ElementType; }
        }

        public virtual IQueryProvider Provider
        {
            get
            {
                return _inner.Provider;
                //return new QueryProvider(_inner.Provider);
            }
        }

        protected IQueryable _inner;

        public QuerableDecorator(IQueryable inner)
        {
            _inner = inner;
        }
    }
}

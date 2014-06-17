// Type: System.Data.Entity.Internal.Linq.IInternalQuery`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: D:\VSProjects\CommonDbSets\packages\EntityFramework.6.1.0\lib\net40\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Internal.Linq
{
  internal interface IInternalQuery<out TElement> : IInternalQuery
  {
    IInternalQuery<TElement> Include(string path);

    IInternalQuery<TElement> AsNoTracking();

    IInternalQuery<TElement> AsStreaming();

    IInternalQuery<TElement> WithExecutionStrategy(IDbExecutionStrategy executionStrategy);

    IEnumerator<TElement> GetEnumerator();
  }
}

using System;

namespace CommonDbSets.Tests.Data
{
    public interface IDocument : IHasIdentity
    {
        
        string Number { get; set; }
        DateTime Date { get; set; }
        

    
    }
}

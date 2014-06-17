using System;

namespace CommonDbSets.Tests.Data
{
    public class Content : IHasIdentity
    {
        public String Author { get; set; }
        public String Title { get; set; }
        public String Text { get; set; }
        public int Id { get; set; }
    }
    
}

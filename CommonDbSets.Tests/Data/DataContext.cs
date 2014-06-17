using System.Data.Entity;

namespace CommonDbSets.Tests.Data
{
    public class DataContext:DbContext
    {
        
        public DbSet<TextDocument> TextDocument { get; set; }
        public DbSet<ContentDocument> ContentDocument { get; set; }
        public DbSet<SelfImplementedDocument> SelfImplementedDocument { get; set; }
        public DbSet<Content> Contents { get; set; }


        public DataContext()
            : base("Name=DataContext")
        {
            
        }

    }
}

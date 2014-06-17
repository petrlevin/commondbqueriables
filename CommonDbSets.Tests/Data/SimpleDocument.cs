using System;

namespace CommonDbSets.Tests.Data
{
    abstract public class  SimpleDocument :IDocument
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
    }
}

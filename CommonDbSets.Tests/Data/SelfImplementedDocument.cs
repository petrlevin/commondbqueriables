using System;

namespace CommonDbSets.Tests.Data
{
    public class SelfImplementedDocument: IDocument
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
    }
}

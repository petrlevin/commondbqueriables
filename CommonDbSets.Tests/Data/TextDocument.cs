using System;

namespace CommonDbSets.Tests.Data
{
    public class TextDocument : SimpleDocument, IDocument<String>, ITextDocument
    {
        public string Content { get; set; }
        public string Text { get { return Content; } }
    }
}

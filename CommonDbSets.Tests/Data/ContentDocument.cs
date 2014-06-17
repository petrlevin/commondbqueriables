namespace CommonDbSets.Tests.Data
{
    public class ContentDocument : SimpleDocument, IDocument<Content>, ITextDocument
    {
        public Content Content { get; set; }
        public string Text
        {
            get { return Content.Text; }
        }
    }
}

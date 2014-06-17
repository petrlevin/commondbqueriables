namespace CommonDbSets.Tests.Data
{
    public interface IDocument<TContent> : IDocument
    {

        TContent Content { get; set; }

    }
}

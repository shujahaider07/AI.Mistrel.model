namespace AI.Mistrel.model.Service.Interface
{
    public interface IResumeParser
    {
        Task<string> ExtractTextFromFileAsync(IFormFile file);
    }
}

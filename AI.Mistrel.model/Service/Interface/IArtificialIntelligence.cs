namespace AI.Mistrel.model.Service.Interface
{
    public interface IArtificialIntelligence
    {
        Task<string> OpenAITurboModelAsync(string prompt, dynamic Data);
    }
}

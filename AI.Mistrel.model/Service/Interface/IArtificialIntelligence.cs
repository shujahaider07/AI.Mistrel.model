namespace AI.Mistrel.model.Service.Interface
{
    public interface IArtificialIntelligence
    {
        Task<string> OpenAITurboModelAsync(dynamic invoiceJson);
    }
}

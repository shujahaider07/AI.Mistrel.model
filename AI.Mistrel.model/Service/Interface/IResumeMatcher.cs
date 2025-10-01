namespace AI.Mistrel.model.Service.Interface
{
    public interface IResumeMatcher
    {
        Task<ResumeMatchResult> MatchResumeAsync(string resumeText, string jobDescription);
    }

    public class ResumeMatchResult
    {
        public int MatchPercentage { get; set; }
        public string Experience { get; set; } = string.Empty;
        public List<string> MatchedSkills { get; set; } = new();
        public List<string> MissingSkills { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();
    }
}

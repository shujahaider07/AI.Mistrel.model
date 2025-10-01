using AI.Mistrel.model.Service.Interface;
using Newtonsoft.Json;

namespace AI.Mistrel.model.Service.Implement
{
    public class ResumeMatcher : IResumeMatcher
    {
        private readonly IArtificialIntelligence _aiService;

        public ResumeMatcher(IArtificialIntelligence aiService)
        {
            _aiService = aiService;
        }

        public async Task<ResumeMatchResult> MatchResumeAsync(string resumeText, string jobDescription)
        {
            var prompt = CreateResumeMatchingPrompt();
            var data = $@"
JOB DESCRIPTION (This is what we're looking for):
{jobDescription}

RESUME TEXT (This is the candidate's background):
{resumeText}

ANALYSIS INSTRUCTIONS:
- Compare ONLY the skills and experience that are mentioned in the job description
- Ignore any skills in the resume that are not relevant to this specific job
- Focus on technical requirements, years of experience, and specific qualifications
- Be strict: if a skill in the resume doesn't match the job requirements, don't count it
";

            var aiResponse = await _aiService.OpenAITurboModelAsync(prompt, data);
            
            try
            {
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(aiResponse);
                string responseText = jsonResponse?.choices?[0]?.message?.content?.ToString();
                
                if (string.IsNullOrEmpty(responseText))
                {
                    return CreateDefaultResult();
                }

                return ParseAIResponse(responseText);
            }
            catch
            {
                return CreateDefaultResult();
            }
        }

        private string CreateResumeMatchingPrompt()
        {
            return @"You are an expert HR recruiter and resume analyzer. Your task is to analyze how well a resume matches a specific job description.

IMPORTANT GUIDELINES:
1. Only consider skills, experience, and qualifications that are DIRECTLY relevant to the job requirements
2. Ignore irrelevant skills or experience that don't match the job description
3. Focus on technical skills, years of experience, and specific qualifications mentioned in the job
4. Be strict in your evaluation - don't inflate match percentages for irrelevant matches

ANALYSIS CRITERIA:
- Technical Skills Match: Compare resume skills with job requirements
- Experience Level: Match years of experience with job expectations
- Relevant Qualifications: Check for required certifications, degrees, or specific knowledge
- Job-Specific Requirements: Look for industry-specific experience or tools mentioned in the job

Return your response in the following JSON format:
{
  ""matchPercentage"": 75,
  ""experience"": ""3+ years"",
  ""matchedSkills"": [""React"", ""Node.js"", ""MongoDB""],
  ""missingSkills"": [""AWS"", ""Docker"", ""Kubernetes""],
  ""suggestions"": [
    ""Add AWS cloud experience to strengthen your profile"",
    ""Include Docker containerization projects in your resume""
  ]
}

EVALUATION RULES:
- Match percentage should reflect ONLY relevant matches
- If resume has many skills but few match the job, percentage should be low
- Experience should be extracted from resume and compared with job requirements
- Missing skills should be important job requirements not found in resume
- Suggestions should be specific and actionable for the job role

Be honest and accurate in your assessment. A 70% match means the candidate is genuinely well-suited for 70% of the job requirements.";
        }

        private ResumeMatchResult ParseAIResponse(string responseText)
        {
            try
            {
                // Try to extract JSON from the response
                var jsonStart = responseText.IndexOf('{');
                var jsonEnd = responseText.LastIndexOf('}');
                
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonString = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    var result = JsonConvert.DeserializeObject<ResumeMatchResult>(jsonString);
                    return result ?? CreateDefaultResult();
                }
            }
            catch
            {
                // Fallback to default result
            }

            return CreateDefaultResult();
        }

        private ResumeMatchResult CreateDefaultResult()
        {
            return new ResumeMatchResult
            {
                MatchPercentage = 45, // More realistic default
                Experience = "Not specified",
                MatchedSkills = new List<string>(), // Empty by default
                MissingSkills = new List<string> { "Please check job requirements" },
                Suggestions = new List<string>
                {
                    "Ensure your resume highlights skills mentioned in the job description",
                    "Add specific examples of relevant projects and achievements"
                }
            };
        }
    }
}

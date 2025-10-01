using AI.Mistrel.model.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI.Mistrel.model.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeParser _resumeParser;
        private readonly IResumeMatcher _resumeMatcher;

        public ResumeController(IResumeParser resumeParser, IResumeMatcher resumeMatcher)
        {
            _resumeParser = resumeParser;
            _resumeMatcher = resumeMatcher;
        }

        [HttpPost("match")]
        public async Task<IActionResult> MatchResume([FromForm] ResumeMatchRequest request)
        {
            try
            {
                if (request.ResumeFile == null || request.ResumeFile.Length == 0)
                {
                    return BadRequest(new { success = false, error = "Resume file is required." });
                }

                if (string.IsNullOrWhiteSpace(request.JobDescription))
                {
                    return BadRequest(new { success = false, error = "Job description is required." });
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(request.ResumeFile.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, error = "Only PDF, DOC, and DOCX files are allowed." });
                }

                // Extract text from resume
                var resumeText = await _resumeParser.ExtractTextFromFileAsync(request.ResumeFile);
                
                if (string.IsNullOrWhiteSpace(resumeText))
                {
                    return BadRequest(new { success = false, error = "Could not extract text from the resume file." });
                }

                // Match resume with job description
                var matchResult = await _resumeMatcher.MatchResumeAsync(resumeText, request.JobDescription);

                return Ok(new { success = true, data = matchResult });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = "An error occurred while processing the resume." });
            }
        }
    }

    public class ResumeMatchRequest
    {
        public IFormFile ResumeFile { get; set; } = null!;
        public string JobDescription { get; set; } = string.Empty;
    }
}

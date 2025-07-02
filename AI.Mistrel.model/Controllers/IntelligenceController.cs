using AI.Mistrel.model.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AI.Mistrel.model.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntelligenceController : ControllerBase
    {
        private readonly IArtificialIntelligence _artificialIntelligence;

        public IntelligenceController(IArtificialIntelligence artificialIntelligence)
        {
            _artificialIntelligence = artificialIntelligence;
        }



        [HttpPost]
        public async Task<IActionResult> AnalyzePrompt([FromBody] PromptRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, error = "Request body is null." });
                }

                if (string.IsNullOrWhiteSpace(request.Prompt) || string.IsNullOrWhiteSpace(request.InvoiceData))
                {
                    return BadRequest(new { success = false, error = "Prompt or InvoiceData is missing." });
                }

                // Combine the prompt with invoiceData into one message
                string finalPrompt = $"{request.Prompt}\n\n{request.InvoiceData}";

                // Call your AI service method
                var data = JsonConvert.SerializeObject(request.InvoiceData);
                var analysisResult = await _artificialIntelligence.OpenAITurboModelAsync(finalPrompt);

                if (string.IsNullOrWhiteSpace(analysisResult))
                {
                    return StatusCode(500, new { success = false, error = "Empty response from AI model." });
                }

                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(analysisResult);
                string responseText = jsonResponse?.choices?[0]?.message?.content?.ToString();

                if (string.IsNullOrEmpty(responseText))
                {
                    return StatusCode(500, new { success = false, error = "AI model did not return a valid response." });
                }

                return Ok(new { success = true, response = responseText });
            }
            catch (JsonSerializationException ex)
            {
                return StatusCode(500, new { success = false, error = $"Serialization error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

    }


    public class PromptRequest
    {
        public string Prompt { get; set; }
        public string InvoiceData { get; set; }
    }
}

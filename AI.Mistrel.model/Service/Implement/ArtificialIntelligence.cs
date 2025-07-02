using AI.Mistrel.model.Service.Interface;
using Newtonsoft.Json;
using System.Text;

namespace AI.Mistrel.model.Service.Implement
{
    public class ArtificialIntelligence : IArtificialIntelligence
    {
        public ArtificialIntelligence()
        {

        }
        public async Task<string> OpenAITurboModelAsync(dynamic invoiceJson)
        {
            string apiKey = "2ZlLJj87ueRboSsZzLG0mMKX2jQbe62q";
            string apiUrl = "https://api.mistral.ai/v1/chat/completions";

            var requestData = new
            {
                model = "mistral-small-latest",
                messages = new object[]
    {

                      new { role = "user", content = $"::\n" + invoiceJson },
                     // new { role = "user", content = userPrompt }
    }
            };

            string jsonContent = JsonConvert.SerializeObject(requestData);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}

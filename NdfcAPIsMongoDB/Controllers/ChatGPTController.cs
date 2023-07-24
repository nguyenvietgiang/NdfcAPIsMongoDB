using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class ChatGPTController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UseChatGPT(string query)
        { 
            string outputResult = "";
            var openai = new OpenAIAPI("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");
            CompletionRequest request = new CompletionRequest();
            request.Prompt= query;
            request.Model = OpenAI_API.Models.Model.DavinciText;

            var completes = openai.Completions.CreateCompletionsAsync(request);
            foreach (var completion in completes.Result.Completions)
            {
                outputResult+= completion.Text;
            }
            return Ok(outputResult);
        }
    }
}

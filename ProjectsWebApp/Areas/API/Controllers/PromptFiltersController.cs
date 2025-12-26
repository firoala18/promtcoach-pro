// PromptFiltersController.cs   (Areas/Api/)
using Dto.PromptFilters;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class PromptFiltersController : ControllerBase
{
    private readonly IPromptFilterAiService _ai;

    public PromptFiltersController(IPromptFilterAiService ai) => _ai = ai;

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] PromptFormDto form,
                                              CancellationToken ct = default)
    {
        var payload = await _ai.GenerateAsync(form, ct);

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        var bytes = Encoding.UTF8.GetBytes(json);

        // ⇒  „prompt-filters.json“ wird direkt heruntergeladen
        return File(bytes, "application/json", "prompt-filters.json");
    }
}

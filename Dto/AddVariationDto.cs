using System.Text.Json;

namespace ProjectsWebApp.Dto
{
    public class AddVariationDto
    {
        public string Akronym { get; set; }
        public JsonElement Data { get; set; }
    }
}

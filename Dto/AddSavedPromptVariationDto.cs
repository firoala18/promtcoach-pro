using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dto
{
    public class AddSavedPromptVariationDto
    {
        public int SavedPromptId { get; set; }
        public string Akronym { get; set; } = "";
        public JsonElement Data { get; set; }
    }

}

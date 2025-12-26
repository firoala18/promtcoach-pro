using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.PromptFilters
{
    // Dto/PromptFilters/PromptFormDto.cs
    public sealed record PromptFormDto(
        string Titel,
        string Thema,
        string Ziele,
        string Beschreibung,
        string[] Schlüsselbegriffe);
}

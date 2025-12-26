using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.PromptFilters
{
    // Dto/PromptFilters/PromptFilterItemDto.cs
    public sealed class PromptFilterItemDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = "";
        public string Info { get; init; } = "";
        public string Instruction { get; init; } = "";
        public int SortOrder { get; init; }
    }
}

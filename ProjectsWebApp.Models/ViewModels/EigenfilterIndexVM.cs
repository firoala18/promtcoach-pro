using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models.ViewModels
{
    public sealed class EigenfilterIndexVM
    {
        public PromptType Type { get; init; }
        public IEnumerable<FilterCategory> Manual { get; init; } = Enumerable.Empty<FilterCategory>();
        public IEnumerable<FilterCategory> Ai { get; init; } = Enumerable.Empty<FilterCategory>();
    }
}

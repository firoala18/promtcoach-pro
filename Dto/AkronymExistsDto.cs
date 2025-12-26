using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    // Models/Dtos
    public sealed class AkronymExistsDto
    {
        public bool Exists { get; set; }
        public string? Message { get; set; }
    }

}

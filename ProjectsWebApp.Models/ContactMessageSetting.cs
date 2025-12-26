using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class ContactMessageSetting
    {
        public int Id { get; set; }

        [Required, MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
    }
}

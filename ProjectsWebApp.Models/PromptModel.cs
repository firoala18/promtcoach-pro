using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class PromptModel
    {
        public int Id { get; set; }

        [Required]
        public string Titel { get; set; }


        [Required]
        [Url]
        public string RedirectUrl { get; set; }
    }

}

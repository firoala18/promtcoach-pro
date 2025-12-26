using ProjectsWebApp.DataAccsess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsWebApp.Models
{
    public class Fachgruppen : IEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Fachgruppen")]
        public string Name { get; set; }

        [DisplayName("Display Order")]
      
        public int DisplayOrder { get; set; }
    }
}

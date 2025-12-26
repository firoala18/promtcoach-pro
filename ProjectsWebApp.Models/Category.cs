using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using ProjectsWebApp;
using ProjectsWebApp.DataAccsess.Repository.IRepository;

namespace ProjectsWebApp.Models
{
    public class Category : IEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

    
    }

}

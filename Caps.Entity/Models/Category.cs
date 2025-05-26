using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSGM.Entity.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public required string Title { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool Status { get; set; }
        
        [StringLength(50)]
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }        
        
        [StringLength(50)]
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
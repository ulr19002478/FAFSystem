using ITS_System.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FlexAppealFitness.Models
{
    public class EquipmentListEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Equipment Equipment { get; set; }

        [Required]
        public DateTime AddedOn { get; set; }
    }
}

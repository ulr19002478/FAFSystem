using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ITS_System.Models.Enums;

namespace ITS_System.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ClassId")]
        public ClassSchedule Class { get; set; }

        public int ClassId { get; set; }

        [Required]
        public IdentityUser Attendee { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public BookingStatus Status { get; set; }
    }
}

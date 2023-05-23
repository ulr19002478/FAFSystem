using FlexAppealFitness.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ITS_System.Models.Enums;

namespace ITS_System.Models
{
    public class ClassSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Class Date and Time")]
        public DateTime DateTime { get; set; }

        [Required]
        [Display(Name = "Instructor")]
        public string InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public IdentityUser Instructor { get; set; }

        [Required]
        [Display(Name = "Max Number of Bookings")]
        public int MaxNumbersOfBooking { get; set; }
      
        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        public ClassStatus Status { get; set; }

        public virtual List<WaitingListEntry> WaitingList { get; set; }
        public virtual List<Booking> Attendees { get; set; }
        public virtual List<EquipmentListEntry> EquipmentList { get; set; }

    }
}

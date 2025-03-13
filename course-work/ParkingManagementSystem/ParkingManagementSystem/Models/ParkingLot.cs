using System;
using System.ComponentModel.DataAnnotations;

namespace ParkingManager.Models
{
    public class ParkingLot
    {
        public int ParkingLotID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        public int TotalSlots { get; set; }

        [Required]
        public int AvailableSlots { get; set; }

        [Required]
        public bool IsCovered { get; set; }

        [Required]
        public DateTime LastMaintenance { get; set; }
    }
}

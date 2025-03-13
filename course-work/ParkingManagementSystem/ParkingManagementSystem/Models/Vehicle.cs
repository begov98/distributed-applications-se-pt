using System;
using System.ComponentModel.DataAnnotations;

namespace ParkingManager.Models
{
    public class Vehicle
    {
        public int VehicleID { get; set; }

        [Required]
        [MaxLength(10)]
        public string PlateNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string OwnerName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Model { get; set; }

        [Required]
        [MaxLength(20)]
        public string Type { get; set; }

        [Required]
        [MaxLength(20)]
        public string Color { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}

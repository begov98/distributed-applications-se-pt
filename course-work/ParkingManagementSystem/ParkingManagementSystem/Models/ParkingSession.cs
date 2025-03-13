using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ParkingManager.Models
{
    public class ParkingSession
    {
        [Key]
        public int SessionID { get; set; }

        [Required]
        public int ParkingLotID { get; set; }

        [Required]
        public int VehicleID { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public decimal Fee { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        public DateTime? PaymentDate { get; set; }

        // Навигационни свойства за връзка с ParkingLot и Vehicle
        [JsonIgnore]
        public virtual ParkingLot? ParkingLot { get; set; }
        [JsonIgnore]
        public virtual Vehicle? Vehicle { get; set; }
    }
}

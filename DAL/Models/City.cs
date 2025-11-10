using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    /// <summary>
    /// Represents a city with congestion tax rules
    /// </summary>
    public sealed class City
    {
        [Key]
        public int CityId { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(10)]
        public required string CountryCode { get; set; }

        /// <summary>
        /// Maximum tax amount per day in local currency
        /// </summary>
        public required decimal MaxDailyFee { get; set; }

        /// <summary>
        /// Currency code (SEK, EUR, etc.)
        /// </summary>
        [MaxLength(3)]
        public required string Currency { get; set; }

        /// <summary>
        /// Time window in minutes for single charge rule
        /// </summary>
        public required int SingleChargeWindowMinutes { get; set; }

        /// <summary>
        /// Whether the city has congestion tax
        /// </summary>
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<TaxTimeSlot> TaxTimeSlots { get; set; } = new HashSet<TaxTimeSlot>();
        public ICollection<TaxFreeDate> TaxFreeDates { get; set; } = new HashSet<TaxFreeDate>();
        public ICollection<TaxFreeVehicleType> TaxFreeVehicleTypes { get; set; } = new HashSet<TaxFreeVehicleType>();
    }
}

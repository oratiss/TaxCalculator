using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

/// <summary>
/// Defines tax amounts for specific time slots
/// </summary>
public class TaxTimeSlot
{
    [Key]
    public int TaxTimeSlotId { get; set; }

    public required int CityId { get; set; }

    /// <summary>
    /// Start time (e.g., "06:00")
    /// </summary>
    public required TimeSpan StartTime { get; set; }

    /// <summary>
    /// End time (e.g., "06:29")
    /// </summary>
    public required TimeSpan EndTime { get; set; }

    /// <summary>
    /// Tax amount for this time slot
    /// </summary>
    public required decimal TaxAmount { get; set; }

    /// <summary>
    /// Display order for UI
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Effective from date (for future changes)
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// Effective until date (for historical tracking)
    /// </summary>
    public DateTime? EffectiveUntil { get; set; }

    // Navigation property
    [ForeignKey("CityId")]
    public virtual required City City { get; set; }
}
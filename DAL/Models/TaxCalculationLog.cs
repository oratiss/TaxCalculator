using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

/// <summary>
/// Tax calculation history for audit purposes
/// </summary>
public class TaxCalculationLog
{
    [Key]
    public int LogId { get; set; }

    public required int CityId { get; set; }

    [MaxLength(50)]
    public required string VehicleType { get; set; }

    public required DateTime CalculationDate { get; set; }

    /// <summary>
    /// JSON array of pass timestamps
    /// </summary>
    [MaxLength(10000)]
    public string? PassTimestamps { get; set; }

    /// <summary>
    /// Calculated total fee
    /// </summary>
    public required decimal TotalFee { get; set; }

    /// <summary>
    /// Breakdown of charges per window
    /// </summary>
    public string? CalculationBreakdown { get; set; }

    /// <summary>
    /// When this calculation was performed
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // Navigation property
    [ForeignKey("CityId")]
    public City? City { get; set; }
}
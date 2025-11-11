using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

/// <summary>
/// Defines tax-free vehicle types for a city
/// </summary>
public class TaxFreeVehicleType
{
    [Key]
    public int TaxFreeVehicleTypeId { get; set; }

    public required int CityId { get; set; }

    /// <summary>
    /// Vehicle type name (e.g., "Motorcycle", "Bus", "Emergency")
    /// </summary>
    [MaxLength(50)]
    public required string VehicleTypeName { get; set; }

    /// <summary>
    /// Description of why this vehicle is exempt
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Is this exemption active?
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Effective from date
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// Effective until date
    /// </summary>
    public DateTime? EffectiveUntil { get; set; }

    // Navigation property
    [ForeignKey("CityId")]
    public virtual City City { get; set; }
}
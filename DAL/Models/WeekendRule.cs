using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

/// <summary>
/// Special rules for weekends
/// </summary>
public class WeekendRule
{
    [Key]
    public int WeekendRuleId { get; set; }

    public required int CityId { get; set; }

    /// <summary>
    /// Is Saturday tax-free?
    /// </summary>
    public bool IsSaturdayTaxFree { get; set; }

    /// <summary>
    /// Is Sunday tax-free?
    /// </summary>
    public bool IsSundayTaxFree { get; set; }

    /// <summary>
    /// Effective from date
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }

    // Navigation property
    [ForeignKey("CityId")]
    public virtual City City { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

/// <summary>
/// Defines tax-free dates (holidays, special days)
/// </summary>
public class TaxFreeDate
{
    [Key]
    public int TaxFreeDateId { get; set; }

    public required int CityId { get; set; }

    /// <summary>
    /// The tax-free date
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    /// Description (e.g., "Christmas Day", "National Day")
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Type of tax-free date
    /// </summary>
    public required TaxFreeDateType Type { get; set; }

    /// <summary>
    /// Is this a recurring holiday (e.g., every December 25)?
    /// </summary>
    public bool IsRecurring { get; set; }

    /// <summary>
    /// If recurring, month (1-12)
    /// </summary>
    public int? RecurringMonth { get; set; }

    /// <summary>
    /// If recurring, day (1-31)
    /// </summary>
    public int? RecurringDay { get; set; }

    // Navigation property
    [ForeignKey("CityId")]
    public virtual City City { get; set; }
}
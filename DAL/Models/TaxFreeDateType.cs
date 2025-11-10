namespace DAL.Models;

/// <summary>
/// Type of tax-free date
/// </summary>
public enum TaxFreeDateType
{
    PublicHoliday = 1,
    DayBeforeHoliday = 2,
    SpecialDay = 3,
    TaxFreeMonth = 4
}
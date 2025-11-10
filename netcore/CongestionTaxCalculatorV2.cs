using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.DbContexts;

namespace congestion.calculator
{
    public class CongestionTaxCalculatorV2(CongestionTaxDbContext context)
    {
        private readonly CongestionTaxDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Calculate the total toll fee for one or multiple days in a specific city
        /// </summary>
        public decimal GetTax(string cityName, Vehicle vehicle, DateTime[] dates)
        {
            // Input validation
            if (dates is null || dates.Length == 0)
                return 0;

            if (string.IsNullOrWhiteSpace(cityName))
                throw new ArgumentException("City name is required", nameof(cityName));

            // Get city configuration
            var city = _context.Cities
                .Include(c => c.TaxTimeSlots)
                .Include(c => c.TaxFreeDates)
                .Include(c => c.TaxFreeVehicleTypes)
                .FirstOrDefault(c => c.Name == cityName && c.IsActive);

            if (city is null)
                throw new ArgumentException($"City '{cityName}' not found or not active", nameof(cityName));

            // Sort dates to ensure chronological order
            Array.Sort(dates);

            // Get weekend rule
            var weekendRule = _context.WeekendRules
                .FirstOrDefault(w => w.CityId == city.CityId);

            // Group dates by day and calculate tax per day
            var datesByDay = dates.GroupBy(d => d.Date);
            decimal totalTax = 0;

            foreach (var dayGroup in datesByDay)
            {
                decimal dailyFee = CalculateDailyTax(city, weekendRule, vehicle, dayGroup.ToArray());
                totalTax += dailyFee;
            }

            return totalTax;
        }

        /// <summary>
        /// Calculate tax for a single day (all passes must be from same day)
        /// </summary>
        private decimal CalculateDailyTax(City city, WeekendRule weekendRule, Vehicle vehicle, DateTime[] dates)
        {
            decimal dailyFee = 0;
            DateTime intervalStart = dates[0];
            decimal maxFeeInInterval = GetTollFee(city, weekendRule, intervalStart, vehicle);

            foreach (DateTime date in dates)
            {
                decimal currentFee = GetTollFee(city, weekendRule, date, vehicle);

                // Calculate actual time difference in minutes
                double minutesDifference = (date - intervalStart).TotalMinutes;

                if (minutesDifference <= city.SingleChargeWindowMinutes)
                {
                    // Still within the same charging window
                    // Keep track of the highest fee in this interval
                    if (currentFee > maxFeeInInterval)
                    {
                        maxFeeInInterval = currentFee;
                    }
                }
                else
                {
                    // More than the charging window minutes have passed - new window
                    // Add the maximum fee from the previous interval
                    dailyFee += maxFeeInInterval;

                    // Start a new interval
                    intervalStart = date;
                    maxFeeInInterval = currentFee;
                }
            }

            // Don't forget to add the last interval's fee
            dailyFee += maxFeeInInterval;

            // Apply maximum fee PER DAY
            if (dailyFee > city.MaxDailyFee)
                dailyFee = city.MaxDailyFee;

            return dailyFee;
        }

        /// <summary>
        /// Get the toll fee for a specific date/time
        /// </summary>
        private decimal GetTollFee(City city, WeekendRule weekendRule, DateTime date, Vehicle vehicle)
        {
            // Check if it's a tax-free date
            if (IsTollFreeDate(city, weekendRule, date))
                return 0;

            // Check if it's a tax-free vehicle
            if (IsTollFreeVehicle(city, vehicle))
                return 0;

            // Find the applicable time slot
            var timeOfDay = date.TimeOfDay;
            var timeSlot = city.TaxTimeSlots
                .Where(ts => ts.EffectiveFrom is null || ts.EffectiveFrom <= date)
                .Where(ts => ts.EffectiveUntil is null || ts.EffectiveUntil >= date)
                .FirstOrDefault(ts => timeOfDay >= ts.StartTime && timeOfDay <= ts.EndTime);

            return timeSlot?.TaxAmount ?? 0;
        }

        /// <summary>
        /// Check if the vehicle is tax-free
        /// </summary>
        private bool IsTollFreeVehicle(City city, Vehicle vehicle)
        {
            if (vehicle is null)
                return false;

            var vehicleType = vehicle.GetVehicleType();

            return city.TaxFreeVehicleTypes
                    .Any(vt => vt.VehicleTypeName == vehicleType &&
                              vt.IsActive &&
                              (vt.EffectiveFrom is null || vt.EffectiveFrom <= DateTime.Now) &&
                              (vt.EffectiveUntil is null || vt.EffectiveUntil >= DateTime.Now));
        }

        /// <summary>
        /// Check if the date is tax-free
        /// </summary>
        private bool IsTollFreeDate(City city, WeekendRule weekendRule, DateTime date)
        {
            // Check weekends
            if (weekendRule is not null)
            {
                if (weekendRule.IsSaturdayTaxFree && date.DayOfWeek == DayOfWeek.Saturday)
                    return true;
                if (weekendRule.IsSundayTaxFree && date.DayOfWeek == DayOfWeek.Sunday)
                    return true;
            }

            // Check if it's July (tax-free month for Gothenburg)
            if (city.TaxFreeDates.Any(d => d.Type == TaxFreeDateType.TaxFreeMonth &&
                                          d.Date.Month == date.Month))
                return true;

            // Check specific dates
            var taxFreeDate = city.TaxFreeDates
                .FirstOrDefault(d => d.Date.Date == date.Date);

            if (taxFreeDate is not null)
                return true;

            // Check recurring dates
            var recurringDate = city.TaxFreeDates
                .FirstOrDefault(d => d.IsRecurring &&
                                    d.RecurringMonth == date.Month &&
                                    d.RecurringDay == date.Day);

            return recurringDate is not null;
        }

        /// <summary>
        /// Log the calculation for audit purposes
        /// </summary>
        public void LogCalculation(string cityName, Vehicle vehicle, DateTime[] dates, decimal totalFee, string breakdown)
        {
            var city = _context.Cities.FirstOrDefault(c => c.Name == cityName);
            if (city is null)
                return;

            var log = new TaxCalculationLog
            {
                CityId = city.CityId,
                VehicleType = vehicle.GetVehicleType(),
                CalculationDate = dates[0].Date,
                PassTimestamps = string.Join(",",
                    dates.Select(d => d.ToString("yyyy-MM-dd HH:mm:ss"))),
                TotalFee = totalFee,
                CalculationBreakdown = breakdown,
                CreatedAt = DateTime.Now,
                City = null
            };

            _context.TaxCalculationLogs.Add(log);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get calculation history for a specific date range
        /// </summary>
        public List<TaxCalculationLog> GetCalculationHistory(string cityName, DateTime startDate, DateTime endDate)
        {
            var city = _context.Cities.FirstOrDefault(c => c.Name == cityName);
            if (city is null)
                return new List<TaxCalculationLog>();

            return _context.TaxCalculationLogs
                    .Where(log => log.CityId == city.CityId &&
                                 log.CalculationDate >= startDate &&
                                 log.CalculationDate <= endDate)
                    .OrderByDescending(log => log.CreatedAt)
                    .ToList();
        }
    }
}
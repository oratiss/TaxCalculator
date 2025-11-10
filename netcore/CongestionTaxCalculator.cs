using System;
using System.Linq;

namespace congestion.calculator
{
    public class CongestionTaxCalculator
    {
        /**
         * Calculate the total toll fee for one or multiple days
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes
         * @return - the total congestion tax for all days
         */
        public int GetTax(Vehicle vehicle, DateTime[] dates)
        {
            // Input validation
            if (dates == null || dates.Length == 0)
                return 0;

            // Ensure dates are sorted chronologically
            Array.Sort(dates);

            // Group dates by day and calculate tax per day
            var datesByDay = dates.GroupBy(d => d.Date);
            var totalTax = 0;

            foreach (var dayGroup in datesByDay)
            {
                var dailyFee = CalculateDailyTax(vehicle, dayGroup.ToArray());
                totalTax += dailyFee;
            }

            return totalTax;
        }

        /**
         * Calculate tax for a single day
         */
        private int CalculateDailyTax(Vehicle vehicle, DateTime[] dates)
        {
            int dailyFee = 0;
            DateTime intervalStart = dates[0];
            int maxFeeInInterval = GetTollFee(intervalStart, vehicle);

            foreach (DateTime date in dates)
            {
                int currentFee = GetTollFee(date, vehicle);

                // Calculate actual time difference in minutes
                double minutesDifference = (date - intervalStart).TotalMinutes;

                if (minutesDifference <= 60)
                {
                    // Still within the same 60-minute window
                    // Keep track of the highest fee in this interval
                    if (currentFee > maxFeeInInterval)
                    {
                        maxFeeInInterval = currentFee;
                    }
                }
                else
                {
                    // More than 60 minutes have passed - new charging window
                    // Add the maximum fee from the previous interval
                    dailyFee += maxFeeInInterval;

                    // Start a new interval
                    intervalStart = date;
                    maxFeeInInterval = currentFee;
                }
            }

            // Don't forget to add the last interval's fee
            dailyFee += maxFeeInInterval;

            // Maximum fee PER DAY is 60 SEK
            if (dailyFee > 60)
                dailyFee = 60;

            return dailyFee;
        }

        private bool IsTollFreeVehicle(Vehicle vehicle)
        {
            if (vehicle == null) return false;

            String vehicleType = vehicle.GetVehicleType();

            // Check against all toll-free vehicle types
            return vehicleType.Equals(TollFreeVehicles.Motorcycle.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Motorbike.ToString()) ||  // Added for consistency
                   vehicleType.Equals(TollFreeVehicles.Bus.ToString()) ||        // Added - was missing
                   vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Military.ToString());
        }

        public int GetTollFee(DateTime date, Vehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle))
                return 0;

            int hour = date.Hour;
            int minute = date.Minute;

            // 06:00–06:29 → SEK 8
            if (hour == 6 && minute >= 0 && minute <= 29)
                return 8;

            // 06:30–06:59 → SEK 13
            else if (hour == 6 && minute >= 30 && minute <= 59)
                return 13;

            // 07:00–07:59 → SEK 18
            else if (hour == 7)
                return 18;

            // 08:00–08:29 → SEK 13
            else if (hour == 8 && minute >= 0 && minute <= 29)
                return 13;

            // 08:30–14:59 → SEK 8 (FIXED: was checking wrong range)
            else if ((hour == 8 && minute >= 30) || (hour >= 9 && hour <= 14))
                return 8;

            // 15:00–15:29 → SEK 13
            else if (hour == 15 && minute >= 0 && minute <= 29)
                return 13;

            // 15:30–16:59 → SEK 18 (FIXED: removed overlap)
            else if ((hour == 15 && minute >= 30) || hour == 16)
                return 18;

            // 17:00–17:59 → SEK 13
            else if (hour == 17)
                return 13;

            // 18:00–18:29 → SEK 8
            else if (hour == 18 && minute >= 0 && minute <= 29)
                return 8;

            // 18:30–05:59 → SEK 0
            else
                return 0;
        }

        private bool IsTollFreeDate(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            // Weekends are always toll-free
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return true;

            // July is always toll-free
            if (month == 7)
                return true;

            // Public holidays and days before public holidays for 2013
            if (year == 2013)
            {
                // New Year's Day: Jan 1
                if (month == 1 && day == 1)
                    return true;

                // Epiphany: Jan 6 (and Jan 5 is day before)
                if (month == 1 && (day == 5 || day == 6))
                    return true;

                // Good Friday: Mar 29, Easter Monday: Apr 1
                // Day before Good Friday: Mar 28
                if ((month == 3 && (day == 28 || day == 29)) ||
                    (month == 4 && day == 1))
                    return true;

                // Labour Day: May 1 (and Apr 30 is day before)
                if ((month == 4 && day == 30) || (month == 5 && day == 1))
                    return true;

                // Ascension Day: May 9 (and May 8 is day before)
                if (month == 5 && (day == 8 || day == 9))
                    return true;

                // National Day: Jun 6 (and Jun 5 is day before)
                if (month == 6 && (day == 5 || day == 6))
                    return true;

                // Midsummer Eve: Jun 21, Midsummer Day: Jun 22
                // (Day before: Jun 20)
                if (month == 6 && (day == 20 || day == 21 || day == 22))
                    return true;

                // All Saint's Day: Nov 2 (and Nov 1 is day before)
                if (month == 11 && (day == 1 || day == 2))
                    return true;

                // Christmas Eve: Dec 24, Christmas Day: Dec 25
                // Boxing Day: Dec 26, New Year's Eve: Dec 31
                // Day before Christmas Eve: Dec 23
                if (month == 12 && (day == 23 || day == 24 || day == 25 || day == 26 || day == 31))
                    return true;
            }

            return false;
        }

        private enum TollFreeVehicles
        {
            Motorcycle = 0,
            Motorbike = 1,    // Added to match Motorbike class
            Bus = 2,          // Added - was missing from requirements
            Tractor = 3,
            Emergency = 4,
            Diplomat = 5,
            Foreign = 6,
            Military = 7
        }
    }
}
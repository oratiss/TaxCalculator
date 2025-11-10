using System;
using congestion.calculator;

namespace congestion.calculator.tests
{
    public class CongestionTaxCalculatorTests
    {
        public static void Main(string[] args)
        {
            var calculator = new CongestionTaxCalculator();
            var car = new Car();
            var motorbike = new Motorbike();

            Console.WriteLine("=== Congestion Tax Calculator Tests ===\n");

            // Test 1: Evening passes (should be 0)
            Console.WriteLine("Test 1: Evening passes");
            var dates1 = new DateTime[] {
                DateTime.Parse("2013-01-14 21:00:00"),
                DateTime.Parse("2013-01-15 21:00:00")
            };
            int result1 = calculator.GetTax(car, dates1);
            Console.WriteLine($"Result: {result1} SEK (Expected: 0 SEK)");
            Console.WriteLine($"Status: {(result1 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 2: Single morning pass
            Console.WriteLine("Test 2: Single morning pass");
            var dates2 = new DateTime[] {
                DateTime.Parse("2013-02-07 06:23:27")
            };
            int result2 = calculator.GetTax(car, dates2);
            Console.WriteLine($"Result: {result2} SEK (Expected: 8 SEK)");
            Console.WriteLine($"Status: {(result2 == 8 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 3: Two passes same day, different windows
            Console.WriteLine("Test 3: Two passes, different windows");
            var dates3 = new DateTime[] {
                DateTime.Parse("2013-02-07 06:23:27"),  // 8 SEK
                DateTime.Parse("2013-02-07 15:27:00")   // 13 SEK (9+ hours later)
            };
            int result3 = calculator.GetTax(car, dates3);
            Console.WriteLine($"Result: {result3} SEK (Expected: 21 SEK)");
            Console.WriteLine($"Status: {(result3 == 21 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 4: Multiple passes within 60-minute window (single charge rule)
            Console.WriteLine("Test 4: Multiple passes within 60-minute window");
            var dates4 = new DateTime[] {
                DateTime.Parse("2013-02-08 06:20:27"),  // 8 SEK
                DateTime.Parse("2013-02-08 06:27:00")   // 8 SEK (7 min later - same window, max = 8)
            };
            int result4 = calculator.GetTax(car, dates4);
            Console.WriteLine($"Result: {result4} SEK (Expected: 8 SEK - single charge rule)");
            Console.WriteLine($"Status: {(result4 == 8 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 5: Complex day with multiple windows
            Console.WriteLine("Test 5: Complex day (2013-02-08) - from assignment");
            var dates5 = new DateTime[] {
                DateTime.Parse("2013-02-08 06:27:00"),   // 8 SEK
                DateTime.Parse("2013-02-08 06:20:27"),   // 8 SEK (will be sorted)
                DateTime.Parse("2013-02-08 14:35:00"),   // 8 SEK (new window)
                DateTime.Parse("2013-02-08 15:29:00"),   // 13 SEK
                DateTime.Parse("2013-02-08 15:47:00"),   // 18 SEK (same window as 15:29)
                DateTime.Parse("2013-02-08 16:01:00"),   // 18 SEK (same window)
                DateTime.Parse("2013-02-08 16:48:00"),   // 18 SEK (same window)
                DateTime.Parse("2013-02-08 17:49:00"),   // 13 SEK (new window)
                DateTime.Parse("2013-02-08 18:29:00"),   // 8 SEK (same window as 17:49)
                DateTime.Parse("2013-02-08 18:35:00")    // 0 SEK (new window but free time)
            };
            int result5 = calculator.GetTax(car, dates5);
            Console.WriteLine("Breakdown:");
            Console.WriteLine("  06:20-06:27 → Max 8 SEK");
            Console.WriteLine("  14:35       → 8 SEK (new window)");
            Console.WriteLine("  15:29-16:48 → Max 18 SEK (all within 60 min)");
            Console.WriteLine("  17:49-18:29 → Max 13 SEK");
            Console.WriteLine("  18:35       → 0 SEK");
            Console.WriteLine($"Result: {result5} SEK (Expected: 47 SEK)");
            Console.WriteLine($"Status: {(result5 == 47 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 6: Weekend (toll-free)
            Console.WriteLine("Test 6: Weekend pass");
            var dates6 = new DateTime[] {
                DateTime.Parse("2013-02-09 07:00:00")  // Saturday
            };
            int result6 = calculator.GetTax(car, dates6);
            Console.WriteLine($"Result: {result6} SEK (Expected: 0 SEK - Saturday)");
            Console.WriteLine($"Status: {(result6 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 7: Public holiday (toll-free)
            Console.WriteLine("Test 7: Public holiday");
            var dates7 = new DateTime[] {
                DateTime.Parse("2013-03-28 14:07:27")  // Maundy Thursday (day before Good Friday)
            };
            int result7 = calculator.GetTax(car, dates7);
            Console.WriteLine($"Result: {result7} SEK (Expected: 0 SEK - day before holiday)");
            Console.WriteLine($"Status: {(result7 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 8: July (toll-free month)
            Console.WriteLine("Test 8: July (toll-free month)");
            var dates8 = new DateTime[] {
                DateTime.Parse("2013-07-15 07:00:00")
            };
            int result8 = calculator.GetTax(car, dates8);
            Console.WriteLine($"Result: {result8} SEK (Expected: 0 SEK - July)");
            Console.WriteLine($"Status: {(result8 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 9: Toll-free vehicle (Motorbike)
            Console.WriteLine("Test 9: Toll-free vehicle (Motorbike)");
            var dates9 = new DateTime[] {
                DateTime.Parse("2013-02-07 07:00:00")  // Peak time but motorbike
            };
            int result9 = calculator.GetTax(motorbike, dates9);
            Console.WriteLine($"Result: {result9} SEK (Expected: 0 SEK - Motorbike)");
            Console.WriteLine($"Status: {(result9 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 10: Maximum daily fee (60 SEK cap)
            Console.WriteLine("Test 10: Maximum daily fee (single day)");
            var dates10 = new DateTime[] {
                DateTime.Parse("2013-02-07 06:00:00"),  // 8 SEK
                DateTime.Parse("2013-02-07 08:00:00"),  // 13 SEK (new window)
                DateTime.Parse("2013-02-07 10:00:00"),  // 8 SEK (new window)
                DateTime.Parse("2013-02-07 12:00:00"),  // 8 SEK (new window)
                DateTime.Parse("2013-02-07 15:00:00"),  // 13 SEK (new window)
                DateTime.Parse("2013-02-07 17:00:00")   // 13 SEK (new window) = 63 SEK total
            };
            int result10 = calculator.GetTax(car, dates10);
            Console.WriteLine($"Result: {result10} SEK (Expected: 60 SEK - daily cap)");
            Console.WriteLine($"Status: {(result10 == 60 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 11: Multiple days - IMPORTANT: Cap applies PER DAY
            Console.WriteLine("Test 11: Multiple days - 60 SEK cap per day");
            var dates11 = new DateTime[] {
                // Day 1: Feb 7 (should cap at 60)
                DateTime.Parse("2013-02-07 06:00:00"),  // 8
                DateTime.Parse("2013-02-07 08:00:00"),  // 13
                DateTime.Parse("2013-02-07 10:00:00"),  // 8
                DateTime.Parse("2013-02-07 15:00:00"),  // 13
                DateTime.Parse("2013-02-07 17:00:00"),  // 13 = 55 SEK
                
                // Day 2: Feb 11 (should cap at 60, separate from day 1)
                DateTime.Parse("2013-02-11 06:00:00"),  // 8
                DateTime.Parse("2013-02-11 08:00:00"),  // 13
                DateTime.Parse("2013-02-11 10:00:00"),  // 8
                DateTime.Parse("2013-02-11 15:00:00"),  // 13
                DateTime.Parse("2013-02-11 17:00:00")   // 13 = 55 SEK
            };
            int result11 = calculator.GetTax(car, dates11);
            Console.WriteLine($"Day 1 (Feb 7): 55 SEK, Day 2 (Feb 11): 55 SEK");
            Console.WriteLine($"Result: {result11} SEK (Expected: 110 SEK - NOT capped across days)");
            Console.WriteLine($"Status: {(result11 == 110 ? "✓ PASS" : "✗ FAIL")}\n");

            Console.WriteLine("=== Tests Complete ===");
        }
    }
}
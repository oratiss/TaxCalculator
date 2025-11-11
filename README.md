# Congestion Tax Calculator

Welcome to the Persici Financial Technologies assessment.

This repository contains a developer [assignment](ASSIGNMENT.md) used as a basis for candidate intervew and evaluation.

Clone this repository to get started. Due to a number of reasons, not least privacy, you will be asked to zip your solution and share it in, instead of submitting a pull-request.

Summary of All Fixes
Bug Fixes:

- Time difference calculation - Now uses (date - intervalStart).TotalMinutes instead of comparing millisecond components
- Single charge rule logic - Properly tracks 60-minute windows and only charges the maximum fee within each window
- Time range logic - Fixed the 08:30-14:59 range that was excluding valid times
- Time range logic - Fixed the 15:30-16:59 range that was excluding valid times
- Vehicle type mismatch - Added both "Motorcycle" and "Motorbike" support
- Missing Bus vehicle - Added Bus class and enum value
- Missing holidays - Added day-before-holiday logic and fixed missing holidays

Improvements:

Input validation (null checks, same-day verification)
Automatic sorting of dates
Better code comments
Clearer variable names
Fixed operator precedence issues
Added all toll-free vehicle types from requirements

Test Results that can be seen:
The test file demonstrates all the fixes work correctly, including:

The complex 2013-02-08 test case with multiple 60-minute windows
Public holiday detection (Mar 28 - day before Good Friday)
60 SEK daily maximum cap
Motorbike toll-free verification




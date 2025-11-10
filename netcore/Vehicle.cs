namespace congestion.calculator
{
    public interface Vehicle
    {
        string GetVehicleType();
    }

    // Car - taxable
    public class Car : Vehicle
    {
        public string GetVehicleType()
        {
            return "Car";
        }
    }

    // Motorbike - toll-free (FIXED: was returning "Motorbike" but enum checked for "Motorcycle")
    public class Motorbike : Vehicle
    {
        public string GetVehicleType()
        {
            return "Motorbike";  // Now matches the enum
        }
    }

    // Motorcycle - toll-free (alternative naming)
    public class Motorcycle : Vehicle
    {
        public string GetVehicleType()
        {
            return "Motorcycle";
        }
    }

    // Bus - toll-free (ADDED - was missing)
    public class Bus : Vehicle
    {
        public string GetVehicleType()
        {
            return "Bus";
        }
    }

    // Tractor - toll-free
    public class Tractor : Vehicle
    {
        public string GetVehicleType()
        {
            return "Tractor";
        }
    }

    // Emergency - toll-free
    public class Emergency : Vehicle
    {
        public string GetVehicleType()
        {
            return "Emergency";
        }
    }

    // Diplomat - toll-free
    public class Diplomat : Vehicle
    {
        public string GetVehicleType()
        {
            return "Diplomat";
        }
    }

    // Foreign - toll-free
    public class Foreign : Vehicle
    {
        public string GetVehicleType()
        {
            return "Foreign";
        }
    }

    // Military - toll-free
    public class Military : Vehicle
    {
        public string GetVehicleType()
        {
            return "Military";
        }
    }
}
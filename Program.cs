using System;

// Main program class that serves as the entry point
class Program
{
    static void Main(string[] args)
    {
        // Create and start the shipping quote application
        var app = new ShippingQuoteApplication();
        app.Start();
    }
}

// Class representing shipping quote application events
class ShippingQuoteEventArgs : EventArgs
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public double? Value { get; set; }
}

// Class handling the shipping quote process using events
class ShippingQuoteApplication
{
    // Event declarations
    private event EventHandler<ShippingQuoteEventArgs> OnWeightValidated;
    private event EventHandler<ShippingQuoteEventArgs> OnDimensionsValidated;
    private event EventHandler<ShippingQuoteEventArgs> OnQuoteCalculated;

    // Constants for validation
    private const double MaxWeight = 50;
    private const double MaxDimensions = 50;

    // Package properties
    private double weight;
    private double width;
    private double height;
    private double length;

    public ShippingQuoteApplication()
    {
        // Set up event handlers
        OnWeightValidated += HandleWeightValidation;
        OnDimensionsValidated += HandleDimensionsValidation;
        OnQuoteCalculated += HandleQuoteCalculation;
    }

    // Start the shipping quote process
    public void Start()
    {
        Console.WriteLine("Welcome to Package Express. Please follow the instructions below.");
        GetAndValidateWeight();
    }

    // Get and validate package weight
    private void GetAndValidateWeight()
    {
        Console.WriteLine("Please enter the package weight:");
        if (double.TryParse(Console.ReadLine(), out weight))
        {
            var args = new ShippingQuoteEventArgs
            {
                IsValid = weight <= MaxWeight,
                Message = weight > MaxWeight ? 
                    "Package too heavy to be shipped via Package Express. Have a good day." : 
                    string.Empty,
                Value = weight
            };
            OnWeightValidated?.Invoke(this, args);
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            GetAndValidateWeight();
        }
    }

    // Handle weight validation result
    private void HandleWeightValidation(object sender, ShippingQuoteEventArgs e)
    {
        if (!e.IsValid)
        {
            Console.WriteLine(e.Message);
            return;
        }
        GetAndValidateDimensions();
    }

    // Get and validate package dimensions
    private void GetAndValidateDimensions()
    {
        // Get width
        Console.WriteLine("Please enter the package width:");
        if (!double.TryParse(Console.ReadLine(), out width))
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            GetAndValidateDimensions();
            return;
        }

        // Get height
        Console.WriteLine("Please enter the package height:");
        if (!double.TryParse(Console.ReadLine(), out height))
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            GetAndValidateDimensions();
            return;
        }

        // Get length
        Console.WriteLine("Please enter the package length:");
        if (!double.TryParse(Console.ReadLine(), out length))
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            GetAndValidateDimensions();
            return;
        }

        // Validate total dimensions
        var totalDimensions = width + height + length;
        var args = new ShippingQuoteEventArgs
        {
            IsValid = totalDimensions <= MaxDimensions,
            Message = totalDimensions > MaxDimensions ? 
                "Package too big to be shipped via Package Express." : 
                string.Empty
        };
        OnDimensionsValidated?.Invoke(this, args);
    }

    // Handle dimensions validation result
    private void HandleDimensionsValidation(object sender, ShippingQuoteEventArgs e)
    {
        if (!e.IsValid)
        {
            Console.WriteLine(e.Message);
            return;
        }
        CalculateQuote();
    }

    // Calculate shipping quote
    private void CalculateQuote()
    {
        var quote = (width * height * length * weight) / 100;
        var args = new ShippingQuoteEventArgs
        {
            IsValid = true,
            Value = quote
        };
        OnQuoteCalculated?.Invoke(this, args);
    }

    // Handle quote calculation result
    private void HandleQuoteCalculation(object sender, ShippingQuoteEventArgs e)
    {
        Console.WriteLine($"Your estimated total for shipping this package is: ${e.Value:F2}");
        Console.WriteLine("Thank you!");
    }
}
﻿global using System.Globalization;
global using DataProcessing;
global using DataProcessing.Input;
global using DataProcessing.Reporting;
global using Microsoft.Extensions.Logging;

using System.Text.RegularExpressions;


// Marks when the app is past the user data collection phase.
var initialized = false; 

using var cts = new CancellationTokenSource();

// Register shutdown event handler.
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine();
    var beforeColour = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Canceling...");
    Console.ForegroundColor = beforeColour;
    Console.WriteLine();
    cts.Cancel();

    if (initialized)
        e.Cancel = false;
};

try
{
    /*
    // Configure a logger factory that can provide loggers where required for logging progress.
    using var loggerFactory = LoggerFactory.Create(builder => builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("DataProcessing", LogLevel.Information)
        .AddConsole());

    // Set the output to Unicode to ensure we can display emojis consistently.
    Console.OutputEncoding = System.Text.Encoding.Unicode;

    // App culture
    var appCulture = CultureInfo.CreateSpecificCulture("en-GB");
    CultureInfo.DefaultThreadCurrentCulture = appCulture;

    var (forename, surname, departmentId) = AcceptUserDetails();
    while(!ValidateUserDetails(forename, surname, departmentId))
    {
        (forename, surname, departmentId) = AcceptUserDetails();
    }

    var context = new SessionContext(forename!, surname!, departmentId!);  

     // Configure the options for the processor.
    // Adds two output writers used when reports are generated.
    var options = new ProcessingOptions(appCulture, context, loggerFactory)
        .AddOutputWriter(new ThirdPartyOutputWriter())
        .AddOutputWriter(new ConsoleOutputWriter());

    // Mark app as initialized
    initialized = true;

    // Begin processing.
    await DataProcessor.ProcessAsync(options, cts.Token);

    // Allow enough time for logs to 'flush' to console before completing.
    await Task.Delay(250);

    Console.WriteLine();
    Console.WriteLine("COMPLETED: Press any key to exit.");
    Console.ReadKey();
    */

    var patterns = new List<string>() {
        @"^<(?<tag>[a-z]+).*?>(?<inner>[^<]*)</\1>"
    };


    var inputs = new List<string>() { 
        "a", 
        "<tag style=\"top: 2px\">abc</tag>" 
    };

    patterns.ForEach(pattern =>
    {
        Console.WriteLine("Regular expression: {0}", pattern);
        var Regex = new Regex(pattern);
        inputs.ForEach(input =>
        {
            Console.WriteLine("\tInput pattern: {0}", input);
            var results = Regex.Matches(input);

            if(results.Count <= 0)
            {
                Console.WriteLine("\t\tNo matches found.");
            }
            foreach(Match result in results)
            {
                Console.WriteLine("\t\tMatch found at index {0}. Length: {1}.", result.Index, result.Length);
                foreach (Group group in result.Groups)
                {
                    Console.WriteLine("\t\t\tGroup {0} at index {1} has value {2}.", group.Name, group.Index, group.Value);
                }
            }
        });
    });
}
catch (OperationCanceledException)
{
    Console.WriteLine();
    Console.WriteLine("CANCELLED: Press any key to exit.");
    Console.ReadKey();
}

static (string? forename, string? surname, string? departmentId) AcceptUserDetails ()
{
    Console.Clear();
    Console.WriteLine("Welcome to \"Data Muncher\", the data processor! \U0001F602");
    Console.WriteLine();
    Console.WriteLine("Please provide a few details:\n");

    Console.Write("Forename: ");
    var forename = Console.ReadLine();

    Console.Write("Surname: ");
    var surname = Console.ReadLine();

    Console.Write("Department ID: ");
    var departmentId = Console.ReadLine();

    return (forename, surname, departmentId);
}

static bool ValidateUserDetails(string? forename, string? surname, string? departmentId)
{
    if (string.IsNullOrWhiteSpace(forename) || string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(departmentId))
    {
        Console.Clear();
        Console.WriteLine("Error");
        Console.ReadKey();
        return false;
    }
    return true;
}


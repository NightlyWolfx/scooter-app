# Scooter App

Based on the requirements of a technical task, I built the back-end of an electric scooter rental service application using a test-driven development (TDD) approach.

Skills honed: Unit testing, C#, .NET

The task:

## Goal 

To implement Scooter rental service in code using TDD approach. 
Implement rules which seem logical to you.

## Requirements

- You can update list of available scooters at any time
- You can rent scooters for any time period
- You must calculate rental price when rental ends
- You must calculate rental company income from all rentals and provide yearly report if requested
- Use Visual Studio 2019 community or any paid and/or newer version if available
- Use TDD approach
- Use C# language
- Think about OOP design patterns and S.O.L.I.D. principles
- In case of error, throw different type of exceptions for each situation
- Comments and code must be in English language
- No need for UI and database
- As a result we expect the solution with source code

## We are giving some interfaces and classes

```
public interface IRentalCompany
    {
    /// <summary>
    /// Name of the company.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Start the rent of the scooter.
    /// </summary>
    /// <param name="id">ID of the scooter</param>
    void StartRent(string id);
    /// <summary>
    /// End the rent of the scooter.
    /// </summary>
    /// <param name="id">ID of the scooter</param>
    /// <returns>The total price of rental. It has to calculated taking into account for how long time scooter was rented.
    /// If total amount per day reaches 20 EUR than timer must be stopped and restarted at beginning of next day at 0:00 am.</returns>
    decimal EndRent(string id);
    /// <summary>
    /// Income report.
    /// </summary>
    /// <param name="year">Year of the report. Sum all years if value is not set.</param>
    /// <param name="includeNotCompletedRentals">Include income from the scooters that are rented out (rental has not ended yet) and
    calculate rental
    /// price as if the rental would end at the time when this report was requested.</param>
    /// <returns>The total price of all rentals filtered by year if given.</returns>
    decimal CalculateIncome(int? year, bool includeNotCompletedRentals);
}

public interface IScooterService
{
    /// <summary>
    /// Add scooter.
    /// </summary>
    /// <param name="id">Unique ID of the scooter</param>
    /// <param name="pricePerMinute">Rental price of the scooter per one minute</param>
    void AddScooter(string id, decimal pricePerMinute);
    /// <summary>
    /// Remove scooter. This action is not allowed for scooters if the rental is in progress.
    /// </summary>
    /// <param name="id">Unique ID of the scooter</param>
    void RemoveScooter(string id);
    /// <summary>
    /// List of scooters that belong to the company.
    /// </summary>
    /// <returns>Return a list of available scooters.</returns>
    IList<Scooter> GetScooters();
    /// <summary>
    /// Get particular scooter by ID.
    /// </summary>
    /// <param name="scooterId">Unique ID of the scooter.</param>
    /// <returns>Return a particular scooter.</returns>
    Scooter GetScooterById(string scooterId);
}

public class Scooter
    {
    /// <summary>
    /// Create new instance of the scooter.
    /// </summary>
    /// <param name="id">ID of the scooter.</param>
    /// <param name="pricePerMinute">Rental price of the scooter per one minute.</param>
    public Scooter(string id, decimal pricePerMinute)
    {
    Id = id;
    PricePerMinute = pricePerMinute;
    }
    /// <summary>
    /// Unique ID of the scooter.
    /// </summary>
    public string Id { get; }
    /// <summary>
    /// Rental price of the scooter per one
    minute.
    /// </summary>
    public decimal PricePerMinute { get; }
    /// <summary>
    /// Identify if someone is renting this
    scooter.
    /// </summary>
    public bool IsRented { get; set; }
}

```

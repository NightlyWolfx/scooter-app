namespace ScooterRental
{
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
}

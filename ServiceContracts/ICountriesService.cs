using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represent business logic for manipulating country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">country object ti be added</param>
        /// <returns>returns the country object afer adding it(including the new one)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
        /// <summary>
        /// Returns all the countries from the list
        /// </summary>
        /// <returns>all countries form the list as list of countryresponse</returns>
        Task<List<CountryResponse>> GetAllCounties();
        /// <summary>
        /// returns a country object based on the given country id
        /// </summary>
        /// <param name="countryID"> countryID(Guid)  to search</param>
        /// <returns>matching country as countryresponse object</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}

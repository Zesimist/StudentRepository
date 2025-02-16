using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ApplicationDbContext _context;
        public CountriesService(ApplicationDbContext personsDbContext)
        {
            _context = personsDbContext;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if(countryAddRequest ==null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            if (await _context.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("already exisits");
            }
            Country country=countryAddRequest.ToCountry();
            country.CountryID  =    Guid.NewGuid();

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCounties()
        {
            return await _context.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if(countryID ==null)
            {
                return null;
            }
            Country? CR_from_list= await _context.Countries.FirstOrDefaultAsync(x=> x.CountryID ==countryID);

            if(CR_from_list ==null)
                return null;

            return CR_from_list.ToCountryResponse();
        }
    }
}

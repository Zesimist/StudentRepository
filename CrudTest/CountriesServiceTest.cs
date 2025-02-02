using ServiceContracts.DTO;
using ServiceContracts;
using Services;
namespace CrudTest
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        //Constructor
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(false);
        }
        #region add countries
        //when countryaddrequest is null then throw ArgumentNullException
        [Fact]
        public void AddCountry_empty()
        {
            //arrange
            CountryAddRequest? request = null;
            //act
            Assert.Throws<ArgumentNullException>(() => { _countriesService.AddCountry(request); });
        }
        //when countryname is null then throw ArgumentException
        [Fact]
        public void AddCountry_null()
        {
            //arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName=null};
            //act
            Assert.Throws<ArgumentException>(() => { _countriesService.AddCountry(request); });
        }
        //when countryname is duplicate throw ArgumentException
        [Fact]
        public void AddCountry_duplicate()
        {
            //arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "usa" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "usa" };
            //act
            Assert.Throws<ArgumentException>(() => { _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }
        //proper countryName then insert to the existing list
        [Fact]
        public void AddCountry_proper()
        {
            //arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "India" };
            //act
            CountryResponse response =_countriesService.AddCountry(request);
            List<CountryResponse> responseList_from_GetAllCountries = _countriesService.GetAllCounties();

            Assert.True(response.CountryID != Guid.Empty);
            ///contains method uses equal method. which compares the reference and not the actual data
            Assert.Contains(response, responseList_from_GetAllCountries);

        }
        #endregion
        #region Get all countries
        //when the list is empty
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            List<CountryResponse> actual_country_response_list = _countriesService.GetAllCounties();
            Assert.Empty(actual_country_response_list);
        }
        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            List<CountryAddRequest> countries_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { CountryName = "USA" },
                new CountryAddRequest() { CountryName = "Canada" },
            };
            List<CountryResponse> CR_from_addcountry = new List<CountryResponse>();
            foreach(CountryAddRequest country_request in countries_request_list)
            {
                CR_from_addcountry.Add(_countriesService.AddCountry(country_request));
            }
            List<CountryResponse> actualCountryResponseList =  _countriesService.GetAllCounties();

            foreach(CountryResponse expected_country in CR_from_addcountry)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }
        #endregion

        #region GetcountryByCountryID

        [Fact]
            //if we supply null it should return null response
        public void GetCountryByCountryID_nullID()
        {
            //arrange
            Guid? countryID = null;

            //act
            CountryResponse? country_response_from_get_method = _countriesService.GetCountryByCountryID(countryID);

            //assert
            Assert.Null(country_response_from_get_method);
        }
        [Fact]
        //if we supply valid data it should return valid response
        public void GetCountryByCountryID_validID()
        {
            //arrange
            CountryAddRequest? country_add_request = new CountryAddRequest() { CountryName = "China" };
            CountryResponse cr_from_addrequest = _countriesService.AddCountry(country_add_request);

            //act
           CountryResponse? CR_from_get = _countriesService.GetCountryByCountryID(cr_from_addrequest.CountryID);

            //assert
            Assert.Equal(cr_from_addrequest, CR_from_get);
        }
        #endregion
    }
}

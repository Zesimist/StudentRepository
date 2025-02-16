using Entities;
using ServiceContracts.DTO;
using ServiceContracts.Enum;
using Services;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using ServiceContracts.Enum;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;

namespace CrudTest
{
    public class PersonServiceTest
    {
        private readonly PersonService _personService;
        private readonly CountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonServiceTest(ITestOutputHelper testOutputHelper) {
            // creating Auto Fixture to get sample data
            _fixture = new Fixture();


            //_countriesService = new CountriesService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options));

            //_personService = new PersonService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options), _countriesService);

            ///Mocking the Person service
            var countriesInitailData = new List<Country>() { };
            var personInitialdata = new List<Person>() { }; // this will create empty list if you want any initial data you needto provide the same

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(c => c.Countries,countriesInitailData);
            dbContextMock.CreateDbSetMock(p => p.Persons,personInitialdata);

            _countriesService = new CountriesService(dbContext);
            _personService = new PersonService(dbContext, _countriesService);
            
            _testOutputHelper = testOutputHelper;
        }
        #region add person
        //when countryaddrequest is null then throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //arrange
            AddPersonRequest? request = null;
            //act
            await Assert.ThrowsAsync<ArgumentNullException>(async() => {await _personService.AddPerson(request); });
        }
        //when countryname is null then throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonnameIsNulll()
        {
            //arrange
            //AddPersonRequest? request = new AddPersonRequest() { PersonName = null };
            AddPersonRequest? request=_fixture.Build<AddPersonRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();
            //act
            await Assert.ThrowsAsync<ArgumentException>(async () => { await _personService.AddPerson(request); });
        }
        //proper countryName then insert to the existing list
        [Fact]
        public async Task AddPerson_proper()
        {
            ////arrange
            //AddPersonRequest? request = new AddPersonRequest() { PersonName = "Chris",Email="chris@chis.com",Address="address",CountryID
            //=Guid.NewGuid(),Gender=GenderOptions.Other,DateOfBirth=DateTime.Parse("2020-01-01"),ReceiveNewsLetter=true}; 

            // using auto fixture to create sample data
            AddPersonRequest? request = _fixture.Build<AddPersonRequest>()
                .With(t => t.Email, "xyzd@email.com")
                .Create();

            //act
            PersonResponse response = await _personService.AddPerson(request);
            List<PersonResponse> responseList_from_GetAllPerson = await _personService.GetAllPersons();

            Assert.True(response.PersonID != Guid.Empty);
            ///contains method uses equal method. which compares the reference and not the actual data
            Assert.Contains(response, responseList_from_GetAllPerson);

        }
        #endregion

        #region Get all person
        //when the list is empty
        [Fact]
        public async Task GetAllPerson_EmptyList()
        {
            List<PersonResponse> actual_person_response_list =  await _personService.GetAllPersons();
            Assert.Empty(actual_person_response_list);
        }
        [Fact]
        public async Task GetAllPerson_AddFewperson()
        {
            CountryAddRequest Cr1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest Cr2 = new CountryAddRequest() { CountryName = "Australia" };
            CountryResponse crr1=await _countriesService.AddCountry(Cr1);
            CountryResponse crr2 =await _countriesService.AddCountry(Cr2);

            AddPersonRequest? p1 = new AddPersonRequest()
            {
                PersonName = "John",
                Email = "email@email.com",
                Address = "address",
                CountryID = crr1.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };
            AddPersonRequest? p2 = new AddPersonRequest()
            {
                PersonName = "lannister",
                Email = "email2@email.com",
                Address = "address",
                CountryID = crr2.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };
            AddPersonRequest? p3 = new AddPersonRequest()
            {
                PersonName = "Alisha",
                Email = "email3@email.com",
                Address = "address",
                CountryID = crr2.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };

            List<PersonResponse> person_responseList = new List<PersonResponse>();
            List<AddPersonRequest> person_requests = new List<AddPersonRequest>() { p1,p2,p3};

            foreach(AddPersonRequest pr in person_requests)
            {
                PersonResponse personResponse=await _personService.AddPerson(pr);
                person_responseList.Add(personResponse);

            }
            List<PersonResponse> person_responseFromGet=await _personService.GetAllPersons();

            foreach (PersonResponse expected_personfromAdd in person_responseList)
            {
                Assert.Contains(expected_personfromAdd, person_responseFromGet);
            }

            foreach (PersonResponse expected_personfromAdd in person_responseList)
            {
                _testOutputHelper.WriteLine(expected_personfromAdd.ToString());
            }
        }
        #endregion

        #region GetPersonByPersonID

        [Fact]
        //if we supply null it should return null response
        public async Task GetPersonByPersonID_nullID()
        {
            //arrange
            Guid? personID = null;

            //act
            PersonResponse? person_response_from_get_method =await _personService.GetPersonByPersonID(personID);

            //assert
            Assert.Null(person_response_from_get_method);
        }
            [Fact]
        //if we supply valid data it should return valid response
        public async Task GetPersonByPersonID_validID()
        {
            //arrange
            CountryAddRequest? country_add_request = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponse =await _countriesService.AddCountry(country_add_request);

            AddPersonRequest? person_add_request = new AddPersonRequest() { PersonName = "Moris",
            Email="email@email.com",Address="address",CountryID= countryResponse.CountryID, DateOfBirth=DateTime.Parse("1996-01-01"),Gender=GenderOptions.Other,ReceiveNewsLetter=true};

            PersonResponse personResponse_from_Add =await _personService.AddPerson(person_add_request);


            //act

            PersonResponse? PR_from_get =await _personService.GetPersonByPersonID(personResponse_from_Add.PersonID);


            //assert
            Assert.Equal(personResponse_from_Add, PR_from_get);
        }
        #endregion

        #region get filtered person
        //if search text is empty and search by is "PersonName" it should return all perosns
        [Fact]
        public async Task GetFilteredPerson_EmptySearchText()
        {
            CountryAddRequest Cr1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest Cr2 = new CountryAddRequest() { CountryName = "Australia" };
            CountryResponse crr1 =await _countriesService.AddCountry(Cr1);
            CountryResponse crr2 = await _countriesService.AddCountry(Cr2);

            AddPersonRequest? p1 = new AddPersonRequest()
            {
                PersonName = "John",
                Email = "email@email.com",
                Address = "address",
                CountryID = crr1.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };
            AddPersonRequest? p2 = new AddPersonRequest()
            {
                PersonName = "lannister",
                Email = "email2@email.com",
                Address = "address",
                CountryID = crr2.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };
            AddPersonRequest? p3 = new AddPersonRequest()
            {
                PersonName = "Alisha",
                Email = "email3@email.com",
                Address = "address",
                CountryID = crr2.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };

            List<PersonResponse> person_responseList = new List<PersonResponse>();
            List<AddPersonRequest> person_requests = new List<AddPersonRequest>() { p1, p2, p3 };

            foreach (AddPersonRequest pr in person_requests)
            {
                PersonResponse personResponse =await _personService.AddPerson(pr);
                person_responseList.Add(personResponse);

            }

            List<PersonResponse> person_responseFromSearch = await _personService.GetFilteredPerson(nameof(Person.PersonName),"");

            foreach (PersonResponse expected_personfromAdd in person_responseList)
            {
                Assert.Contains(expected_personfromAdd, person_responseFromSearch);
            }
            //printing the data with the help of test output helper
            foreach (PersonResponse expected_personfromAdd in person_responseList)
            {
                _testOutputHelper.WriteLine(expected_personfromAdd.ToString());
            }
        }

        //first we will add few persons; and then we will search based on person name with some search string
        [Fact]
        public async Task GetFilteredPerson_searchBypersonName()
        {
            CountryAddRequest Cr1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest Cr2 = new CountryAddRequest() { CountryName = "Australia" };
            CountryResponse crr1 = await _countriesService.AddCountry(Cr1);
            CountryResponse crr2 = await _countriesService.AddCountry(Cr2);

            AddPersonRequest? p1 = new AddPersonRequest()
            {
                PersonName = "John",
                Email = "email@email.com",
                Address = "address",
                CountryID = crr1.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };
            AddPersonRequest? p2 = new AddPersonRequest()
            {
                PersonName = "lannister",
                Email = "email2@email.com",
                Address = "address",
                CountryID = crr2.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };
            AddPersonRequest? p3 = new AddPersonRequest()
            {
                PersonName = "Alisha",
                Email = "email3@email.com",
                Address = "address",
                CountryID = crr2.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };

            List<PersonResponse> person_responseList = new List<PersonResponse>();
            List<AddPersonRequest> person_requests = new List<AddPersonRequest>() { p1, p2, p3 };

            foreach (AddPersonRequest pr in person_requests)
            {
                PersonResponse personResponse = await _personService.AddPerson(pr);
                person_responseList.Add(personResponse);

            }

            List<PersonResponse> person_responseFromSearch =await _personService.GetFilteredPerson(nameof(Person.PersonName), "is");

            foreach (PersonResponse expected_personfromAdd in person_responseList)
            {
                if(expected_personfromAdd.PersonName !=null)
                {
                    if (expected_personfromAdd.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(expected_personfromAdd, person_responseFromSearch);
                    }
                }

                
            }
            //printing the data with the help of test output helper
            foreach (PersonResponse expected_personfromAdd in person_responseList)
            {
                _testOutputHelper.WriteLine(person_responseFromSearch .ToString());
            }
        }

        #endregion

        #region Get sorted person
        /// <summary>
        /// when we sort based on a particular field on a perticular order it should do so
        /// </summary>
        [Fact]
        public async Task GetSortedPerson_Test()
        {
            CountryAddRequest Cr1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest Cr2 = new CountryAddRequest() { CountryName = "Australia" };
            CountryResponse crr1 =await _countriesService.AddCountry(Cr1);
            CountryResponse crr2 = await _countriesService.AddCountry(Cr2);

            AddPersonRequest? p1 = new AddPersonRequest()
            {
                PersonName = "John",
                Email = "email@email.com",
                Address = "address",
                CountryID = crr1.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };
            AddPersonRequest? p2 = new AddPersonRequest()
            {
                PersonName = "lannister",
                Email = "email2@email.com",
                Address = "address",
                CountryID = crr2.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };
            AddPersonRequest? p3 = new AddPersonRequest()
            {
                PersonName = "Alisha",
                Email = "email3@email.com",
                Address = "address",
                CountryID = crr2.CountryID,
                DateOfBirth = DateTime.Parse("1996-01-01"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetter = true
            };

            List<PersonResponse> person_responseList_fromAdd = new List<PersonResponse>();
            List<AddPersonRequest> person_requests = new List<AddPersonRequest>() { p1, p2, p3 };

            foreach (AddPersonRequest pr in person_requests)
            {
                PersonResponse personResponse =await _personService.AddPerson(pr);
                person_responseList_fromAdd.Add(personResponse);

            }
            _testOutputHelper.WriteLine("Expected:");
            foreach(PersonResponse person_responseListfromAdd in person_responseList_fromAdd)
            {
                _testOutputHelper.WriteLine(person_responseListfromAdd.ToString());
            }

            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            List<PersonResponse> person_responseFromSort =await _personService.GetSortedPersons(allPersons,nameof(Person.PersonName), SortOptions.DESC);

            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personresponseListfromsort in person_responseFromSort)
            {
                _testOutputHelper.WriteLine(personresponseListfromsort.ToString());
            }

            person_responseList_fromAdd = person_responseList_fromAdd.OrderByDescending(k=>k.PersonName).ToList();

            for(int i=0; i< person_responseList_fromAdd.Count;i++)
            {
                Assert.Equal(person_responseList_fromAdd[i], person_responseFromSort[i]);
            }
        }
        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() => {
                await _personService.UpdatePerson(person_update_request);
            });
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => {await
                _personService.UpdatePerson(person_update_request);
            });
        }


        //When PersonName is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add =await _countriesService.AddCountry(country_add_request);

            AddPersonRequest person_add_request = new AddPersonRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID,Email ="email@email.com" };
            PersonResponse person_response_from_add =await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() => {
                //Act
                await _personService.UpdatePerson(person_update_request);
            });

        }


        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add =await _countriesService.AddCountry(country_add_request);

            AddPersonRequest person_add_request = new AddPersonRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID, Address = "Abc road", DateOfBirth = DateTime.Parse("2000-01-01"), Email = "abc@example.com", Gender = GenderOptions.Male, ReceiveNewsLetter = true };

            PersonResponse person_response_from_add =await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);

        }

        #endregion

        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_from_add =await _countriesService.AddCountry(country_add_request);

            AddPersonRequest person_add_request = new AddPersonRequest() { PersonName = "Jones", Address = "address", CountryID = country_response_from_add.CountryID, DateOfBirth = Convert.ToDateTime("2010-01-01"), Email = "jones@example.com", Gender = GenderOptions.Male, ReceiveNewsLetter = true };

            PersonResponse person_response_from_add =await _personService.AddPerson(person_add_request);


            //Act
            bool isDeleted =await _personService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            Assert.True(isDeleted);
        }


        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted =await _personService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }

        #endregion
    }
}

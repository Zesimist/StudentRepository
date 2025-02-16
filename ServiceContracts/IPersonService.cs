using ServiceContracts.DTO;
using ServiceContracts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonService
    {
        /// <summary>
        /// adds a new person into the list of person
        /// </summary>
        /// <param name="AddPersonRequest">person to add</param>
        /// <returns> return the same person details + new person</returns>
        Task<PersonResponse> AddPerson(AddPersonRequest? AddPersonRequest);
        /// <summary>
        /// return all persons
        /// </summary>
        /// <returns>return a list of onject of personresponse type</returns>
        Task<List<PersonResponse>> GetAllPersons();
        /// <summary>
        /// Returns the person object based nont he given perosn ID
        /// </summary>
        /// <param name="personID">perosn id to search</param>
        /// <returns> return the matching person object</returns>
        Task<PersonResponse> GetPersonByPersonID(Guid? personID);
        /// <summary>
        /// return all person object thta matches with the given search fields
        /// </summary>
        /// <param name="searchBy">search field to search</param>
        /// <param name="searchString">search string</param>
        /// <returns></returns>
        Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString);
        /// <summary>
        /// resturn sorted list of persons
        /// </summary>
        /// <param name="allPersons">list to be sorted</param>
        /// <param name="sortBy">name of property based on which list to be sorted</param>
        /// <param name="sortOptions">ASC or DESC</param>
        /// <returns></returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOptions sortOptions);
        /// <summary>
        /// updates the specific person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest"> perosnd details to update including the personID</param>
        /// <returns>returns the person response object after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Deletes a person based on the given peron ID
        /// </summary>
        /// <param name="PersonID">Person Id to delete</param>
        /// <returns>return true of deleteion is successful</returns>
        Task<bool> DeletePerson(Guid? PersonID);
    }
}

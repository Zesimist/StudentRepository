using Entities;
using ServiceContracts.Enum;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// Reprsent the DTO class that contains the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required]
        public Guid PersonID { get; set; }
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email value should be a valid email ")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetter { get; set; }
        /// <summary>
        /// converts the current object of personAddRequest into a new object of person type
        /// </summary>
        /// <returns>person object</returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonID=PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetter = ReceiveNewsLetter
            };
        }
    }
}

using System;
using Entities;
using ServiceContracts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// represent DTO class taht is used as returen type of most method of person service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Age { get; set; }
        public string? Gender { get; set; }
        public string? CountryName { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetter { get; set; }
        /// <summary>
        /// compares the current object data witht the parameter object
        /// </summary>
        /// <param name="obj">the Personresponse object to compare</param>
        /// <returns>indicates whether the person deatila matched</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(PersonResponse))
            {
                return false;
            }
            PersonResponse person_to_compare = (PersonResponse)obj;
            return this.CountryID == person_to_compare.CountryID &&
                this.PersonName == person_to_compare.PersonName &&
                this.PersonID == person_to_compare.PersonID &&
                this.Gender == person_to_compare.Gender &&
                this.Email == person_to_compare.Email &&
                this.Address == person_to_compare.Address &&
                this.ReceiveNewsLetter == person_to_compare.ReceiveNewsLetter;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"Person ID : {PersonID}, Person Name: {PersonName}";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest() { PersonID = PersonID, PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, 
                Gender = (GenderOptions)System.Enum.Parse(typeof(GenderOptions), Gender, true), Address = Address, CountryID = CountryID, ReceiveNewsLetter = ReceiveNewsLetter,
            };
        }
    }
    public static class PersonExtension
    {
        /// <summary>
        /// extension method to convert an object of person class into personresponse class
        /// </summary>
        /// <param name="person">the object to be convert</param>
        /// <returns>returns the converted personresponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            //return new PersonResponse()
            //{
            //    CountryID = person.CountryID,
            //    PersonName = person.PersonName,
            //    PersonID = person.PersonID,
            //    Gender = person.Gender,
            //    Email = person.Email,
            //    Address = person.Address,
            //    ReceiveNewsLetter = person.ReceiveNewsLetter,
            //    Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays/365.25):null
            //};
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                ReceiveNewsLetter = person.ReceiveNewsLetter,
                Address = person.Address,
                CountryID = person.CountryID,
                Gender = person.Gender,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
                CountryName = person.Country?.CountryName
            };
        }
    }
}

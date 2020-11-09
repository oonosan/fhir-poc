using FHIR_POC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace FHIR_POC.Application.Interfaces
{
    public interface IPatientService
    {
        /// <summary>
        /// Returns search query with filters added
        /// </summary>
        /// <param name="gender">Patient Gender</param>
        /// <param name="name">Patient Name</param>
        /// <param name="lastName">Patient LastName</param>
        /// <returns>SearchParams</returns>
        SearchParams GetQuery(string gender, string name, string lastName, string country, string state, string city, string birthDate);

        /// <summary>
        /// Returns a patient by Id
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <returns>Models.Patient</returns>
        Domain.Models.Patient GetById(string patientId);

        /// <summary>
        /// Joins a list a string to a single string
        /// </summary>
        /// <param name="lines">List of string</param>
        /// <returns>string</returns>
        string ListStringJoined(IEnumerable<string> lines);

        /// <summary>
        /// Parse a Hl7.Fhir.Model.Address list to a Models.Address List
        /// </summary>
        /// <param name="addressList">Hl7.Fhir.Model.Address</param>
        /// <returns>IList<Models.Address></returns>
        IList<Domain.Models.Address> GetAddresses(List<Hl7.Fhir.Model.Address> addressList);

        /// <summary>
        /// Parse HumanName name to PatientName
        /// </summary>
        /// <param name="nameList">IEnumerable<HumanName></param>
        /// <returns>List<PatientName></returns>
        List<PatientName> GetNames(IEnumerable<HumanName> nameList);

        /// <summary>
        /// Parse Hl7.Fhir.Model.Patient to Patient model
        /// </summary>
        /// <param name="patientFhir">Hl7.Fhir.Model.Patient</param>
        /// <returns>Models.Patient</returns>
        Domain.Models.Patient GetPatientModel(Hl7.Fhir.Model.Patient patientFhir);

        /// <summary>
        /// Goes to the next page of the search
        /// </summary>
        /// <returns>List<Models.Patient></returns>
        Task<List<Domain.Models.Patient>> NextPage();

        /// <summary>
        /// Goes to the previous page of the search
        /// </summary>
        /// <returns>List<Models.Patient></returns>
        Task<List<Domain.Models.Patient>> PreviousPage();

        /// <summary>
        /// Parse date of type DateTimeOffset to string
        /// </summary>
        /// <param name="date">DateTimeOffset</param>
        /// <returns>string</returns>
        string getDate(DateTimeOffset date);
    }
}

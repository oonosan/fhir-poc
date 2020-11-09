using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using FHIR_POC.Application.Interfaces;
using FHIR_POC.Application.Models;
using FHIR_POC.Infrastructure.Repositories;

namespace FHIR_POC.Services
{
    public class PatientService : IPatientService
    {
        public Bundle results;
        private IPatientRepository _patientRepository;
        public bool readFailed;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        /// <summary>
        /// Search a Patient by selected filters
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <param name="gender">Gender</param>
        /// <param name="name">Name</param>
        /// <param name="lastName">Last Name</param>
        /// <returns>List<Models.Patient></returns>
        public Task<List<Application.Models.Patient>> GetPatientAsync(string patientId, string gender, string name, string lastName, string country, string state, string city, DateTimeOffset birthDate)
        {
            results = new Bundle();
            var result = new List<Application.Models.Patient>();

            if (string.IsNullOrEmpty(gender) &&
                string.IsNullOrEmpty(patientId) &&
                string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(lastName) &&
                string.IsNullOrEmpty(country) &&
                string.IsNullOrEmpty(state) &&
                string.IsNullOrEmpty(city) &&
                string.IsNullOrEmpty(birthDate.ToString()))
            {
                return System.Threading.Tasks.Task.FromResult(result);
            }

            if (!string.IsNullOrEmpty(patientId))
            {
                var p = GetById(patientId);
                result.Add(p);
            }
            else
            {
                var birthDateString = getDate(birthDate);

                // Search filters
                var query = GetQuery(gender, name, lastName, country, state, city, birthDateString);

                // Execute query
                results = _patientRepository.ExecuteQuery(query);

                foreach (var p in results.Entry)
                {
                    var id = p.Resource.Id;
                    if (!string.IsNullOrEmpty(id))
                    {
                        try
                        {
                            var patient = GetById(id);
                            result.Add(patient);
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                }
            }
            return System.Threading.Tasks.Task.FromResult(result);
        }

        /// <summary>
        /// Returns search query with filters added
        /// </summary>
        /// <param name="gender">Patient Gender</param>
        /// <param name="name">Patient Name</param>
        /// <param name="lastName">Patient LastName</param>
        /// <returns>SearchParams</returns>
        public SearchParams GetQuery(string gender, string name, string lastName, string country, string state, string city, string birthDate)
        {
            int resultsPerPage = 5;

            var query = new SearchParams()
                        .OrderBy("name", SortOrder.Ascending);

            query.Count = resultsPerPage;

            if (!string.IsNullOrEmpty(gender))
            {
                if (!gender.ToLower().Equals("all"))
                {
                    if (gender.ToLower().Equals("male"))
                    {
                        query.Add("gender", "male");
                    }
                    else if (gender.ToLower().Equals("female"))
                    {
                        query.Add("gender", "female");
                    }
                    else
                    {
                        query.Add("gender:missing", "true");
                    }
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                query.Add("given", name);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query.Add("family", lastName);
            }

            if (!string.IsNullOrEmpty(country))
            {
                query.Add("address-country", country);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query.Add("address-state", state);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query.Add("address-city", city);
            }

            if (!string.IsNullOrEmpty(birthDate))
            {
                query.Add("birthdate", birthDate);
            }

            return query;
        }

        /// <summary>
        /// Returns a patient by Id
        /// </summary>
        /// <param name="patientId">Patient Id</param>
        /// <returns>Models.Patient</returns>
        public Application.Models.Patient GetById(string patientId)
        {
            try
            {
                readFailed = false;
                var result = _patientRepository.GetById(patientId);
                return GetPatientModel(result);
            } catch (Exception e)
            {
                readFailed = true;
                throw e;
            }
        }

        /// <summary>
        /// Joins a list a string to a single string
        /// </summary>
        /// <param name="lines">List of string</param>
        /// <returns>string</returns>
        public string ListStringJoined(IEnumerable<string> lines)
        {
            string line = "";

            foreach (var l in lines)
            {
                line = string.Join(", ", l);
            }

            return line;
        }

        /// <summary>
        /// Parse a Hl7.Fhir.Model.Address list to a Models.Address List
        /// </summary>
        /// <param name="addressList">Hl7.Fhir.Model.Address</param>
        /// <returns>IList<Models.Address></returns>
        public IList<Application.Models.Address> GetAddresses(List<Hl7.Fhir.Model.Address> addressList)
        {
            if (addressList is null)
            {
                throw new ArgumentNullException(nameof(addressList));
            }

            var addresses = new List<Application.Models.Address>();

            foreach (var a in addressList)
            {
                var address = new Application.Models.Address()
                {
                    Country = a.Country,
                    State = a.State,
                    City = a.City,
                    Line = ListStringJoined(a.Line)
                };

                addresses.Add(address);
            }

            return addresses;
        }

        /// <summary>
        /// Parse HumanName name to PatientName
        /// </summary>
        /// <param name="nameList">IEnumerable<HumanName></param>
        /// <returns>List<PatientName></returns>
        public List<PatientName> GetNames(IEnumerable<HumanName> nameList)
        {
            var names = new List<PatientName>();

            foreach (var n in nameList)
            {
                var name = new PatientName();

                name.Use = n.Use != null ? n.Use.ToString() : string.Empty;
                name.Name = n.Given != null ? ListStringJoined(n.Given) : string.Empty;
                name.LastName = n.Family != null ? n.Family : string.Empty;

                names.Add(name);
            }

            return names;
        }

        /// <summary>
        /// Parse Hl7.Fhir.Model.Patient to Patient model
        /// </summary>
        /// <param name="patientFhir">Hl7.Fhir.Model.Patient</param>
        /// <returns>Models.Patient</returns>
        public Application.Models.Patient GetPatientModel(Hl7.Fhir.Model.Patient patientFhir)
        {
            var patient = new Application.Models.Patient()
            {
                Id = patientFhir.Id,
                PatientName = GetNames(patientFhir.Name),
                BirthDate = patientFhir.BirthDate,
                Gender = patientFhir.Gender.ToString(),
                Address = GetAddresses(patientFhir.Address)
            };

            return patient;
        }

        /// <summary>
        /// Goes to the next page of the search
        /// </summary>
        /// <returns>List<Models.Patient></returns>
        public Task<List<Application.Models.Patient>> NextPage()
        {
            var result = new List<Application.Models.Patient>();

            results = _patientRepository.GetNextPage(results);

            if (results != null)
            {
                foreach (var p in results.Entry)
                {
                    var id = p.Resource.Id;
                    if (!string.IsNullOrEmpty(id))
                    {
                        var patient = GetById(id);
                        result.Add(patient);
                    }
                }
            }

            return System.Threading.Tasks.Task.FromResult(result);
        }

        /// <summary>
        /// Goes to the previous page of the search
        /// </summary>
        /// <returns>List<Models.Patient></returns>
        public Task<List<Application.Models.Patient>> PreviousPage()
        {
            var result = new List<Application.Models.Patient>();

            results = _patientRepository.GetPreviousPage(results);
            if (results != null)
            {
                foreach (var p in results.Entry)
                {
                    var id = p.Resource.Id;
                    if (!string.IsNullOrEmpty(id))
                    {
                        var patient = GetById(id);
                        result.Add(patient);
                    }
                }
            }

            return System.Threading.Tasks.Task.FromResult(result);
        }

        /// <summary>
        /// Parse date of type DateTimeOffset to string
        /// </summary>
        /// <param name="date">DateTimeOffset</param>
        /// <returns>string</returns>
        public string getDate(DateTimeOffset date)
        {
            var month = string.Empty;
            var day = string.Empty;


            if (date.Month < 10)
            {
                month = "0" + date.Month;
            }

            if (date.Day < 10)
            {
                day = "0" + date.Day;
            }

            return date.Year + "-" + month + "-" + day;
        }
    }
}

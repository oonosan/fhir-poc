using FHIR_POC.Application.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;

namespace FHIR_POC.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private FhirClient client;

        public PatientRepository()
        {
            // Create a client
            //client = new FhirClient("http://hapi.fhir.org/baseR4/");
            client = new FhirClient("https://vonk.fire.ly/");
            client.Settings.PreferredFormat = ResourceFormat.Json;
            client.Settings.Timeout = 120000; // The timeout is set in milliseconds, with a default of 100000
        }

        public Patient GetById(string patientId)
        {
            //var location = new Uri($"http://hapi.fhir.org/baseR4/Patient/{patientId}");
            var location = new Uri($"https://vonk.fire.ly/Patient/{patientId}");

            try
            {
                return client.Read<Hl7.Fhir.Model.Patient>(location);
            }
            catch (Hl7.Fhir.ElementModel.StructuralTypeException structuralTypeException)
            {
                throw structuralTypeException;
            }
        }

        public Bundle ExecuteQuery(SearchParams query)
        {
            return client.Search<Hl7.Fhir.Model.Patient>(query);
        }

        public Bundle GetNextPage(Bundle results)
        {
            results = client.Continue(results, PageDirection.Next);
            return results;
        }

        public Bundle GetPreviousPage(Bundle results)
        {
            results = client.Continue(results, PageDirection.Previous);
            return results;
        }
    }
}

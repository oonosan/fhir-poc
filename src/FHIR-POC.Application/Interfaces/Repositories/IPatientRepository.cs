using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace FHIR_POC.Application.Interfaces
{
    public interface IPatientRepository
    {
       Patient GetById(string patientId);
       Bundle ExecuteQuery(SearchParams query);
        Bundle GetNextPage(Bundle results);
        Bundle GetPreviousPage(Bundle results);
    }
}

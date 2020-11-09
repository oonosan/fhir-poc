using System.Collections.Generic;

namespace FHIR_POC.Application.Models
{
    public class Patient
    {
        public string Id { get; set; }
        public List<PatientName> PatientName { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public IList<Address> Address { get; set; }
    }
}

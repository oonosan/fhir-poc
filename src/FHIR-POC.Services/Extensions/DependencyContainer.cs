using FHIR_POC.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FHIR_POC.Services.Extensions
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPatientService, PatientService>();
        }
    }
}

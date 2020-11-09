using FHIR_POC.Application.Interfaces;
using FHIR_POC.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FHIR_POC.Services.Extensions
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPatientService, PatientService>();

            services.AddScoped<IPatientRepository, PatientRepository>();
        }
    }
}

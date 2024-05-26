
namespace Patient.Domain.Abstractions
{
    public interface IPatientBossService
    {
        Task<DomainModels.Patient> Get(Guid Id);

        Task Add(DomainModels.Patient patient);

        Task Update(DomainModels.Patient patient);

        Task Delete(Guid Id);
    }
}

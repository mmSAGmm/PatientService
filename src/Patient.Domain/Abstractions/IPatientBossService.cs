
namespace Patient.Domain.Abstractions
{
    public interface IPatientBossService
    {
        Task<DomainModels.Patient> Get(Guid Id, CancellationToken token);

        Task Add(DomainModels.Patient patient, CancellationToken token);

        Task Update(DomainModels.Patient patient, CancellationToken token);

        Task Delete(Guid Id, CancellationToken token);

        Task<IEnumerable<DomainModels.Patient>> Search(string[] pattern, CancellationToken token);
    }
}

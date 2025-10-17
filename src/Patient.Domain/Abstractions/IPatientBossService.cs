
namespace Patient.Domain.Abstractions
{
    public interface IPatientBossService
    {
        Task<DomainModels.Patient> GetAsync(Guid Id, CancellationToken token);

        Task AddAsync(DomainModels.Patient patient, CancellationToken token);

        Task UpdateAsync(DomainModels.Patient patient, CancellationToken token);

        Task DeleteAsync(Guid Id, CancellationToken token);

        Task<IEnumerable<DomainModels.Patient>> SearchAsync(string[] pattern, CancellationToken token);
    }
}

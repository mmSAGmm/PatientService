using AutoFixture.Xunit3;
using DbDataAccess.Abstractions;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenRemovePatient : BasePatientBossServiceFixture
    {
        private void Invoke(Guid id) => Subject.DeleteAsync(id, CancellationToken.None);

        [Theory, AutoData]
        public void ShouldCallRepo(Guid id) 
        {
            Invoke(id);
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.DeleteAsync(id, CancellationToken.None));
        }
    }
}

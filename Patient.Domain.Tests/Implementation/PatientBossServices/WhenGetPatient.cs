using AutoFixture.Xunit3;
using DbDataAccess.Abstractions;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenGetPatient : BasePatientBossServiceFixture
    {
        public WhenGetPatient()
        {

        }

        [Theory, AutoData]
        public void ShouldCallRepo(DomainModels.Patient patient)
        {
            Invoke(patient);
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.GetAsync(patient.Id, CancellationToken.None));
        }


        private void Invoke(DomainModels.Patient model)
        {
            Subject.GetAsync(model.Id, CancellationToken.None);
        }
    }
}

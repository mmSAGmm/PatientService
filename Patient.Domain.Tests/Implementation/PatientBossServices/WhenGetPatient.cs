using DbDataAccess.Abstractions;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenGetPatient : BasePatientBossServiceFixture
    {
        private DomainModels.Patient model;

        public WhenGetPatient()
        {
            model = new DomainModels.Patient();
            Subject.Get(model.Id, CancellationToken.None);
        }

        [Fact]
        public void ShouldCallRepo() 
        {
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.Get(model.Id, CancellationToken.None));
        }
    }
}

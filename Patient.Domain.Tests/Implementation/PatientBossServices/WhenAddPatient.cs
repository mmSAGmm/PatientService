using DbDataAccess.Abstractions;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenAddPatient : BasePatientBossServiceFixture
    {
        private DomainModels.Patient model;

        public WhenAddPatient() 
        {
            model = new DomainModels.Patient();
            Subject.Add(model, CancellationToken.None);
        }

        [Fact]
        public void ShouldCallRepo()
        {
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.Add(model, CancellationToken.None));
        }
    }
}

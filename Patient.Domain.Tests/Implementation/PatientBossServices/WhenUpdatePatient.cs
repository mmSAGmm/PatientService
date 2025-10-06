using DbDataAccess.Abstractions;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenUpdatePatient : BasePatientBossServiceFixture
    {
        private DomainModels.Patient model;

        public WhenUpdatePatient() 
        {
            model = new DomainModels.Patient();
            Subject.Update(model, CancellationToken.None);
        }

        [Fact]
        public void ShouldCallRepo() 
        {
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.Update(model, CancellationToken.None));
        }
    }
}

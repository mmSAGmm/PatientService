using DbDataAccess.Abstractions;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenUpdatePatient : BasePatientBossServiceFixture
    {
        private DomainModels.Patient model;

        public WhenUpdatePatient() 
        {
            model = new DomainModels.Patient();
            Subject.UpdateAsync(model, CancellationToken.None);
        }

        [Fact]
        public void ShouldCallRepo() 
        {
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.UpdateAsync(model, CancellationToken.None));
        }
    }
}

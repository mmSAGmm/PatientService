using DbDataAccess.Abstractions;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenRemovePatient : BasePatientBossServiceFixture
    {
        private DomainModels.Patient model;

        public WhenRemovePatient() 
        {
            model = new DomainModels.Patient();
            Subject.Delete(model.Id, CancellationToken.None);
        }

        [Fact]
        public void ShouldCallRepo() 
        {
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.Delete(model.Id, CancellationToken.None));
        }
    }
}

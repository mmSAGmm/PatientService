using AutoFixture.Xunit3;
using DbDataAccess.Abstractions;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenRemovePatient : BasePatientBossServiceFixture
    {

        public WhenRemovePatient() 
        {
           
        }

        private void Invoke(Guid id) => Subject.Delete(id, CancellationToken.None);

        [Theory, AutoData]
        public void ShouldCallRepo(Guid id) 
        {
            Invoke(id);
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.Delete(id, CancellationToken.None));
        }
    }
}

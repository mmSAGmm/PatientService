using Moq.AutoMock;
using Patient.Domain.Implementation;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class BasePatientBossServiceFixture
    {
        public AutoMocker mocker = new AutoMocker();

        public PatientBossService Subject { get; set; } 
    
        public BasePatientBossServiceFixture()
        {
            Subject = mocker.CreateInstance<PatientBossService>();
        }


    }
}

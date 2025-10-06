using DbDataAccess.Abstractions;
using Moq;
using Patient.Domain.Abstractions;
using Patient.DomainModels.QueryParse;

namespace Patient.Domain.Tests.Implementation.PatientBossServices
{
    public class WhenSearchPatient : BasePatientBossServiceFixture
    {
        public WhenSearchPatient() 
        {
            Subject.Search([string.Empty], CancellationToken.None);
        }

        [Fact]
        public void ShouldCallRepo() 
        {
            mocker.GetMock<IPatientRepository>()
                .Verify(x => x.Search(It.IsAny<IEnumerable<ParseResult>>(), CancellationToken.None));
        }

        [Fact]
        public void ShouldCallQueryParser()
        {
            mocker.GetMock<IQueryParser>()
                .Verify(x => x.Parse(string.Empty));
        }
    }
}

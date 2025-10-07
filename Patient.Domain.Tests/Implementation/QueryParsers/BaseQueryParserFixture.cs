using Moq.AutoMock;
using Patient.Domain.Implementation;

namespace Patient.Domain.Tests.Implementation.QueryParsers
{
    public class BaseQueryParserFixture
    {
        protected AutoMocker autoMocker = new AutoMocker();

        protected QueryParser Subject;

        protected BaseQueryParserFixture() 
        {
            Subject = autoMocker.CreateInstance<QueryParser>();
        }
    }
}

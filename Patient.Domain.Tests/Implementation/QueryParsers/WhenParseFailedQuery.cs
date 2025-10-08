using Patient.DomainModels.QueryParse;
using Shouldly;

namespace Patient.Domain.Tests.Implementation.QueryParsers
{
    public class WhenParseFailedQuery : BaseQueryParserFixture
    {
        ParseResult parseResult;

        public WhenParseFailedQuery() 
        {
            parseResult = Subject.Parse("asaas");
        }

        [Fact]
        public void ShouldBeFailed() 
        {
            parseResult.Status.ShouldBe(Status.Failed);
        }
    }
}

using Patient.DomainModels.QueryParse;
using Shouldly;

namespace Patient.Domain.Tests.Implementation.QueryParsers
{
    public class WhenParseQuery : BaseQueryParserFixture
    {
        ParseResult parseResult;

        public WhenParseQuery() 
        {
            parseResult = Subject.Parse("eq1999-12-12T12:12:12");
        }

        [Fact]
        public void ShouldReturnData() 
        {
            parseResult.Time.ShouldBe(new TimeOnly(12, 12, 12));
            parseResult.Date.ShouldBe(new DateOnly(1999, 12, 12));
            parseResult.Prefix.ShouldBe(Prefix.Equal);
        }

        [Fact]
        public void ShouldBeSuccesfull() 
        {
            parseResult.Status.ShouldBe(Status.Success);
        }
    }
}

using Patient.DomainModels.QueryParse;

namespace Patient.Domain.Abstractions
{
    public interface IQueryParser
    {
        ParseResult Parse(string pattern);
    }
}

using Patient.Domain.Abstractions;
using Patient.DomainModels.QueryParse;
using System.Text.RegularExpressions;

namespace Patient.Domain.Implementation
{
    public class QueryParser : IQueryParser
    {
        private Regex _regex = new Regex(@"(?<prefix>[\w]{2}){0,1}(?<date>\d{4}-\d{2}-\d{2}){1}(?<time>T[\d]{2}(:[\d]{2}){0,1}(:\d\d){0,1}){0,1}");
     
        public ParseResult Parse(string pattern)
        {
            string prefix = string.Empty;
            string date = string.Empty;
            string time = string.Empty;
            var match = _regex.Match(pattern);
            if (match.Success)
            {
                if (match.Groups["prefix"].Success)
                {
                    prefix = match.Groups["prefix"].Value;
                }

                if (match.Groups["date"].Success)
                {
                    date = match.Groups["date"].Value;
                }

                if (match.Groups["time"].Success)
                {
                    time = match.Groups["time"].Value.Trim('T');
                }

                return ParseResult.Result(prefix, date, time);
            }
            else
            {
                return ParseResult.Fail();
            }
        }
    }
}

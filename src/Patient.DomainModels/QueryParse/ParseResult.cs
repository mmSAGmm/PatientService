using System;

namespace Patient.DomainModels.QueryParse
{
    public class ParseResult
    {
        public Status Status { get; set; }

        public Prefix Prefix { get; set; }

        public DateOnly? Date { get; set; }

        public TimeOnly? Time { get; set; }

        public static ParseResult Fail() => new ParseResult { Status = Status.Failed };

        public static ParseResult Result(string prefix, string date, string time)
        {
            var prefixValue = prefix switch
            {
                "eq" => Prefix.Equal,
                "lt" => Prefix.LessThan,
                "gt" => Prefix.GraterThan,
                "ge" => Prefix.GreaterOrEqual,
                "le" => Prefix.LessOrEqual,
                "sa" => Prefix.StartsAfter,
                "eb" => Prefix.EndBefore,
                "ap" => Prefix.Approximately,
                _ => Prefix.Empty
            };

            DateOnly? dateParsed = default;
            dateParsed = DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsed) ? parsed : null;
            TimeOnly? timeParsed = default;
            if (!string.IsNullOrEmpty(time))
            {
                var timeArray = time.Split(':');
                TimeSpan timeSpan = default;
                for (int i = 0; i < timeArray.Length; i++)
                {
                    var section = timeArray[i];
                    var pefiod = i switch
                    {
                        0 => TimeSpan.FromHours(int.Parse(section)),
                        1 => TimeSpan.FromMinutes(int.Parse(section)),
                        2 => TimeSpan.FromSeconds(int.Parse(section))
                    };
                    timeSpan = timeSpan.Add(pefiod);
                }
                timeParsed = new TimeOnly(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }

            return new ParseResult
            {
                Status = Status.Success,
                Prefix = prefixValue,
                Date = dateParsed,
                Time = timeParsed
            };
        }
    }
}

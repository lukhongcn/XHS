using System.Collections.Generic;

namespace XHS.service.DateCode
{
    internal sealed class DateCodeRule
    {
        public string RuleName { get; set; }

        public int Length { get; set; }

        public Dictionary<string, DateCodeFieldRule> Fields { get; set; }
    }

    internal sealed class DateCodeFieldRule
    {
        public int Position { get; set; }

        public string Description { get; set; }

        public Dictionary<string, string> Map { get; set; }
    }
}

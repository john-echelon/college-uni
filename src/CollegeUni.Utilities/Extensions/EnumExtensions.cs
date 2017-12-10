using System;

namespace CollegeUni.Api.Utilities.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsFlagDefined(this Enum e)
        {
            decimal d;
            return !decimal.TryParse(e.ToString(), out d);
        }
    }
}

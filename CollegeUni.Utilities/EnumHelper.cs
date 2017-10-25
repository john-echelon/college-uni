﻿using System;

namespace CollegeUni.Utilities
{
    public class EnumHelper
    {
        public static bool IsFlagDefined(Enum e)
        {
            decimal d;
            return !decimal.TryParse(e.ToString(), out d);
        }
    }
}

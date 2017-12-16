using System;
using System.Collections.Generic;
using System.Text;

namespace CollegeUni.Utilities.Enumeration
{
    public enum RefreshConflict
    {
        StoreWins = 0,

        ClientWins = 1,

        MergeClientAndStore = 2,
    }

    public enum ResolveStrategy
    {
        StoreWins = RefreshConflict.StoreWins,
        ClientWins = RefreshConflict.ClientWins,
        ShowUnresolvedConflicts = 3,
    }
}

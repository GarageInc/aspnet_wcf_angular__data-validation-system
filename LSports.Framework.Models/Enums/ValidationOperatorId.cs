using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSports.Framework.Models.Enums
{
    public enum ValidationOperatorId
    {
        Equals = 1,
        NotEqual = 2,
        GreaterThan = 3,
        GreaterThanOrEqualTo = 4,
        LowerThan = 5,
        LowerThanOrEqualTo = 6,
        Contains = 7,
        NotContains = 8,
        StartsWith = 9,
        NotStartingWith = 10,
        EndsWith = 11,
        NotEndingWith = 12,
        Between = 13,
        NotBetween = 14,
        Regex = 15,
        Exists = 17,
        NotExist = 18,
        Empty = 19,
        NotEmpty = 20,
        PercentOfChangesMoreThan = 21,
        DifferenceBetweenTheLastMoreThan = 22,
        DifferenceBetweenTheLastLessThan = 23,
        DifferenceBetweenTheLastEquals = 24
    }

    public enum DatatypeId
    {
        Int = 1,
        Datetime = 2,
        Decimal = 3,
        Varchar = 4,
        Boolean = 5
    }
}

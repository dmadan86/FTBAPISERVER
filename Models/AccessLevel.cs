using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTBAPISERVER.Models
{
    [Flags]
    public enum AccessLevel
    {
        // The flag for SuperUser is 0001.
        SuperUser = 0x01,
        // The flag for ITUser is 0010.
        ITUser = 0x02,
        // The flag for Manager is 0100.
        Manager = 0x04,
        // The flag for General is 1000.
        General = 0x08,
    }
}
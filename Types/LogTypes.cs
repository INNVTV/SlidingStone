using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingStone
{
    public enum LogTypes
    {
        Default = 0,

        Platform = 1,
        Account = 2,
        User = 3,
        Error = 4,
        Object = 5
    }

    public enum ActivityTypes
    {
        Default = 0,

        User_Created = 1,
        Asset_Downloaded = 2,
        User = 3,
        Error = 4,
        Object = 5
    }

}

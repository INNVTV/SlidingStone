using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingStone
{
    public class SlidingStoneResponseType
    {
        public bool isSuccess { get; set; }

        public int successId { get; set; }
        public string successMessage { get; set; }

        public int errorId { get; set; }
        public string errorMessage { get; set; }

    }
}

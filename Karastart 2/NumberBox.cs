using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sintec.Tool
{
    public static class NumberBox
    {
        private static int progressiveNumber = 0;
        public static int giveMeANumber()
        {
            NumberBox.progressiveNumber = (NumberBox.progressiveNumber + 1) % int.MaxValue;
            return NumberBox.progressiveNumber;
        }
    }
}

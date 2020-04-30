using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Data
{
    public static class WaitHelper
    {

        public static async Task Wait(Func<bool> func)
        {
            var times = 10;
            var duration = 100;

            while (func() == false)
            {
                --times;

                await Task.Delay(duration).ConfigureAwait(false);
            }

            if (func() == false)
            {
                throw new InvalidOperationException("Func did not return true");
            }
        }
    }
}

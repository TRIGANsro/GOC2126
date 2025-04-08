using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDebug
{
    public class CrashGenerator : ITryCrash
    {
        private readonly Random generator = new();

        /// <summary>
        /// Tries to crash the application.
        /// </summary>
        /// <param name="crashType">Exception to throw</param>
        /// <param name="probability">Probability to Throw an exception, interval 0 to 100</param>
        /// <param name="timeout">Timeout in ms before try to crash, default is 100</param>
        public void TryCrash(Exception crashType, int probability, int timeout = 100)
        {
            // Implementation of the crash logic
            Task.Delay(timeout).Wait();

            int prob = Math.Max(0, Math.Min(100, probability));

            if (prob == 0)
            {
                return;
            }

            if (prob == 100)
            {
                throw crashType;
            }

            int randomValue = generator.Next(0, 100);

            if (randomValue < prob)
            {
                throw crashType;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDebug
{
    public interface ITryCrash
    {
        /// <summary>
        /// Tries to crash the application.
        /// </summary>
        /// <param name="crashType">Exception to throw</param>
        /// <param name="probability">Probability to Throw an exception</param>
        /// <param name="timeout">Timeout in ms before try to crash, default is 100</param>
        void TryCrash(Exception crashType, int probability, int timeout = 100);
    }
}

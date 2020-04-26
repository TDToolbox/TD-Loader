using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_Loader.Classes
{
    /// <summary>
    /// Contains common guard clauses and how to handle the errors
    /// </summary>
    class Guard
    {

        /// <summary>
        /// Checks if string is empty or null. Returns true if string is valid
        /// </summary>
        /// <param name="inputString">input string you are testing</param>
        /// <returns>bool whether or not string is valid</returns>
        public static bool IsStringValid(string inputString)
        {
            if (inputString == null)
            {
                Log.Output("input string is null");
                return false;
            }

            if (inputString == "")
            {
                Log.Output("input string is empty");
                return false;
            }

            return true;
        }
    }
}

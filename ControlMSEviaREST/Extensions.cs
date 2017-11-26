using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlMSEviaREST
{
    public static class Extensions
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<string> ParseInputArgs(this string input)
        {
            List<string> listOfArguments = new List<string>();

            try
            {
                listOfArguments = input.Split(',').ToList();
            }
            catch (Exception e)
            {
                log.Error("Error while parsing input parameters: " + e.Message);
            }

            return listOfArguments;
        }
    }
}

using ControlMSEviaREST;
using System;
using System.Collections.Generic;

namespace ControlMSE
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private static void Main(string[] args)
        {
            // copy build to: K:\vizrt\KV17\MSEController
            log.Info("Application Started");

            try
            {
                List<string> inputArgs = args[0].ParseInputArgs();

                string templateInstanceName; 
                string transitionType;
                string playlistTarget;
                string profileTarget;

                if (inputArgs[0].Length > 0) // viz template-instance Name
                {
                    templateInstanceName = inputArgs[0];
                }
                else
                {
                    templateInstanceName = ControlMSEviaREST.Properties.Settings.Default.TemplateInstanceName;
                }
                log.Info("TemplateInstanceName: " + templateInstanceName);


                if (inputArgs[1].Length > 0) // transition type
                {
                    transitionType = inputArgs[1];
                }
                else
                {
                    transitionType = "continue";
                }
                log.Info("Transition type: " + transitionType);

                playlistTarget = ControlMSEviaREST.Properties.Settings.Default.PlaylistTarget;
                profileTarget = ControlMSEviaREST.Properties.Settings.Default.ProfileTarget;

                log.Info("MSE Host: " + ControlMSEviaREST.Properties.Settings.Default.MSEHost);                
                log.Info("Playlist: " + playlistTarget);
                log.Info("Profile: " + profileTarget);

                QueryMSE myExample = new QueryMSE();

                //myExample.InitializeShow(playlistTarget, profileTarget, templateInstanceName); // TODO: propably not required ?

                myExample.TransitionElement(playlistTarget, templateInstanceName, profileTarget, transitionType);
            }
            catch (Exception e)
            {
                log.Error("Fatal error in application: " + e.Message);
            }

        }
    }
}

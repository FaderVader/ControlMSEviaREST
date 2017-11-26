using System;
using System.Net;
using System.Text;
using System.Xml;

namespace ControlMSEviaREST
{
    public class QueryMSE
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string mseHost = Properties.Settings.Default.MSEHost; // sets the MSE host

        private XmlNamespaceManager nameSpaceManager_;

        public QueryMSE()
        {
            nameSpaceManager_ = new XmlNamespaceManager(new NameTable());
            nameSpaceManager_.AddNamespace("viz", "http://www.vizrt.com/atom");
            nameSpaceManager_.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            nameSpaceManager_.AddNamespace("vdf", "http://www.vizrt.com/types");
            nameSpaceManager_.AddNamespace("app", "http://www.w3.org/2007/app");
            nameSpaceManager_.AddNamespace("vaext", "http://www.vizrt.com/atom-ext");           
        }

        private string GetValueFromDocumentByXPath(XmlDocument doc, string xpath)
        {
            var nav = doc.CreateNavigator();
            var it = nav.Select(xpath, nameSpaceManager_);
            if (it.MoveNext())
            {
                return it.Current.Value;
            }

            return "";
        }

        private string GetElementCollectionUri(string playlistName, string templateInstanceName)
        {
            string elementCollectionUri = "";

            try
            {
                // Get service document.
                var serviceDoc = LoadXMLFromUri(mseHost); 

                // Extract directory uri.
                string directoryUri = GetValueFromDocumentByXPath(serviceDoc,
                    "//app:service/app:workspace/app:collection[app:categories/atom:category[@term='directory']]/@href");


                // Get directory document.
                var directoryDoc = LoadXMLFromUri(directoryUri);

                // Extract directory/shows uri.
                //string directoryShowUri = GetValueFromDocumentByXPath(directoryDoc,
                //    "//atom:feed/atom:entry[atom:category[@term='directory'] and atom:title[text()='shows']]/atom:link[@rel='alternate']/@href");

                // Extract directory/playlists uri.
                string directoryPlaylistUri = GetValueFromDocumentByXPath(directoryDoc,
                    "//atom:feed/atom:entry[atom:category[@term='directory'] and atom:title[text()='playlists']]/atom:link[@rel='alternate']/@href");

                // Get directory/shows document.
                var directoryPlaylistDoc = LoadXMLFromUri(directoryPlaylistUri);

                // Extract show uri from the given showName.
                string myPlaylistUri = GetValueFromDocumentByXPath(directoryPlaylistDoc,
                    "//atom:feed/atom:entry[atom:title[text()='" + playlistName + "']]/atom:link[@rel='alternate']/@href");

                // Get show document.
                var myShowDoc = LoadXMLFromUri(myPlaylistUri);

                //// Extract element collection uri.
                //string elementCollectionUri = GetValueFromDocumentByXPath(myShowDoc,
                //    "//atom:feed/atom:entry[atom:category[@term='element_collection']]/atom:link[@rel='alternate']/@href");

                // Extract element collection uri.   // modded!                
                 elementCollectionUri = GetValueFromDocumentByXPath(myShowDoc,
                   "//atom:feed/atom:entry[atom:title[text()='" + templateInstanceName + "']]/atom:link[@rel='alternate']/@href");
            }
            catch (Exception e)
            {
                log.Error("Error in GetElementCollectionUri: " + e.Message);
            }

            return elementCollectionUri;
        }

        private string GetElementEntryUri(string playlistName, string entryName)
        {
            string elementEntryUri = "";

            try
            {
                // Get service document.
                var serviceDoc = LoadXMLFromUri(mseHost);

                // Extract directory uri.
                string directoryUri = GetValueFromDocumentByXPath(serviceDoc,
                    "//app:service/app:workspace/app:collection[app:categories/atom:category[@term='directory']]/@href");

                // Get directory document.
                var directoryDoc = LoadXMLFromUri(directoryUri);

                // Extract directory/shows uri.
                //string directoryShowUri = GetValueFromDocumentByXPath(directoryDoc,
                //    "//atom:feed/atom:entry[atom:category[@term='directory'] and atom:title[text()='shows']]/atom:link[@rel='alternate']/@href");

                // Extract directory/playlists uri.
                string directoryPlaylistUri = GetValueFromDocumentByXPath(directoryDoc,
                    "//atom:feed/atom:entry[atom:category[@term='directory'] and atom:title[text()='playlists']]/atom:link[@rel='alternate']/@href");


                // Get directory/shows document.
                var directoryPlaylistDoc = LoadXMLFromUri(directoryPlaylistUri);

                // Extract show uri from the given showName.
                string myPlaylistUri = GetValueFromDocumentByXPath(directoryPlaylistDoc,
                    "//atom:feed/atom:entry[atom:title[text()='" + playlistName + "']]/atom:link[@rel='alternate']/@href");


                // Get playlist document.
                var myPlaylistDoc = LoadXMLFromUri(myPlaylistUri);


                // Extract element collection uri. Original
                //string elementCollectionUri = GetValueFromDocumentByXPath(myShowDoc,
                //    "//atom:feed/atom:entry[atom:category[@term='element_collection']]/atom:link[@rel='alternate']/@href");

                // Extract element collection uri. Modded
                string elementCollectionUri = GetValueFromDocumentByXPath(myPlaylistDoc,
                       "//atom:feed/atom:entry[atom:title[text()='" + entryName + "']]/atom:link[@rel='alternate']/@href");

                elementCollectionUri = elementCollectionUri.Substring(0, elementCollectionUri.Length - 1);
                log.Info("Got elementCollectionUri: " + elementCollectionUri);

                // Get element collection document.
                var elementCollectionDoc = LoadXMLFromUri(elementCollectionUri);

                // Extract element entry uri from the given entryName.
                elementEntryUri = GetValueFromDocumentByXPath(elementCollectionDoc,
                    "//atom:entry[atom:title[text()='" + entryName + "']]/atom:link[@rel='self']/@href");
                log.Info("Got elementEntryUri: " + elementEntryUri);
            }
            catch (Exception e)
            {
                log.Error("Error while getting elementUri: " + e.Message);
            }

            return elementEntryUri;
        }

        private string GetProfileCommandUri(string profileName, string command)
        {
            string profileCommandUri = "";

            try
            {
                // Get service document.
                var serviceDoc = LoadXMLFromUri(mseHost);  

                // Extract profile collection uri.
                string profileCollectionUri = GetValueFromDocumentByXPath(serviceDoc,
                        "//app:service/app:workspace/app:collection[app:categories/atom:category[@term='profile']]/@href");

                // Get profile collection document.
                var profileCollectionDoc = LoadXMLFromUri(profileCollectionUri);

                // Extract profile entry uri from the given profileName.
                string profileEntryUri = GetValueFromDocumentByXPath(profileCollectionDoc,
                    "//atom:feed/atom:entry[atom:title[text()='" + profileName + "']]/atom:link[@rel='self']/@href");

                // Get profile entry document.
                var profileEntryDoc = LoadXMLFromUri(profileEntryUri);

                // Extract profile command uri from the given command.
                profileCommandUri = GetValueFromDocumentByXPath(profileEntryDoc,
                    "//atom:entry/atom:link[@rel='" + command + "']/@href");

            }
            catch (Exception e)
            {
                log.Error("Error while getting profile: " + e.Message);
            }

            return profileCommandUri;
        }

        private HttpWebResponse PostOnUri(string uri, string body)
        {
            try
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

                var client = (HttpWebRequest)HttpWebRequest.Create(uri);
                client.AllowAutoRedirect = false;
                client.AllowWriteStreamBuffering = false;

                client.Method = "POST";
                client.ContentType = "text/plain";
                client.ContentLength = bodyBytes.Length;

                client.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);

                WebResponse response = client.GetResponse();

                log.Info("MSE Host response to POST: " + response.ToString());   

                return (HttpWebResponse)response;
            }
            catch (WebException e)
            {
                log.Error("Error while POSTing: " + e.Response.ToString());              
                return (HttpWebResponse)e.Response;
            }
        }

        public void InitializeShow(string playlistName, string profileName, string templateInstanceName) // TODO: do we need this ?
        {
            try
            {
                string elementCollectionUri = GetElementCollectionUri(playlistName, templateInstanceName);
                string profileCommandUri = GetProfileCommandUri(profileName, "initialize");

                // POST on profile command uri with the collectionUri as a body.
                HttpWebResponse response = PostOnUri(profileCommandUri, elementCollectionUri);
                Console.WriteLine("Response status : " + response.StatusCode + "\n");
                log.Info("Response status : " + response.StatusCode);
            }
            catch (Exception e)
            {
                log.Error("Error while initializing: " + e.Message);
            }
        }

        public void TransitionElement(string showName, string entryName, string profileName, string transition)
        {
            try
            {
                string elementEntryUri = GetElementEntryUri(showName, entryName);
                
                string profileCommandUri = GetProfileCommandUri(profileName, transition);
                
                // POST on profile command uri with the entryUri as a body.
                HttpWebResponse response = PostOnUri(profileCommandUri, elementEntryUri);
                Console.WriteLine("Response status : " + response.StatusCode + "\n");
                log.Info("Response status: " + response.StatusCode);
            }

            catch (Exception e)
            {
                log.Error("Error while sending transition to Viz: " + e.Message);
            }
        }

        private XmlDocument LoadXMLFromUri(string uri)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(uri);
            }
            catch (Exception e)
            {
                log.Error("Error while loading xml: " + e.Message);
            }

            return doc;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Util.Store;

namespace PGA.Updater
{
    class Authentication
    {
        public void GetAuthenticated()
        {

            //Scopes for use with the Google Drive API
            string[] scopes = new string[] { DriveService.Scope.Drive,
                                             DriveService.Scope.DriveFile};
            var clientId = "[Client ID]";      // From https://console.developers.google.com
            var clientSecret = "xxx";          // From https://console.developers.google.com
            // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = clientId,
                                                                                             ClientSecret = clientSecret},
                                                            scopes,
                                                            Environment.UserName,
                                                            CancellationToken.None,
                                                            new FileDataStore("Daimto.GoogleDrive.Auth.Store")).Result;


        }

    }
}

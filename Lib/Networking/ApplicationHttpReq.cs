using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Lib.Exceptions;
using Sokoban.Model;
using Sokoban.Interfaces;
using Sokoban.Lib.Http;
using System.Net;

namespace Sokoban.Networking
{
    public class ApplicationHttpReq
    {
        public static string LastError = "";
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">i.e., "/remote/GetInitLeagues/"</param>
        /// <returns>Response of the server</returns>
        public static string GetRequestOnServer(string request, IProfileRepository profile, string module, IErrorMessagesPresenter errorPresenter)
        {
            if (profile == null) throw new UninitializedException("Server name was not initialized.");

            LastError = "";
            string output;

            if (profile.Server == String.Empty)
            {
                output = "error";
                LastError = "User is not logged in.";
                errorMessage(errorPresenter, ErrorMessageSeverity.Low, module, "User is not logged in. Please log in and click on 'Refresh'");
            }
            else
            {

                string url = profile.Server.TrimEnd(new char[] { '/' }) + request;

                try
                {
                    output = HttpReq.GetRequest(url, "");
                }
                catch (WebException e)
                {
                    output = "error";
                    LastError = "Error in communication with the server. Please try again in a while.";
                    errorMessage(errorPresenter, ErrorMessageSeverity.Medium, module, "Error in communication with the server. Additional information: " + e.Message);
                }
                catch (Exception e)
                {
                    output = "error";
                    LastError = "Unknown error occured. Please try again in a while.";
                    errorMessage(errorPresenter, ErrorMessageSeverity.High, module, "Unknown error in communication with the server. Additional information: " + e.Message);
                }

            }

            return output;
        }

        private static void errorMessage(IErrorMessagesPresenter errorPresenter, ErrorMessageSeverity ems, string module, string message)
        {
            if (errorPresenter != null)
            {
                errorPresenter.ErrorMessage(ems, module, message);
            }
        }
    }
}

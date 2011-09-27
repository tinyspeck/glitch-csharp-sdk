using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlitchSDK
{
    public class Glitch
    {
        #region Constants

        public const String BASE_URL = "http://api.glitch.com"; // Base service URL

        #endregion


        #region Public Properties

        public String accessToken; // Access token for the currently logged in user

        #endregion


        #region Constructors

        // Constructor for Glitch object
        // Access token should be obtained by the developer before instantiating this
        // To get assistance with retrieving an access token, please follow the authentication tutorial
        // at http://developer.glitch.com/docs/auth/
        public Glitch(String accessToken)
        {
            if (String.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Please specify your access token when initializing a Glitch object");
            }

            this.accessToken = accessToken;
        }

        #endregion


        #region Interacting with the API

        public GlitchRequest GetRequest(String method)
        {
            return GetRequest(method, null);
        }
    
        public GlitchRequest GetRequest(String method, Dictionary<String,String> parameters)
        {
            return new GlitchRequest(method, parameters, this);
        }

        #endregion
    }
}

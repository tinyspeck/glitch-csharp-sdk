using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;

namespace GlitchSDK
{
    public class GlitchRequest
    {
        #region Constants

        public const String API_URL = Glitch.BASE_URL + "/simple/";

        #endregion
        

        #region Public Properties

        public String url; // Full url for request, e.g. "http://api.glitch.com/simple/players.info"
	    public String method; // Specific method without 'simple', e.g. "players.info"
	    public Dictionary<String,String> parameters; // Dictionary of parameters passed in the request
        public IGlitchRequester requester; // Requester that will be called when events occur before, during, and after the request
	    public object response; // JSON response object

        #endregion


        #region Private Properties

        private HttpWebRequest request; // Async request that interacts with API
        private Glitch glitch; // Glitch parent instance

        #endregion


        #region Constructors

        public GlitchRequest(String startMethod, Dictionary<String, String> startParams, Glitch startGlitch)
        {
            method = startMethod;
            parameters = startParams;
            glitch = startGlitch;
        }

        public GlitchRequest(String startMethod, Glitch startGlitch)
        {
            method = startMethod;
            parameters = null;
            glitch = startGlitch;
        }

        #endregion


        #region Interacting with the API

        // Call this to execute your request
	    // Pass in the delegate which will be called when events occur that are related to this object
        public void Execute(IGlitchRequester glitchRequester)
        {
            // Set local IGlitchRequester for callbacks
            requester = glitchRequester;

            // Concatenate the full url
            String fullUrl = API_URL + method;

            // If we don't have parameters, create the dictionary
            if (parameters == null)
            {
                parameters = new Dictionary<String, String>();
            }

            // Add access token to dictionary
            if (glitch.accessToken != null)
            {
                parameters.Add("oauth_token", glitch.accessToken);
            }

            // Serialize our full url with our parameters
            fullUrl = SerializeURL(fullUrl, parameters);

            // Create the request and set its method
            request = HttpWebRequest.Create(fullUrl) as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Get;

            // Begin the response with the associated callback
            request.BeginGetResponse(r =>
                {
                    // Callback
                    // r is the IAsyncResult

                    // Get the associated Glitch Request object
                    GlitchRequest glitchRequest = r.AsyncState as GlitchRequest;

                    // Attempt to parse the response to check for ok
                    try
                    {
                        // Get the associated response object
                        HttpWebResponse httpWebResponse = request.EndGetResponse(r) as HttpWebResponse;

                        // Read the response into a string
                        StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
                        String result = reader.ReadToEnd();
                        reader.Dispose();

                        // Serialize the response string into an object
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        object response = serializer.DeserializeObject(result);

                        // Check the type before checking for ok
                        if (response.GetType().Equals(typeof(Dictionary<String, Object>)))
                        {
                            // Cast to a dictionary
                            Dictionary<String, Object> responseDict = response as Dictionary<String, Object>;
                            
                            // Declare ok object
                            object ok;

                            // Attempt to get the ok value
                            if (responseDict.TryGetValue("ok", out ok))
                            {
                                // If ok is an int
                                if (ok.GetType().Equals(typeof(int)))
                                {
                                    // Check if it is 1, which means the request was ok
                                    if ((int)ok == 1)
                                    {
                                        // Set the response for the request
                                        glitchRequest.response = response;

                                        // Call the requester with the request
                                        requester.RequestFinished(glitchRequest);

                                        return;
                                    }
                                }

                                // If the service did not return ok or did not return ok = 1
                                requester.RequestFailed(glitchRequest, new Exception("Service did not return OK"));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // If there was an exception somewhere in the parsing process, send it back to the requester
                        requester.RequestFailed(glitchRequest, ex);
                    }

                    // If we get to this point, we didn't encounter an exception but we didn't get a valid parsed response
                    requester.RequestFailed(glitchRequest, new Exception("An unknown error occurred with the request!"));
                }, this);
        }

        // Cancel the pending request
        public void Cancel()
        {
            // Abort the request
            if (request != null)
            {
                request.Abort();
            }
        }

        #endregion


        #region URL Helper Methods

        public static String SerializeURL(String url, Dictionary<String,String> parameters)
        {
    	    url = url + "?";
    	
    	    url = url + SerializeParams(parameters);
    	
    	    return url;
        }
    
        public static String SerializeParams(Dictionary<String,String> parameters)
        {
    	    String serializedParameters = "";
    	
    	    if (parameters != null && parameters.Count > 0)
    	    {    		
    		    foreach (String key in parameters.Keys)
    		    {
    			    serializedParameters = serializedParameters + "&" + key + "=" + parameters[key];
    		    }

			    // Remove extra ampersand from front
                serializedParameters = serializedParameters.Substring(1);
    	    }

            return serializedParameters;
        }

        #endregion
    }
}

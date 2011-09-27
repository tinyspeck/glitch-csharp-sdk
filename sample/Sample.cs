using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlitchSDK;
using System.Threading;

namespace GlitchSample
{
    class Sample : IGlitchRequester // Implement IGlitchRequester, which will receive callback events from the async request
    {
        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            // Create new instance of class
            Sample sample = new Sample();

            // Set our access token
            // It is up to you to get this yourself            
            // To get assistance with retrieving an access token, please follow the authentication tutorial
            // at http://developer.glitch.com/docs/auth/

            string accessToken = ""; // SET ACCESS TOKEN HERE

            // Instantiate our Glitch class, from which you will interact with the APIs
            Glitch glitch = new Glitch(accessToken);

            // Get a request with a specific method
            GlitchRequest request = glitch.GetRequest("players.fullInfo", new Dictionary<string, string>
                {
                    // Parameters go here
                    {"player_tsid", "PIF12K4LV4D1FCG"}
                });

            // Execute the request!
            request.Execute(sample);

            resetEvent.WaitOne(); // Hold the program until the async call completes
        }


        #region IGlitchRequester Interface Methods

        // Request completed
        public void RequestFinished(GlitchRequest request)
        {
            Console.WriteLine("Request Finished!");

            // Null checks and type check
            if (request != null && request.response != null &&
                    request.response.GetType().Equals(typeof(Dictionary<String, Object>)))
            {
                // Cast to dictionary
                Dictionary<String, Object> response = request.response as Dictionary<String, Object>;

                // Declare playerName to be set later
                object playerName;

                // Try to get player_name from the dictionary
                if (response.TryGetValue("player_name", out playerName))
                {
                    // Check if it is a string before writing it to the console
                    if (playerName.GetType().Equals(typeof(String)))
                    {
                        Console.WriteLine("Hello " + playerName as String);
                    }                    
                }
            }            

            resetEvent.Set(); // Allow the program to exit
        }

        // Request failed with associated exception
        public void RequestFailed(GlitchRequest request, Exception exception)
        {
            resetEvent.Set(); // Allow the program to exit

            if (exception != null)
            {
                throw exception; // Throw exception, handle this how you want
            }
        }

        #endregion
    }
}

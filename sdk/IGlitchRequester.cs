using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlitchSDK
{
    public interface IGlitchRequester
    {
        // Called when a request is completed
        // Check the method via request.method and response via request.response
        void RequestFinished(GlitchRequest request);

        // Called when a request fails
        void RequestFailed(GlitchRequest request, Exception exception);
    }
}

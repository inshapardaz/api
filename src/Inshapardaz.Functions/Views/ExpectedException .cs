using System;
using System.Net;
using System.Net.Http;

namespace Inshapardaz.Functions.Views
{
    public class ExpectedException : Exception
    {
        public ExpectedException(HttpStatusCode code, string message = "")
            : base(message)
        {
            Code = code;
        }

        public HttpStatusCode Code { get; }

        public HttpResponseMessage CreateErrorResponseMessage(HttpRequestMessage request)
        {
            var result = request.CreateErrorResponse(Code, Message);
            ApplyResponseDetails(result);
            return result;
        }

        protected virtual void ApplyResponseDetails(HttpResponseMessage response)
        {
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public class RequestBuilder
    {
        private Dictionary<string, StringValues> parameters = new Dictionary<string, StringValues>();

        private string _contents;

        public RequestBuilder WithQueryParameter(string name, object value)
        {
            parameters.Add(name, value.ToString());
            return this;
        }
            
        public RequestBuilder WithBody(string contents)
        {
            _contents = contents;
            return this;
        }

        public DefaultHttpRequest Build()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());

            if (parameters.Count > 0)
            {
                request.Query = new QueryCollection(parameters);
            }

            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(_contents ?? ""));
            return request;
        }

        public DefaultHttpRequest BuildImageUpload()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());

            if (parameters.Count > 0)
            {
                request.Query = new QueryCollection(parameters);
            }

            request.Body = new MemoryStream(new byte[10]);


            return request;
        }
    }
}

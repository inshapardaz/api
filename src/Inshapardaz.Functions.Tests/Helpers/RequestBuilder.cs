using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Bogus;
using Newtonsoft.Json;
using System;

namespace Inshapardaz.Functions.Tests.Helpers
{
    public class RequestBuilder
    {
        private readonly Dictionary<string, StringValues> _parameters = new Dictionary<string, StringValues>();

        private string _contents;

        private byte[] _file;
        private string _acceptType;

        public RequestBuilder WithQueryParameter(string name, object value)
        {
            _parameters.Add(name, value.ToString());
            return this;
        }

        public RequestBuilder WithJsonBody(object data)
        {
            _contents = JsonConvert.SerializeObject(data);
            return this;
        }

        public RequestBuilder WithBody(string contents)
        {
            _contents = contents;
            return this;
        }

        public RequestBuilder WithImage()
        {
            _file = new Faker().Image.Random.Bytes(20);
            return this;
        }

        internal RequestBuilder WithBytes(byte[] binaryContent)
        {
            _file = binaryContent;
            return this;
        }

        internal RequestBuilder WithAcceptType(string acceptType)
        {
            _acceptType = acceptType;
            return this;
        }

        public DefaultHttpRequest Build()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());

            if (!string.IsNullOrEmpty(_acceptType))
            {
                request.Headers.Add("Content-Type", _acceptType);
            }

            if (_parameters.Count > 0)
            {
                request.Query = new QueryCollection(_parameters);
            }

            request.Body = _file != null ?
                                    new MemoryStream(_file) :
                                    new MemoryStream(Encoding.UTF8.GetBytes(_contents ?? ""));
            return request;
        }

        public HttpRequestMessage BuildRequestMessage()
        {
            var request = new HttpRequestMessage();
            if (_file != null)
            {
                request.Content = CreateMultiPartImageData(_file);
            }
            else
            {
                request.Content = CreateMultiPartStringData(_contents ?? "");
            };

            return request;
        }

        private static MultipartFormDataContent CreateMultiPartStringData(string data)
        {
            var stringContent = new StringContent(data, Encoding.UTF8);

            return new MultipartFormDataContent { stringContent };
        }

        private static MultipartFormDataContent CreateMultiPartImageData(byte[] data, string mimeType = "image/jpeg")
        {
            ByteArrayContent byteContent = new ByteArrayContent(data);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            byteContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = new Faker().System.FileName("jpg") };
            return new MultipartFormDataContent { byteContent };
        }
    }

    public static class RequestExtentions
    {
        public static DefaultHttpRequest ToRequest(this object body) =>
            new RequestBuilder().WithJsonBody(body).Build();
    }
}

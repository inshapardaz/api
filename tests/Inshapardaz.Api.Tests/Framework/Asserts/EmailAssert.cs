using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Fakes;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class EmailAssert
    {
        private readonly FakeSmtpClient.EmailMessage _message;

        public EmailAssert(FakeSmtpClient.EmailMessage message)
        {
            _message = message;
        }

        internal EmailAssert WithSubject(string subject)
        {
            _message.Subject.Should().Be(subject, "Subject not matching");
            return this;
        }

        internal EmailAssert WithBodyContainting(string text)
        {
            _message.Body.Should().Contain(text, "Body not matching");
            return this;
        }
    }
}

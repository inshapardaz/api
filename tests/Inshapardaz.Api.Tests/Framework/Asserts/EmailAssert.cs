using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Fakes;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class EmailAssert(FakeSmtpClient.EmailMessage message)
    {
        internal EmailAssert WithSubject(string subject)
        {
            message.Subject.Should().Be(subject, "Subject not matching");
            return this;
        }

        internal EmailAssert WithBodyContainting(string text)
        {
            message.Body.Should().Contain(text, "Body not matching");
            return this;
        }
    }
}

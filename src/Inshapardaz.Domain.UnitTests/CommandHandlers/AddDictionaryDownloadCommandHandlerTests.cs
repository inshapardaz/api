using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    [TestFixture]
    public class AddDictionaryDownloadCommandHandlerTests
    {
        private readonly AddDictionaryDownloadCommandHandler _handler;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient;

        public AddDictionaryDownloadCommandHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _backgroundJobClient = new Mock<IBackgroundJobClient>();
            _backgroundJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>())).Returns("34");

            _handler = new AddDictionaryDownloadCommandHandler(_backgroundJobClient.Object);
        }
        
        [Test]
        public async Task WhenAdded_ShouldSaveToDatabase()
        {
            var name = "Test";
            var response = await _handler.HandleAsync(new AddDictionaryDownloadCommand
            {
                DitionarayId = 4,
                DownloadType = "something"
            });

            response.JobId.ShouldBe("34");
            _backgroundJobClient.VerifyAll();
        }
    }
}
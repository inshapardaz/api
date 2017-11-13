﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    [TestFixture]
    public class DeleteDictionaryCommandHandlerTests : DatabaseTest
    {
        private readonly DeleteDictionaryCommandHandler _handler;

        public DeleteDictionaryCommandHandlerTests()
        {
            DbContext.Dictionary.Add(new Dictionary {Id = 1, IsPublic = true });
            DbContext.Dictionary.Add(new Dictionary {Id = 2, IsPublic = true });
            DbContext.Dictionary.Add(new Dictionary {Id = 3, IsPublic = false });
            DbContext.Dictionary.Add(new Dictionary {Id = 4, IsPublic = false });
            DbContext.SaveChanges();

            _handler = new DeleteDictionaryCommandHandler(DbContext);
        }

        [Test]
        public async Task WhenRemovedPrivateDictionary_ShouldDeleteFromDatabase()
        {
            await _handler.HandleAsync(new DeleteDictionaryCommand(3));

            DbContext.Dictionary.SingleOrDefault(d => d.Id == 3).ShouldBeNull();
        }

        [Test]
        public async Task IfDictionaryIsNotFound_ShouldThrowException()
        {
            await _handler.HandleAsync(new DeleteDictionaryCommand(7))
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}
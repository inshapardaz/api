﻿namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class BookShelfDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public long? ImageId { get; set; }

        public int AccountId { get; set; }
        public int LibraryId { get; set; }
    }
}

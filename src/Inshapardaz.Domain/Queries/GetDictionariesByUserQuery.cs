﻿using System;
using System.Collections.Generic;

using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionariesByUserQuery : IQuery<IEnumerable<Dictionary>>
    {
        public Guid UserId { get; set; }
    }
}
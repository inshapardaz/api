using Inshapardaz.Database.Dto.Library;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inshapardaz.Database
{
    public static class Mappers
    {
        public static Page<Author> Map(this IEnumerable<AuthorDto> source, int pageNumber, int pageSize, int totalCount) => 
              new Page<Author>
              {
                  Data = source.Select(Map),
                  PageNumber = pageNumber,
                  PageSize = pageSize,
                  TotalCount = totalCount
              };
        public static Author Map(this AuthorDto source) => null;
    }
}

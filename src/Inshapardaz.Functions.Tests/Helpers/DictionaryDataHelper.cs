using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Tests.Helpers
{
    internal class DictionaryDataHelper
    {
        private readonly IDatabaseContext _context;

        public DictionaryDataHelper(IDatabaseContext context)
        {
            _context = context;
        }

        internal Dictionary GetDictionaryByid(int id) =>
            _context.Dictionary.SingleOrDefault(d => d.Id == id);
    }
}

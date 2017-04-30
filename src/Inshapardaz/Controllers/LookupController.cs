using System;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    public class LookupController : Controller
    {
        [HttpGet]
        [Route("api/languages", Name = "GetLanguages")]
        public IEnumerable<KeyValuePair<string, int>> GetLanguages()
        {
            return Enum.GetValues(typeof(Languages))
                .Cast<Languages>()
                .Select(lang => new KeyValuePair<string, int>(Enum.GetName(typeof(Languages), lang), (int)lang))
                .ToList();
        }

        [HttpGet]
        [Route("api/attributes", Name = "GetAttributes")]
        public IEnumerable<KeyValuePair<string, int>> GetAttributes()
        {
            return Enum.GetValues(typeof(GrammaticalType))
                .Cast<GrammaticalType>()
                .Select(type => new KeyValuePair<string, int>(Enum.GetName(typeof(GrammaticalType), type), (int)type))
                .ToList();
        }

        [HttpGet]
        [Route("api/relationtypes", Name = "GetRelationTypes")]
        public IEnumerable<KeyValuePair<string, int>> GetRelationTypes()
        {
            return Enum.GetValues(typeof(RelationType))
                .Cast<RelationType>()
                .Select(relation => new KeyValuePair<string, int>(Enum.GetName(typeof(RelationType), relation), (int)relation))
                .ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    public class LookupController : Controller
    {
        [HttpGet]
        [Route("api/languages", Name = "GetLanguages")]
        [Produces(typeof(IEnumerable<KeyValuePair<string, int>>))]
        public IActionResult GetLanguages()
        {
             var result = Enum.GetValues(typeof(Languages))
                .Cast<Languages>()
                .Select(lang => new KeyValuePair<string, int>(Enum.GetName(typeof(Languages), lang), (int)lang))
                .ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("api/attributes", Name = "GetAttributes")]
        [Produces(typeof(IEnumerable<KeyValuePair<string, int>>))]        
        public IActionResult GetAttributes()
        {
            var result =  Enum.GetValues(typeof(GrammaticalType))
                .Cast<GrammaticalType>()
                .Select(type => new KeyValuePair<string, int>(Enum.GetName(typeof(GrammaticalType), type), (int)type))
                .ToList();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/relationtypes", Name = "GetRelationTypes")]
        [Produces(typeof(IEnumerable<KeyValuePair<string, int>>))]                
        public IActionResult GetRelationTypes()
        {
            var result =  Enum.GetValues(typeof(RelationType))
                .Cast<RelationType>()
                .Select(relation => new KeyValuePair<string, int>(Enum.GetName(typeof(RelationType), relation), (int)relation))
                .ToList();
            return Ok(result);
        }
    }
}

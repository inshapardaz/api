using Inshapardaz.Model;
using Inshapardaz.Renderers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Controllers
{
    public class EntryController : Controller
    {
        private readonly IRenderResponse<EntryView> _entryRenderer;

        private readonly IRenderResponse<DictionaryEntryView> _dictionaryEntryResponse;

        private readonly IRenderResponse<IndexView> _indexRenderer;

        public EntryController(IRenderResponse<EntryView> entryRenderer, IRenderResponse<DictionaryEntryView> dictionaryEntryResponse, IRenderResponse<IndexView> indexRenderer)
        {
            _entryRenderer = entryRenderer;
            _dictionaryEntryResponse = dictionaryEntryResponse;
            _indexRenderer = indexRenderer;
        }

        [HttpGet("api/entry", Name = "Entry")]
        public EntryView Get()
        {
            return _entryRenderer.Render();
        }

        [HttpGet("api/dictionary", Name = "DictionaryEntry")]
        public DictionaryEntryView EntryDictionary()
        {
            return _dictionaryEntryResponse.Render();
        }

        [HttpGet("api/dictionary/index", Name = "DictionaryIndex")]
        public IndexView DictionaryIndex()
        {
            return _indexRenderer.Render();
        }

        [Authorize]
        [HttpGet]
        [Route("ping/secure")]
        public string PingSecured()
        {
            return "All good. You only get this message if you are authenticated.";
        }
    }
}

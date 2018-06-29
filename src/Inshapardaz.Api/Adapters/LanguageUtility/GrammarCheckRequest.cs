
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.GrammarParser;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Dictionary;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Api.Adapters.LanguageUtility
{
    public class GrammarCheckRequest : IRequest
    {
        public Guid Id { get; set; }
        public string Paragraph { get; internal set; }
        public IEnumerable<SpellCheckResultView> Response { get; internal set; }
    }

    public class GrammarCheckRequestHandler : RequestHandlerAsync<GrammarCheckRequest>
    {
        private readonly IWordRepository _wordRepository;
        private readonly Settings _settings;
        private readonly ILogger<SpellCheckRequestHandler> _logger;
        private int _dictionaryId = 0;
        public GrammarCheckRequestHandler(IWordRepository wordRepository,
                                        Settings settings,
                                        ILogger<SpellCheckRequestHandler> logger)
        {
            _wordRepository = wordRepository;
            _settings = settings;
            _logger = logger;
        }



        public override async Task<GrammarCheckRequest> HandleAsync(GrammarCheckRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            _dictionaryId = _settings.DefaultDictionaryId;
            
            if (_dictionaryId == 0)
            {
                throw new Exception("Default dictionary not found");
            }

            await ParseGrammarAsync(request.Paragraph, cancellationToken);
            return await base.HandleAsync(request, cancellationToken);
        }

        private async Task ParseGrammarAsync(string paragraph, CancellationToken cancellationToken)
        {
            var sentences = paragraph.SplitIntoSentences();
            foreach (var s in sentences)
            {
                await CheckSentence(s, cancellationToken);
            }
        }

        private async Task<bool> CheckSentence(string sentence, CancellationToken cancellationToken)
        {
            var tokens = await Tokenise(sentence, cancellationToken);
            if (IsAmrONahi(tokens))
            {
                // TODO : Add success
                return true;
            }

            if (IsMurakbat(tokens))
            {
                var next = tokens.Pop();
                if (next.IsAlamatNafi())
                {
                    var next2 = tokens.Pop();
                    if (next2.IsFealNaqis())
                    {
                        if (!tokens.Any())
                        {
                            // TODO : Add success
                            return true;
                        }
                    }
                }
                else
                {
                    tokens.Push(next);
                    var next2 = tokens.Pop();
                    if (next2.IsFealNaqis())
                    {
                        if (!tokens.Any())
                        {
                            // TODO : Add success
                            return true;
                        }
                    }
                }
            }

            if (IsJumlaFealea(tokens))
            {
                if (!tokens.Any())
                {
                    // TODO : Add success
                    return true;
                }
            }

            // TODO : Add failure
            return false;
        }

        private async Task<Stack<Word>> Tokenise(string sentence, CancellationToken cancellationToken)
        {
            var words = sentence.TrimSpecialCharacters().SplitIntoSentences();

            return new Stack<Word>(await _wordRepository.GetWordsByTitles(_dictionaryId, words, cancellationToken));
        }
        
        private bool IsAmrONahi(Stack<Word> tokens)
        {
            int state = 1;

            var token = tokens.Pop();
            while (token != null)
            {
                switch (state)
                {
                    case 1:
                        if (token.IsHarfENida())
                            state = 2;
                        else if (IsMurakbat(tokens))
                            state = 3;
                        else if (token.IsIsm())
                            state = 3;
                        else if (token.IsHarfETakeed())
                            state = 4;
                        else if (token.IsAlamatNafi())
                            state = 5;
                        else if (token.IsAmr())
                            state = 6;
                        else
                            return false;
                        break;
                    case 2:
                        if (IsMurakbat(tokens))
                            state = 3;
                        else if (token.IsIsm())
                            state = 3;
                        else if (token.IsHarfETakeed())
                            state = 4;
                        else if (token.IsAlamatNafi())
                            state = 5;
                        else if (token.IsAmr())
                            state = 6;
                        else
                            return false;
                        break;
                    case 3:
                        if (IsMurakbat(tokens))
                            state = 7;
                        else if (token.IsIsm() || token.IsAlamatMafool())
                            state = 7;
                        else if (token.IsHarfETakeed())
                            state = 4;
                        else if (token.IsAlamatNafi())
                            state = 5;
                        else if (token.IsAmr())
                            state = 6;
                        else
                            return false;
                        break;
                    case 4:
                        if (token.IsAlamatNafi())
                            state = 5;
                        else if (token.IsAmr())
                            state = 6;
                        else
                            return false;
                        break;
                    case 5:
                        if (token.IsAmr())
                            state = 6;
                        else
                            return false;
                        break;
                    case 6:
                        if (token.IsAmr())
                            state = 8;
                        else
                            return false;
                        break;
                    case 7:
                        if (IsMurakbat(tokens))
                            state = 7;
                        else if (token.IsIsm())
                            state = 7;
                        else if (token.IsHarfETakeed())
                            state = 4;
                        else if (token.IsAlamatNafi())
                            state = 5;
                        else if (token.IsAmr())
                            state = 6;
                        else
                            return false;
                        break;
                    case 8:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        private bool IsMurakbat(Stack<Word> tokens)
        {
            int state = 1;

            var token = tokens.Pop();
            while (token != null)
            {
                switch (state)
                {
                    case 1:
                        if (token.IsIsmSift())
                            state = 2;
                        else if (token.IsIsm())
                            state = 3;
                        else if (token.IsIsmIshara())
                            state= 4;
                        else
                            return false;
                        break;
                    case 2:
                        if (token.IsIsmSift())
                            state= 2;
                        else if (token.IsIsm())
                            state= 5;
                        else if (token.IsHarfEJar())
                            state= 6;
                        else if (token.IsHarf())
                            state= 7;
                        else
                        {
                            tokens.Push(token);
                            // eturn MURAKAB_TOSEEFI;
                            return true;
                        }
                        break;
                    case 3:
                        if (token.IsIsmSift())
                            state = 2;
                        else if (token.IsIsm())
                            state = 8;
                        else if (token.IsHarfEJar())
                            state = 6;
                        else if (token.IsHarf())
                            state = 7;
                        else
                            return false;
                        break;
                    case 4:
                        if (token.IsIsmSift())
                            state= 2;
                        else if (token.IsIsm())
                            state= 8;
                        else
                            return false;
                        break;
                    case 5:
                        if (token.IsIsmSift())
                            state= 2;
                        else if (token.IsIsm())
                            state= 5;
                        else if (token.IsHarfEJar())
                            state= 6;
                        else if (token.IsHarf())
                            state= 7;
                        else
                        {
                            tokens.Push(token);
                            // return MURAKAB_TOSEEFI;
                            return true;
                        }
                        break;
                    case 6:
                        if (token.IsIsmSift())
                            state= 2;
                        else if (token.IsIsm())
                            state= 6;
                        else
                        {
                            tokens.Push(token);
                            // return MURAKAB_JARI;
                            return true;
                        }
                        break;
                    case 7:
                        if (token.IsIsmSift())
                            state= 2;
                        else if (token.IsIsm())
                            state= 8;
                        else
                            return false;
                        break;
                    case 8:
                        if (token.IsIsmSift())
                            state= 2;
                        else if (token.IsIsm())
                            state= 8;
                        else if (token.IsHarfEJar())
                            state= 6;
                        else if (token.IsHarf())
                            state= 7;
                        else
                        {
                            tokens.Push(token);
                            // return MURAKAB_ATFI;
                            return true;
                        }
                        break;
                    case -1:
                        return false;
                    default:
                        break;
                }
            }

            return false;   
        }

        private bool IsJumlaFealea(Stack<Word> tokens)
        {
            int state = 1;

            var token = tokens.Pop();
            while (token != null)
            {
                switch (state)
                {
                    case 1:
                        if (IsMurakbat(tokens) || token.IsIsm())
                            state = 2;
                        else
                            return false;
                        break;
                    case 2:
                        if (IsMurakbat(tokens) || token.IsIsm())
                            state = 3;
                        else if (token.IsFeal())
                            state = 7;
                        else if (token.IsAlamatMafool())
                            state = 5;
                        else if (token.IsAlamatNafi())
                            state = 6;
                        else if (token.IsAlamatFail())
                            state = 4;
                        else
                            return false;
                        break;
                    case 3:
                        if (IsMurakbat(tokens) || token.IsIsm())
                            state = 8;
                        else if (token.IsFeal())
                            state = 7;
                        else if (token.IsAlamatMafool())
                            state = 5;
                        else if (token.IsAlamatNafi())
                            state = 6;
                        else
                            return false;
                        break;
                    case 4:
                        if (IsMurakbat(tokens) || token.IsIsm())
                            state = 3;
                        else if (token.IsFeal())
                            state = 7;
                        else if (token.IsAlamatMafool())
                            state = 5;
                        else if (token.IsAlamatNafi())
                            state = 6;
                        else
                            return false;
                        break;
                    case 5:
                        if (IsMurakbat(tokens) || token.IsIsm())
                            state = 8;
                        else if (token.IsFeal())
                            state = 7;
                        else if (token.IsAlamatNafi())
                            state = 6;
                        else
                            return false;
                        break;
                    case 6:
                        if (IsMurakbat(tokens) || token.IsIsm())
                            state = 7;
                        else
                            return true;
                        break;
                    case 7:
                        return true;
                    case 8:
                        if (IsMurakbat(tokens) || token.IsIsm())
                            state = 6;
                        else if (token.IsFeal())
                            state = 7;
                        else if (token.IsAlamatNafi())
                            state = 6;
                        else
                            return false;
                        break;
                   
                    default:
                        return false;
                }
            }

            return false;
        }

        private bool IsFeal(Stack<Word> tokens)
        {
            int state = 1;

            var token = tokens.Pop();
            while (token != null)
            {
                switch (state)
                {
                    case 1:
                        if (token.IsFealHaal())
                            state = 2;
                        else if (token.IsFealMuzaray())
                            state = 4;
                        else if (token.IsFealMaziMutlaq())
                            state = 6;
                        else if (token.IsAmr())
                            state = 12;
                        else
                            return false;
                        break;
                    case 2:
                        if (token.IsFealNaqisHall())
                            state = 3;
                        else if (token.IsFealNaqisMazi())
                            state = 9;
                        else if (token.IsMight())
                            state = 10;
                        else
                            //return TYPE_FAEL_MAZI_TAMANNAI;
                            return true;
                        break;
                    case 3:
                        //return TYPE_FAEL_HAL;
                        return true;
                    case 4:
                        if (token.IsWill())
                            state = 5;
                        else
                            //return TYPE_FAEL_MUZARY;
                            return true;
                        break;
                    case 5:
                        // return TYPE_FAEL_MUSTAQBIL;
                        return true;
                    case 6:
                        if (token.IsFealNaqisHall())
                            state = 7;
                        else if (token.IsFealNaqisMazi())
                            state = 8;
                        else if (token.IsUsedTo())
                            state = 15;
                        else if (token.IsMight())
                            state = 10;
                        else if (token.IsWould() || token.IsHo())
                            state = 11;
                        else
                            // return TYPE_FAEL_MAZI_MUTLAQ;
                            return true;
                        break;
                    case 7:
                        // return TYPE_FAEL_MAZI_QAREEB;
                        return true;
                    case 8:
                        // return TYPE_FAEL_MAZI_BAEED;
                        return true;
                    case 9:
                        // return TYPE_FAEL_MAZI_ISTAMRARI;
                        return true;
                    case 10:
                        // return TYPE_FAEL_MAZI_SHAKKIA;
                        return true;
                    case 11:
                        // return TYPE_FAEL_MAZI_TAMANNAI;
                        return true;
                    case 12:
                        if (token.IsDoing())
                            state = 13;
                        else if (token.IsHad())
                            state = 14;
                        else
                            return false;
                        break;
                    case 13:
                        if (token.IsFealNaqisHall())
                            state = 3;
                        else if (token.IsFealNaqisMazi())
                            state = 8;
                        else if (token.IsMight())
                            state = 10;
                        else
                            return false;
                        break;
                    case 14:
                        if (token.IsFealNaqisHall())
                            state = 7;
                        else if (token.IsFealNaqisMazi())
                            state = 8;
                        else if (token.IsMight())
                            state = 10;
                        else
                            return false;
                        break;
                    case 15:
                        if (token.IsFealNaqisMazi())
                            state = 9;
                        else
                            return false;
                        break;
                    default:
                        break;
                }
            }

            return false;
        }
    }
}

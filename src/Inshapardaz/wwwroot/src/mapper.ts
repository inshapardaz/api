import { start } from 'repl';
import * as _ from 'lodash';

import { Entry } from './models/entry';
import { Link } from './models/link';
import { Dictionaries } from './models/dictionaries';
import { Dictionary, DictionaryIndex } from './models/Dictionary';
import { Word } from './models/word';
import { WordPage } from './models/wordpage';
import { WordDetail } from './models/WordDetail';
import { Translation } from './models/Translation';
import { MeaningContext } from './models/MeaningContext';
import { Meaning } from './models/Meaning';
import { Relation } from './models/relation';

export class Mapper{
    public static MapEntry(source : any) : Entry{
        let entry = new Entry();
        entry.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        entry.dictionariesLink = Mapper.findHrefWithRel(source.links, RelTypes.Dictionaries);
        entry.thesaurusLink = Mapper.findHrefWithRel(source.links, RelTypes.Thesaurus);
        entry.languagesLink = Mapper.findHrefWithRel(source.links, RelTypes.Languages);
        entry.attributesLink = Mapper.findHrefWithRel(source.links, RelTypes.Attributes);
        entry.relationshipTypesLink = Mapper.findHrefWithRel(source.links, RelTypes.RelationshipTypes);
        return entry;
    }

    public static MapDictionaries(source : any) : Dictionaries{
        let dictionaries = new Dictionaries();
        dictionaries.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        dictionaries.createLink = Mapper.findHrefWithRel(source.links, RelTypes.Create);
        dictionaries.dictionaries = Mapper.MapDictionaryList(source.items);
        return dictionaries;
    }

    public static MapDictionaryList(source : any) : Array<Dictionary>{
        var dictionariesCol = new Array<Dictionary>();
        _.forEach(source, (d) => dictionariesCol.push(Mapper.MapDictionary(d)));        
         return dictionariesCol;
    }

    public static MapDictionary(source : any) : Dictionary{
        let dictionary = new Dictionary();
        dictionary.id = source.id;
        dictionary.name = source.name;
        dictionary.isPublic = source.isPublic;
        dictionary.wordCount = source.wordCount;
        dictionary.language  = source.language;

        dictionary.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        dictionary.searchLink = Mapper.findHrefWithRel(source.links, RelTypes.Search);
        dictionary.indexLink = Mapper.findHrefWithRel(source.links, RelTypes.Index);
        dictionary.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        dictionary.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);
        dictionary.createWordLink = Mapper.findHrefWithRel(source.links, RelTypes.CreateWord);
        dictionary.createDownloadLink = Mapper.findHrefWithRel(source.links, RelTypes.CreateDownload);

        var indexes = new Array<DictionaryIndex>();
        _.forEach(source.indexes, (i) => indexes.push(Mapper.MapDictionaryIndex(i)));
        dictionary.indexes = indexes;

        return dictionary;
    }

    public static MapDictionaryIndex(sourceIndex : any) : DictionaryIndex{
        let index = new DictionaryIndex();
        index.link = sourceIndex.href;
        index.title = sourceIndex.rel;
        return index;
    }

    public static MapWordPage(source : any) : WordPage {
        let page = new WordPage();
        page.currentPageIndex = source.currentPageIndex;
        page.pageSize = source.pageSize;
        page.pageCount = source.pageCount;
        page.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        page.nextLink = Mapper.findHrefWithRel(source.links, RelTypes.Next);
        page.prevLink = Mapper.findHrefWithRel(source.links, RelTypes.Previous);
        page.words = Mapper.MapWords(source.data);
        return page;
    }

    public static MapWords(source : any) : Word[]{
        var words = [];
        _.forEach(source, (v) => words.push(Mapper.MapWord(v)));
        return words;
    }

    public static MapWord(source : any) : Word{
        let word = new Word();
        word.id = source.id;
        word.title = source.title;
        word.titleWithMovements = source.titleWithMovements;
        word.pronunciation = source.pronunciation;
        word.description = source.description;

        word.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        word.relationsLink = Mapper.findHrefWithRel(source.links, RelTypes.Relations);
        word.detailsLink = Mapper.findHrefWithRel(source.links, RelTypes.Details);
        word.dictionaryLink = Mapper.findHrefWithRel(source.links, RelTypes.Dictionary);
        word.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        word.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);
        word.addDetailLink = Mapper.findHrefWithRel(source.links, RelTypes.AddDetail);
        word.addRelationLink = Mapper.findHrefWithRel(source.links, RelTypes.AddRelation);

        return word;
    }

    public static MapWordDetails(source : any) : WordDetail[]{
        var details = []
        _.forEach(source, (v) => details.push(Mapper.MapWordDetail(v)));        
        return details;
    }

    public static MapWordDetail(source : any) : WordDetail{
        let detail = new WordDetail();
        detail.id = source.id;
        detail.wordId = source.wordId;
        detail.attributes = source.attributes;
        detail.attributeValue = source.attributeValue;
        detail.language = source.language;
        detail.languageId = source.languageId;
                
        detail.translations = Mapper.MapTranslations(source.translations);
        detail.meaningContexts = Mapper.MapMeaningContexts(source.meanings);

        detail.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        detail.translationsLink = Mapper.findHrefWithRel(source.links, RelTypes.Translations);
        detail.meaningsLink = Mapper.findHrefWithRel(source.links, RelTypes.Meanings)
        detail.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        detail.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);
        detail.createMeaningLink = Mapper.findHrefWithRel(source.links, RelTypes.AddMeaning);
        detail.createTranslationLink = Mapper.findHrefWithRel(source.links, RelTypes.AddTranslation);
        return detail;
    }

    public static MapTranslations(source: any) : Translation[]{
        var translations = [];
        _.forEach(source, (v) => translations.push(Mapper.MapTranslation(v)));
        return translations;
    }

    public static MapTranslation(source: any) : Translation{
        let translation = new Translation();
        translation.id = source.id;
        translation.language = source.language;
        translation.languageId = source.languageId;
        translation.value = source.value;
        translation.wordId = source.wordId;

        translation.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        translation.parentLink = Mapper.findHrefWithRel(source.links, RelTypes.WordDetail);
        translation.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        translation.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);

        return translation;
    }

    public static MapMeaningContexts(source: any) : MeaningContext[]{
        var contexts = [];
        _.forEach(source, (v) => contexts.push(Mapper.MapMeaningContext(v)));
        return contexts;
    }

    public static MapMeaningContext(source: any) : MeaningContext{
        let ct = new MeaningContext();
        ct.context = source.context;
        ct.meanings = Mapper.MapMeanings(source.meanings);
        
        ct.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        
        return ct;
    }
    
    public static MapMeanings(source: any) : Meaning[]{
        var meanings = [];
        _.forEach(source, (v) => meanings.push(Mapper.MapMeaning(v)));        
        return meanings;
    }

    public static MapMeaning(source: any) : Meaning{
        let meaning = new Meaning();
        meaning.id = source.id;
        meaning.wordDetailId = source.wordDetailId;
        meaning.value = source.value;
        meaning.example = source.example;
        meaning.selfLink =  Mapper.findHrefWithRel(source.links, RelTypes.Self);
        meaning.parentLink =  Mapper.findHrefWithRel(source.links, RelTypes.WordDetail);
        meaning.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        meaning.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);

        return meaning;
    }

    public static MapRelations(source: any) : Relation[]{
        var relations = [];
        _.forEach(source, (v) => relations.push(Mapper.MapRelation(v)));
        return relations;
    }

    public static MapRelation(source: any) : Relation{
        let relation = new Relation();
        relation.id = source.id;
        relation.sourceWord = source.sourceWord;
        relation.sourceWordId = source.sourceWordId;
        relation.relatedWord = source.relatedWord;
        relation.relatedWordId = source.relatedWordId;
        relation.relationType = source.relationType;
        relation.relationTypeId = source.relationTypeId;

        relation.selfLink =  Mapper.findHrefWithRel(source.links, RelTypes.Self);
        relation.relatedWordLink =  Mapper.findHrefWithRel(source.links, RelTypes.RelatedWord);
        relation.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        relation.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);

        return relation;
    }

    private static findHrefWithRel(links : any, rel : string){
        var link : any = _.find(links, ['rel', rel]);
        if (link != null){
            return link.href;
        }

        return null;
    }
}

export class RelTypes{
    public static readonly Self = "self";
    public static readonly Update = "update";
    public static readonly Delete = "delete";
    public static readonly RelatedWord = "related-word";
    public static readonly WordDetail = "worddetail";
    public static readonly Translations = "translations";
    public static readonly Meanings = "meanings";
    public static readonly AddMeaning = "addMeaning";
    public static readonly AddTranslation = "addTranslation";
    public static readonly Relations = "relations";

    public static readonly Details = "details";
    public static readonly Dictionary = "dictionary";
    public static readonly AddDetail = "add-detail";
    public static readonly AddRelation = "add-relation";

    public static readonly Next = "next";
    public static readonly Previous = "previous";

    public static readonly Create = "create";
    public static readonly Search = "search";
    public static readonly Index = "index";
    public static readonly CreateWord = "create-word";
    public static readonly CreateDownload = "create-download";

    public static readonly Dictionaries = "dictionaries";
    public static readonly Thesaurus = "thesaurus";
    public static readonly Languages = "languages";
    public static readonly Attributes = "attributes";
    public static readonly RelationshipTypes = "relationshiptypes";
}
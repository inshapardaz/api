import * as _ from 'lodash';
import { Entry } from './models/entry';
import { Dictionaries } from './models/dictionaries';
import { Dictionary, DictionaryIndex } from './models/Dictionary';
import { Word } from './models/Word';
import { WordPage } from './models/wordpage';
import { WordDetail } from './models/WordDetail';
import { Translation } from './models/Translation';
import { MeaningContext } from './models/MeaningContext';
import { Meaning } from './models/Meaning';
import { Relation } from './models/relation';
var Mapper = (function () {
    function Mapper() {
    }
    Mapper.MapEntry = function (source) {
        var entry = new Entry();
        entry.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        entry.dictionariesLink = Mapper.findHrefWithRel(source.links, RelTypes.Dictionaries);
        entry.thesaurusLink = Mapper.findHrefWithRel(source.links, RelTypes.Thesaurus);
        entry.languagesLink = Mapper.findHrefWithRel(source.links, RelTypes.Languages);
        entry.attributesLink = Mapper.findHrefWithRel(source.links, RelTypes.Attributes);
        entry.relationshipTypesLink = Mapper.findHrefWithRel(source.links, RelTypes.RelationshipTypes);
        return entry;
    };
    Mapper.MapDictionaries = function (source) {
        var dictionaries = new Dictionaries();
        dictionaries.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        dictionaries.createLink = Mapper.findHrefWithRel(source.links, RelTypes.Create);
        dictionaries.dictionaries = Mapper.MapDictionaryList(source.items);
        return dictionaries;
    };
    Mapper.MapDictionaryList = function (source) {
        var dictionariesCol = new Array();
        _.forEach(source, function (d) { return dictionariesCol.push(Mapper.MapDictionary(d)); });
        return dictionariesCol;
    };
    Mapper.MapDictionary = function (source) {
        var dictionary = new Dictionary();
        dictionary.id = source.id;
        dictionary.name = source.name;
        dictionary.isPublic = source.isPublic;
        dictionary.wordCount = source.wordCount;
        dictionary.language = source.language;
        dictionary.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        dictionary.searchLink = Mapper.findHrefWithRel(source.links, RelTypes.Search);
        dictionary.indexLink = Mapper.findHrefWithRel(source.links, RelTypes.Index);
        dictionary.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        dictionary.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);
        dictionary.createWordLink = Mapper.findHrefWithRel(source.links, RelTypes.CreateWord);
        dictionary.createDownloadLink = Mapper.findHrefWithRel(source.links, RelTypes.CreateDownload);
        var indexes = new Array();
        _.forEach(source.indexes, function (i) { return indexes.push(Mapper.MapDictionaryIndex(i)); });
        dictionary.indexes = indexes;
        return dictionary;
    };
    Mapper.MapDictionaryIndex = function (sourceIndex) {
        var index = new DictionaryIndex();
        index.link = sourceIndex.href;
        index.title = sourceIndex.rel;
        return index;
    };
    Mapper.MapWordPage = function (source) {
        var page = new WordPage();
        page.currentPageIndex = source.currentPageIndex;
        page.pageSize = source.pageSize;
        page.pageCount = source.pageCount;
        page.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        page.nextLink = Mapper.findHrefWithRel(source.links, RelTypes.Next);
        page.prevLink = Mapper.findHrefWithRel(source.links, RelTypes.Previous);
        page.words = Mapper.MapWords(source.data);
        return page;
    };
    Mapper.MapWords = function (source) {
        var words = [];
        _.forEach(source, function (v) { return words.push(Mapper.MapWord(v)); });
        return words;
    };
    Mapper.MapWord = function (source) {
        var word = new Word();
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
    };
    Mapper.MapWordDetails = function (source) {
        var details = [];
        _.forEach(source, function (v) { return details.push(Mapper.MapWordDetail(v)); });
        return details;
    };
    Mapper.MapWordDetail = function (source) {
        var detail = new WordDetail();
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
        detail.meaningsLink = Mapper.findHrefWithRel(source.links, RelTypes.Meanings);
        detail.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        detail.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);
        detail.createMeaningLink = Mapper.findHrefWithRel(source.links, RelTypes.AddMeaning);
        detail.createTranslationLink = Mapper.findHrefWithRel(source.links, RelTypes.AddTranslation);
        return detail;
    };
    Mapper.MapTranslations = function (source) {
        var translations = [];
        _.forEach(source, function (v) { return translations.push(Mapper.MapTranslation(v)); });
        return translations;
    };
    Mapper.MapTranslation = function (source) {
        var translation = new Translation();
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
    };
    Mapper.MapMeaningContexts = function (source) {
        var contexts = [];
        _.forEach(source, function (v) { return contexts.push(Mapper.MapMeaningContext(v)); });
        return contexts;
    };
    Mapper.MapMeaningContext = function (source) {
        var ct = new MeaningContext();
        ct.context = source.context;
        ct.meanings = Mapper.MapMeanings(source.meanings);
        ct.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        return ct;
    };
    Mapper.MapMeanings = function (source) {
        var meanings = [];
        _.forEach(source, function (v) { return meanings.push(Mapper.MapMeaning(v)); });
        return meanings;
    };
    Mapper.MapMeaning = function (source) {
        var meaning = new Meaning();
        meaning.id = source.id;
        meaning.wordDetailId = source.wordDetailId;
        meaning.value = source.value;
        meaning.example = source.example;
        meaning.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        meaning.parentLink = Mapper.findHrefWithRel(source.links, RelTypes.WordDetail);
        meaning.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        meaning.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);
        return meaning;
    };
    Mapper.MapRelations = function (source) {
        var relations = [];
        _.forEach(source, function (v) { return relations.push(Mapper.MapRelation(v)); });
        return relations;
    };
    Mapper.MapRelation = function (source) {
        var relation = new Relation();
        relation.id = source.id;
        relation.sourceWord = source.sourceWord;
        relation.sourceWordId = source.sourceWordId;
        relation.relatedWord = source.relatedWord;
        relation.relatedWordId = source.relatedWordId;
        relation.relationType = source.relationType;
        relation.relationTypeId = source.relationTypeId;
        relation.selfLink = Mapper.findHrefWithRel(source.links, RelTypes.Self);
        relation.relatedWordLink = Mapper.findHrefWithRel(source.links, RelTypes.RelatedWord);
        relation.updateLink = Mapper.findHrefWithRel(source.links, RelTypes.Update);
        relation.deleteLink = Mapper.findHrefWithRel(source.links, RelTypes.Delete);
        return relation;
    };
    Mapper.findHrefWithRel = function (links, rel) {
        var link = _.find(links, ['rel', rel]);
        if (link != null) {
            return link.href;
        }
        return null;
    };
    return Mapper;
}());
export { Mapper };
var RelTypes = (function () {
    function RelTypes() {
    }
    return RelTypes;
}());
export { RelTypes };
RelTypes.Self = "self";
RelTypes.Update = "update";
RelTypes.Delete = "delete";
RelTypes.RelatedWord = "related-word";
RelTypes.WordDetail = "worddetail";
RelTypes.Translations = "translations";
RelTypes.Meanings = "meanings";
RelTypes.AddMeaning = "addMeaning";
RelTypes.AddTranslation = "addTranslation";
RelTypes.Relations = "relations";
RelTypes.Details = "details";
RelTypes.Dictionary = "dictionary";
RelTypes.AddDetail = "add-detail";
RelTypes.AddRelation = "add-relation";
RelTypes.Next = "next";
RelTypes.Previous = "previous";
RelTypes.Create = "create";
RelTypes.Search = "search";
RelTypes.Index = "index";
RelTypes.CreateWord = "create-word";
RelTypes.CreateDownload = "create-download";
RelTypes.Dictionaries = "dictionaries";
RelTypes.Thesaurus = "thesaurus";
RelTypes.Languages = "languages";
RelTypes.Attributes = "attributes";
RelTypes.RelationshipTypes = "relationshiptypes";
//# sourceMappingURL=mapper.js.map
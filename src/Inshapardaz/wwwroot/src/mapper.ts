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
        entry.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        entry.dictionariesLink = _.find<string[], Link>(source.links, ['rel', 'dictionaries']).href;
        entry.thesaurusLink = _.find<string[], Link>(source.links, ['rel', 'thesaurus']).href;
        entry.languagesLink = _.find<string[], Link>(source.links, ['rel', 'languages']).href;
        entry.attributesLink = _.find<string[], Link>(source.links, ['rel', 'attributes']).href;
        entry.relationshipTypesLink = _.find<string[], Link>(source.links, ['rel', 'relationshiptypes']).href;
        return entry;
    }

    public static MapDictionaries(source : any) : Dictionaries{
        let dictionaries = new Dictionaries();
        dictionaries.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
         var createLink = _.find<string[], Link>(source.links, ['rel', 'create']);
         if (createLink){
             dictionaries.createLink = createLink.href;
         }
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
        dictionary.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        dictionary.name = source.name;
        dictionary.isPublic = source.isPublic;
        dictionary.wordCount = source.wordCount;
        dictionary.language  = source.language;
        dictionary.searchLink = _.find<string[], Link>(source.links, ['rel', 'search']).href;
        dictionary.indexLink = _.find<string[], Link>(source.links, ['rel', 'index']).href;
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
        page.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;        
        
        var nextLink = _.find<string[], Link>(source.links, ['rel', 'next']);
        page.nextLink = nextLink ? nextLink.href : null;

        var prevLink = _.find<string[], Link>(source.links, ['rel', 'previous']);
        page.prevLink = prevLink ? prevLink.href : null ;
        
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

        word.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        word.relationsLink = _.find<string[], Link>(source.links, ['rel', 'relations']).href;
        word.detailsLink = _.find<string[], Link>(source.links, ['rel', 'details']).href;

        var updateLink = _.find<string[], Link>(source.links, ['rel', 'update']);
        if (updateLink != null){
            word.updateLink = updateLink.href;
        }

        var deleteLink = _.find<string[], Link>(source.links, ['rel', 'delete']);
        if (deleteLink != null){
            word.deleteLink = deleteLink.href;
        }

        return word;
    }

    public static MapWordDetails(source : any) : WordDetail[]{
        var details = []
        _.forEach(source, (v) => details.push(Mapper.MapWordDetail(v)));        
        return details;
    }

    public static MapWordDetail(source : any) : WordDetail{
        let word = new WordDetail();
        word.id = source.id;
        word.wordId = source.wordId;

        word.attributes = source.attributes;
        word.attributeValue = source.attributeValue;
        word.language = source.language;
        word.languageId = source.languageId;
        
        
        word.translations = Mapper.MapTranslations(source.translations);
        word.meaningContexts = Mapper.MapMeaningContexts(source.meanings);

        word.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        word.translationsLink = _.find<string[], Link>(source.links, ['rel', 'translations']).href;
        word.meaningsLink = _.find<string[], Link>(source.links, ['rel', 'meanings']).href;
        return word;
    }

    public static MapTranslations(source: any) : Translation[]{
        var translations = [];
        _.forEach(source, (v) => translations.push(Mapper.MapTranslation(v)));
        return translations;
    }

    public static MapTranslation(source: any) : Translation{
        let translation = new Translation();
        translation.id = source.id;
        translation.lanuage = source.lanuage;
        translation.languageValue = source.lanuageId;
        translation.value = source.value;
        translation.wordId = source.wordId;

        translation.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        translation.parentLink = _.find<string[], Link>(source.links, ['rel', 'worddetail']).href;
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
        
        ct.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        
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
        meaning.selfLink =  _.find<string[], Link>(source.links, ['rel', 'self']).href;
        meaning.parentLink =  _.find<string[], Link>(source.links, ['rel', 'worddetail']).href;
        

        var linkUpd = _.find<string[], Link>(source.links, ['rel', 'update']);
        if (linkUpd != null){
            meaning.updateLink = linkUpd.href;
        }
        var linkDel = _.find<string[], Link>(source.links, ['rel', 'delete']);
        if (linkDel != null){
            meaning.deleteLink = linkDel.href;
        }

        return meaning;
    }

    public static MapRelations(source: any) : Relation[]{
        var relations = [];
        _.forEach(source, (v) => relations.push(Mapper.MapRelation(v)));
        return relations;
    }

    public static MapRelation(source: any) : Relation{
        let meaning = new Relation();
        meaning.id = source.id;
        meaning.relatedWord = source.relatedWord;
        meaning.relatedWordId = source.relatedWordId;
        meaning.relationType = source.relationType;
        meaning.relationTypeId = source.relationTypeId;
        meaning.selfLink =  _.find<string[], Link>(source.links, ['rel', 'self']).href;
        meaning.relatedWordLink =  _.find<string[], Link>(source.links, ['rel', 'related-word']).href;
        
        return meaning;
    }
}
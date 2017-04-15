import * as _ from 'lodash';

import { Entry } from './models/entry';
import { Link } from './models/link';
import { Dictionaries } from './models/dictionaries';
import { Dictionary } from './models/Dictionary';
import { Word } from './models/word';
import { WordPage } from './models/wordpage';
import { WordDetail } from './models/WordDetail';
import { Translation } from './models/Translation';
import { MeaningContext } from './models/MeaningContext';
import { Meaning } from './models/Meaning';
import { Relation } from './models/relation';
import { DictionaryIndex } from './models/DictionaryIndex';

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
        dictionaries.dictionaries = _.forEach(source.items, d => Mapper.MapDictionary(d));
        return dictionaries;
    }

    public static MapDictionary(source : any) : Dictionary{
        let dictionary = new Dictionary();
        dictionary.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        dictionary.name = source.name;
        dictionary.isPublic = source.isPublic;
        dictionary.wordCount = source.wordCount;
        dictionary.language  = source.language;
        return dictionary;
    }

    public static MapDictionaryIndex(source : any) : DictionaryIndex{
        let index = new DictionaryIndex();
        index.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        index.indexes = source.indexes;
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
        return _.forEach(source, (v) => Mapper.MapWord(v));
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

        return word;
    }

    public static MapWordDetails(source : any) : WordDetail[]{
        return _.forEach(source, (v) => 
            Mapper.MapWordDetail(v)
        );        
    }

    public static MapWordDetail(source : any) : WordDetail{
        let word = new WordDetail();
        word.id = source.id;
        word.wordId = source.wordId;

        word.attributes = source.attributes;
        word.attributeValue = source.attributeValue;
        word.lanuage = source.lanuage;
        word.languageId = source.languageId;
        
        
        word.translations = Mapper.MapTranslations(source.translations);
        word.meaningContexts = Mapper.MapMeaningContexts(source.meanings);

        word.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        word.translationsLink = _.find<string[], Link>(source.links, ['rel', 'translations']).href;
        word.meaningsLink = _.find<string[], Link>(source.links, ['rel', 'meanings']).href;

        return word;
    }

    public static MapTranslations(source: any) : Translation[]{
        return _.forEach(source, (v) => Mapper.MapTranslation(v));
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
        return _.forEach(source, (v) => Mapper.MapMeaningContext(v));
    }

    public static MapMeaningContext(source: any) : MeaningContext{
        let ct = new MeaningContext();
        ct.context = source.context;
        ct.meanings = Mapper.MapMeanings(source.meanings);
        
        ct.selfLink = _.find<string[], Link>(source.links, ['rel', 'self']).href;
        
        return ct;
    }

    
    public static MapMeanings(source: any) : Meaning[]{
        return _.forEach(source, (v) => Mapper.MapMeaning(v));        
    }

    public static MapMeaning(source: any) : Meaning{
        let meaning = new Meaning();
        meaning.id = source.id;
        meaning.wordDetailId = source.wordDetailId;
        meaning.value = source.value;
        meaning.example = source.example;
        meaning.selfLink =  _.find<string[], Link>(source.links, ['rel', 'self']).href;
        meaning.parentLink =  _.find<string[], Link>(source.links, ['rel', 'worddetail']).href;
        
        return meaning;
    }

    public static MapRelations(source: any) : Relation[]{
        return _.forEach(source.relationships, (v) => Mapper.MapRelation(v));        
    }

    public static MapRelation(source: any) : Relation{
        let meaning = new Relation();
        meaning.id = source.id;
        meaning.relatedWord = source.relatedWord;
        meaning.relatedWordId = source.relatedWordId;
        meaning.relationType = source.relationType;
        meaning.relationTypeId = source.relationTypeId;
        meaning.selfLink =  _.find<string[], Link>(source.links, ['rel', 'self']).href;
        meaning.relatedWordLink =  _.find<string[], Link>(source.links, ['rel', 'word']).href;
        
        return meaning;
    }
}
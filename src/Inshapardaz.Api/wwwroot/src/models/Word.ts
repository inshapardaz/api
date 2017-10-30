import { Translation } from './Translation';
import { MeaningContext } from './MeaningContext';

export class Word {
    public id: number;
    public title: string;
    public titleWithMovements: string;
    public pronunciation : string;
    public description: string;
    public attributes: string;
    public attributeValue: number;
    public language: string;
    public languageId : number;

    public translations : Translation[];
    public meaningContexts : MeaningContext[];    

    public selfLink : string;
    public relationsLink : string;
    public updateLink :string;
    public deleteLink : string;
    public dictionaryLink : string;
    public addRelationLink : string;
    public translationsLink : string;
    public meaningsLink : string;
    public createMeaningLink : string;
    public createTranslationLink : string;

}
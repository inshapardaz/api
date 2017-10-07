import { Translation } from './Translation';
import { MeaningContext } from './MeaningContext';

export class WordDetail {

    public id: number;
    public wordId: number;
    public attributes: string;
    public attributeValue: number;
    public language: string;
    public languageId : number;
    
    public selfLink : string;
    public parentLink : string;
    public translationsLink : string;
    public meaningsLink : string;
    public createMeaningLink : string;
    public createTranslationLink : string;
    public updateLink : string;   
    public deleteLink : string;  

    public translations : Translation[];
    public meaningContexts : MeaningContext[];
}
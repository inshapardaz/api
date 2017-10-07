import { Word } from './word';

export class WordPage{
    public currentPageIndex : number;
    public pageSize : number; 
    public pageCount : number; 
     
    public words : Word[];

    public selfLink : string;
    public nextLink : string;
    public prevLink : string;
}
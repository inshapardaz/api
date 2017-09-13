import {RelationTypes} from './relationTypes';
export class Relation{
    public id : number;
    public relatedWord : string;
    public relatedWordId : number;
    public relationType : string
    public relationTypeId : RelationTypes;
    public relatedWordLink : string;
    public selfLink : string;

    public updateLink : string;
    public deleteLink : string;
}
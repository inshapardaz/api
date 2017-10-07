import { Meaning } from './Meaning';

export class MeaningContext {
    public id: number;
    public context : string;

    public selfLink: string;

    public meanings : Meaning[];
}
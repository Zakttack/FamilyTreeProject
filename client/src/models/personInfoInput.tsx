import PersonElement from "./PersonElement";
export enum PersonType {
    Member,
    InLaw
};

export default interface PersonInfoInput {
    type: PersonType;
    element: PersonElement
}
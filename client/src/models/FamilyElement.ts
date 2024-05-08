import PersonElement from "./PersonElement";
export default interface FamilyElement {
    member: PersonElement;
    inLaw: PersonElement;
    marriageDate: string;
}
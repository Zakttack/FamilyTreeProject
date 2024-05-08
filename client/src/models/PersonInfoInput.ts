import PersonElement from "./PersonElement";
import { PersonType } from "../enums/PersonType";

export default interface PersonInfoInput {
    type: PersonType;
    element: PersonElement
}
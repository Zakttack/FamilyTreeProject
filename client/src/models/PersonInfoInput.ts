import PersonElement from "./PersonElement";
import { PersonType } from "../Enums";

export default interface PersonInfoInput {
    type: PersonType;
    element: PersonElement
}
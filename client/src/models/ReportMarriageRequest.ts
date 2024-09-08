import FamilyElement from "./FamilyElement";
import PersonElement from "./PersonElement";

export default interface ReportMarriageRequest {
    initialMember: PersonElement;
    family: FamilyElement
}
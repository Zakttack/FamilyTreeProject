import { FamilyTreeApiResponseStatus } from "./Enums";
import FamilyElement from "./models/FamilyElement";
import FamilyTreeApiResponse from "./models/FamilyTreeApiResponse";
import PersonElement from "./models/PersonElement";

export const StringDefault = 'unknown';
export const FamilyTreeController = 'http://localhost:5201/api/familytree';
export const UtilityController = 'http://localhost:5201/api/utility';
export const NumberDefault = 0;

export const PersonDefault: PersonElement = {
    name: StringDefault,
    birthDate: StringDefault,
    deceasedDate: StringDefault
};
export const FamilyDefault: FamilyElement = {
    member: PersonDefault,
    inLaw: PersonDefault,
    marriageDate: StringDefault
};

export const EmptyResponse: FamilyTreeApiResponse = {
    status: FamilyTreeApiResponseStatus.Processing,
    message: ''
};
import { FamilyTreeApiResponseStatus } from "../Enums";

export default interface FamilyTreeApiResponse {
    status: FamilyTreeApiResponseStatus;
    message: string;
    result?: any;
}
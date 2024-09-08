import _ from "lodash";
import { FamilyTreeApiResponseStatus, LoadingContext } from "./Enums";
import FamilyTreeApiResponse from "./models/FamilyTreeApiResponse";
import { EmptyResponse } from "./Constants";

export function createURL(path: string, queryParams = {}): string {
    const queryString = new URLSearchParams(queryParams).toString();
    return `${path}?${queryString}`;
}

export function getLoadingText(context: LoadingContext): string {
    if (context === LoadingContext.FamilyElementToRepresentation || context === LoadingContext.PersonElementToRepresentation) {
        return 'Retrieving Representation...';
    }
    switch (context) {
        case LoadingContext.GenerationNumber: return 'Retrieving Generation Number...';
        case LoadingContext.NumberOfFamilies: return 'Retrieving Number Of Families...';
        case LoadingContext.NumberOfGenerations: return 'Retrieving Number Of Generations...';
        case LoadingContext.ReportChildren: return 'Children Report is Processing...';
        case LoadingContext.ReportDeceased: return 'Deceased Report is Processing...';
        case LoadingContext.ReportMarriage: return 'Marriage Report is Processing...';
        case LoadingContext.RepresentationToFamilyElement: return 'Updating Selection Of Family Element...';
        case LoadingContext.RetrieveChildren: return 'Retrieving Children...';
        case LoadingContext.RetrieveFamilyTree: return 'Retrieving Family Tree...';
        case LoadingContext.RetrieveParent: return 'Retrieving Parent...';
        case LoadingContext.RevertTree: return 'Reverting Family Tree...';
        case LoadingContext.ViewSubtree: return 'Retrieving Subtree...';
    }
    return 'Loading Content...';
}

export function isProcessing(response: FamilyTreeApiResponse) {
    return _.isEqual(response, EmptyResponse);
}

export function isSuccess(response: FamilyTreeApiResponse) {
    return response.status === FamilyTreeApiResponseStatus.Success;
}
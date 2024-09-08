import FamilyElement from "./models/FamilyElement";
import FamilyTreeApiResponse from "./models/FamilyTreeApiResponse";
import PersonElement from "./models/PersonElement";
import ReportChildrenRequest from "./models/ReportChildrenRequest";
import ReportDeceasedRequest from "./models/ReportDeceasedRequest";
import ReportMarriageRequest from "./models/ReportMarriageRequest";
import RepresentationElement from "./models/RepresentationElement";
import { UtilityController, FamilyTreeController} from "./Constants";
import { createURL } from "./Utils";

export async function familyElementToRepresentation(element: FamilyElement): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/family-element-to-representation`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function generationNumberOf(element: FamilyElement): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/generation-number-of`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function getClientFamilyName(): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/get-client-family-name`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

export async function getClientFamilyTree(): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/get-client-family-tree`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

export async function getClientSelectedFamily(): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/get-client-selected-family`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

export async function getClientTitle(): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/get-client-title`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

export async function getFamilies(orderOption: string, memberName: string): Promise<FamilyTreeApiResponse> {
    const url = createURL(`${FamilyTreeController}/get-families/${encodeURIComponent(orderOption)}/by-member-name`, {memberName: memberName});
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

export async function getNumberOfFamilies(): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/number-of-families`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

export async function getNumberOfGenerations(): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/number-of-generations`;
    const response = await fetch(url);
    return await response.json() as FamilyTreeApiResponse;
}

export async function initializeService(familyName: string): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/initialize-service/${familyName}`;
    return await (await fetch(url)).json() as FamilyTreeApiResponse;
}

export async function personElementToRepresentation(element: PersonElement): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/person-element-to-representation`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function reportChildren(request: ReportChildrenRequest): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/report-children`;
    return await (await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })).json() as FamilyTreeApiResponse;
}

export async function reportDeceased(request: ReportDeceasedRequest): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/report-deceased`;
    return await (await fetch(url, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })).json() as FamilyTreeApiResponse;
}

export async function reportMarriage(request: ReportMarriageRequest): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/report-married`;
    return await (await fetch(url, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })).json() as FamilyTreeApiResponse;
}

export async function representationToFamilyElement(representation: RepresentationElement): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/representation-to-family-element`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(representation)
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function retrieveChildren(element: FamilyElement): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/retrieve-children`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function retrieveParent(element: FamilyElement): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/retrieve-parent`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function revertTree(request: File): Promise<FamilyTreeApiResponse> {
    const url = `${FamilyTreeController}/revert-tree`;
    const data = new FormData();
    data.append('file', request);
    const response = await fetch(url, {
        method: 'PUT',
        body: data
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function setClientFamilyName(familyName: string): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/set-client-family-name/${encodeURIComponent(familyName)}`;
    const response = await fetch(url, {
        method: 'PUT'
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function setClientFamilyTree(familyTree: FamilyElement[]): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/set-client-family-tree`;
    const response = await fetch(url, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(familyTree)
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function setClientSelectedFamily(selectedFamily: FamilyElement): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/set-client-selected-family`;
    const response = await fetch(url, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(selectedFamily)
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function setClientTitle(title: string): Promise<FamilyTreeApiResponse> {
    const url = `${UtilityController}/set-client-title/${encodeURIComponent(title)}`;
    const response = await fetch(url, {
        method: 'PUT'
    });
    return await response.json() as FamilyTreeApiResponse;
}

export async function viewSubtree(orderOption: string, memberName: string, family: FamilyElement): Promise<FamilyTreeApiResponse> {
    const url = createURL(`${FamilyTreeController}/view-subtree/${encodeURIComponent(orderOption)}/by-member-name`, {memberName: memberName});
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(family)
    });
    return await response.json() as FamilyTreeApiResponse;
}
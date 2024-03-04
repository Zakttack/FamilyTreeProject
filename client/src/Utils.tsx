import FamilyElement from "./models/FamilyElement";
import RepresentationElement from "./models/RepresentationElement";
import MessageResponse from "./models/MessageResponse";
import OutputResponse from "./models/outputResponse";
import PersonElement from "./models/PersonElement";
import FileElement from "./models/FileElement";

export async function elementToRepresentation(element: FamilyElement): Promise<OutputResponse<RepresentationElement>> {
    const url = 'http://localhost:5201/api/utility/element-to-representation';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: RepresentationElement = await response.json();
    return {output: result};
};

export async function getFilePaths(fileName: string): Promise<OutputResponse<FileElement[]>> {
    const url = `http://localhost:5201/api/utility/get-file-paths/${fileName}`;
    const response = await fetch(url);
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: FileElement[] = await response.json();
    return {output: result};
}

export async function getNumberOfFamilies(familyName: string): Promise<OutputResponse<number>> {
    const url = `http://localhost:5201/api/familytree/${familyName}/number-of-families`;
    const response = await fetch(url);
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: number = await response.json();
    return {output: result};
}

export async function getNumberOfGenerations(familyName: string): Promise<OutputResponse<number>> {
    const url = `http://localhost:5201/api/familytree/${familyName}/number-of-generations`;
    const response = await fetch(url);
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: number = await response.json();
    return {output: result};
}

export async function representationToElement(representation: RepresentationElement): Promise<OutputResponse<FamilyElement>> {
    const url = 'http://localhost:5201/api/utility/representation-to-element';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(representation)
    });
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: FamilyElement = await response.json();
    return {output: result};
}

export async function retrieveParent(familyName: string, element: FamilyElement): Promise<OutputResponse<FamilyElement>> {
    const url = `http://localhost:5201/api/familytree/${familyName}/retrieveParent`;
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        const errorResult: MessageResponse = await response.json();
        return {problem: errorResult};
    }
    const result: FamilyElement = await response.json();
    return {output: result};
}

export async function revertTree(familyName: string, request: FileElement): Promise<OutputResponse<MessageResponse>> {
    const url = `http://localhost:5201/api/familytree/${familyName}/revert-tree`;
    const response = await fetch(url, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    });
    const result: MessageResponse = await response.json();
    return result.isSuccess ? {output: result} : {problem: result};
}

export async function personElementToRepresentation(element: PersonElement): Promise<OutputResponse<RepresentationElement>> {
    const url = 'http://localhost:5201/api/utility/person-element-to-representation';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        const result: MessageResponse = await response.json();
        return {problem: result};
    }
    const result: RepresentationElement = await response.json();
    return {output: result};
}

export const StringDefault = 'unknown';
export const NumberDefault = 0;
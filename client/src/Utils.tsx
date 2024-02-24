import ExceptionResponse from "./models/exceptionResponse";
import FamilyElement from "./models/FamilyElement";
import FamilyRepresentationElement from "./models/familyRepresentationElement";
import OutputResponse from "./models/outputResponse";

export async function elementToRepresentation(element: FamilyElement): Promise<OutputResponse<FamilyRepresentationElement>> {
    const url = 'http://localhost:5201/api/utility/element-to-representation';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(element)
    });
    if (!response.ok) {
        const result: ExceptionResponse = await response.json();
        return {output: null, problem: result};
    }
    const result: FamilyRepresentationElement = await response.json();
    return {output: result, problem: null};
};

export async function representationToElement(representation: FamilyRepresentationElement): Promise<OutputResponse<FamilyElement>> {
    const url = 'http://localhost:5201/api/utility/representation-to-element';
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(representation)
    });
    if (!response.ok) {
        const result: ExceptionResponse = await response.json();
        return {output: null, problem: result};
    }
    const result: FamilyElement = await response.json();
    return {output: result, problem: null};
}
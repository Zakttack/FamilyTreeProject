import React from "react";
import PersonElement, { PersonDefault } from "./PersonElement";
export default interface FamilyElement {
    member: PersonElement;
    inLaw: PersonElement;
    marriageDate: string | null;
}

export const FamilyDefault: FamilyElement = {
    member: PersonDefault,
    inLaw: PersonDefault,
    marriageDate: null
};

interface FamilyElementContextType {
    selectedElement: FamilyElement;
    changeSelectedElement: (selectedElement: FamilyElement) => void;
}

export const FamilyElementContext = React.createContext<FamilyElementContextType>({selectedElement: FamilyDefault, changeSelectedElement: () => {}});
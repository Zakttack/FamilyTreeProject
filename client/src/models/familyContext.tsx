import React from "react";
import { PersonDefault, PersonElement } from "./personContext";

export interface FamilyElement {
    member: PersonElement;
    inLaw: PersonElement | null;
    marriageDate: string | null;
}

export const FamilyDefault: FamilyElement = {member: PersonDefault, inLaw: null, marriageDate: null};

interface FamilyContextType {
    family: FamilyElement;
    setFamily: (family: FamilyElement) => void;
}

const FamilyContext = React.createContext<FamilyContextType>({family: FamilyDefault, setFamily: () => {}});

export default FamilyContext;
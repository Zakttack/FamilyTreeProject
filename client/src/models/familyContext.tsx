import React from "react";
import { PersonDefault, PersonElement } from "./personContext";

export interface FamilyElement {
    member: PersonElement;
    inLaw: PersonElement;
    marriageDate: string;
}

export const FamilyDefault: FamilyElement = {member: PersonDefault, inLaw: PersonDefault, marriageDate: ''};

interface FamilyContextType {
    family: FamilyElement;
    setFamily: (family: FamilyElement) => void;
}

const FamilyContext = React.createContext<FamilyContextType>({family: FamilyDefault, setFamily: () => {}});

export default FamilyContext;
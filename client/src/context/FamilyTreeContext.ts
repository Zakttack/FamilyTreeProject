import React from "react";
import OutputResponse from "../models/OutputResponse";
import FamilyElement from "../models/FamilyElement";

interface FamilyTreeContextType {
    familyTreeResponse: OutputResponse<FamilyElement[]>;
    setFamilyTreeResponse: (familyTreeResponse: OutputResponse<FamilyElement[]>) => void;
}

const FamilyTreeContext = React.createContext<FamilyTreeContextType>({familyTreeResponse: {}, setFamilyTreeResponse: () => {}});
export default FamilyTreeContext;
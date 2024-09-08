import React from "react";
import FamilyElement from "../models/FamilyElement";

interface FamilyTreeContextType {
    familyTree: FamilyElement[];
    setFamilyTree: (familyTree: FamilyElement[]) => void;
};

const FamilyTreeContext = React.createContext<FamilyTreeContextType>({familyTree: [], setFamilyTree: () => {}});

export default FamilyTreeContext;
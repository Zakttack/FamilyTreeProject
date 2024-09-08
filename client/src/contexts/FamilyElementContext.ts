import React from "react";
import FamilyElement from "../models/FamilyElement";
import { FamilyDefault } from "../Constants";

interface FamilyElementContextType {
    selectedElement: FamilyElement;
    changeSelectedElement: (selectedElement: FamilyElement) => void;
}

const FamilyElementContext = React.createContext<FamilyElementContextType>({selectedElement: FamilyDefault, changeSelectedElement: () => {}});

export default FamilyElementContext;
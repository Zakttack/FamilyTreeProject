import React from "react";
import FamilyElement from "../models/FamilyElement";
import { FamilyDefault } from "../Utils";
interface FamilyElementContextType {
    selectedFamily: FamilyElement;
    changeSelectedFamily: (selectedElement: FamilyElement) => void;
}

const SelectedFamilyContext = React.createContext<FamilyElementContextType>({selectedFamily: FamilyDefault, changeSelectedFamily: () => {}});
export default SelectedFamilyContext;
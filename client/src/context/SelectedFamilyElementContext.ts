import React from "react";
import FamilyElement from "../models/FamilyElement";
import { FamilyDefault } from "../Utils";

interface SelectedFamilyElementContextType {
    selectedFamilyElement: FamilyElement;
    changeFamilyElement: (selectedFamilyElement: FamilyElement) => void;
}

const SelectedFamilyElementContext = React.createContext<SelectedFamilyElementContextType>({selectedFamilyElement: FamilyDefault, changeFamilyElement: () => {}});
export default SelectedFamilyElementContext;
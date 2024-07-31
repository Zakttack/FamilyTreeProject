import React, {useState} from "react";
import { ProviderProps } from "../models/ProviderProps";
import SelectedFamilyElementContext from "../context/SelectedFamilyElementContext";
import FamilyElement from "../models/FamilyElement";
import { FamilyDefault } from "../Utils";

const SelectedFamilyElementProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedFamilyElement, changeFamilyElement] = useState<FamilyElement>(FamilyDefault);
    return (
        <SelectedFamilyElementContext.Provider value={{selectedFamilyElement, changeFamilyElement}}>
            {children}
        </SelectedFamilyElementContext.Provider>
    );
};
export default SelectedFamilyElementProvider;
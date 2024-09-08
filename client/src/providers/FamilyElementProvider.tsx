import React, { useState } from "react";
import FamilyElementContext from "../contexts/FamilyElementContext";
import FamilyElement from "../models/FamilyElement";
import ProviderProps from "../models/ProviderProps";
import { FamilyDefault } from "../Constants";

const FamilyElementProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedElement, changeSelectedElement] = useState<FamilyElement>(FamilyDefault);
    return (
        <FamilyElementContext.Provider value={{selectedElement, changeSelectedElement}}>
            {children}
        </FamilyElementContext.Provider>
    )
}

export default FamilyElementProvider;
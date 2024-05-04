import React from "react";
import { FamilyElementContext } from "../models/FamilyElement";
import { ProviderProps } from "../models/providerProps";
import { getClientSelectedFamily, setClientSelectedFamily } from "../Utils";

const FamilyElementProvider: React.FC<ProviderProps> = ({children}) => {
    return (
        <FamilyElementContext.Provider value={{selectedElement: getClientSelectedFamily(), changeSelectedElement: setClientSelectedFamily}}>
            {children}
        </FamilyElementContext.Provider>
    )
}

export default FamilyElementProvider;
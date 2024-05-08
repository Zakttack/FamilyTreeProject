import React from "react";
import SelectedFamilyContext from "../context/SelectedFamilyContext";
import { ProviderProps } from "../models/ProviderProps";
import { getClientSelectedFamily, setClientSelectedFamily } from "../Utils";

const FamilyElementProvider: React.FC<ProviderProps> = ({children}) => {
    return (
        <SelectedFamilyContext.Provider value={{selectedFamily: getClientSelectedFamily(), changeSelectedFamily: setClientSelectedFamily}}>
            {children}
        </SelectedFamilyContext.Provider>
    )
}

export default FamilyElementProvider;
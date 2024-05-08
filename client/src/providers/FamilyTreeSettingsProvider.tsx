import React from "react";
import { ProviderProps } from "../models/ProviderProps";
import FamilyTreeContext from "../context/FamilyTreeContext";
import { getClientFamilyTree, setClientFamilyTree } from "../Utils";

const FamilyTreeProvider: React.FC<ProviderProps> = ({children}) => {

    return (
        <FamilyTreeContext.Provider value={{familyTreeResponse: getClientFamilyTree(), setFamilyTreeResponse: setClientFamilyTree}}>
                {children}
        </FamilyTreeContext.Provider>
    )
};

export default FamilyTreeProvider;
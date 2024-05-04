import React from "react";
import { ProviderProps } from "../models/providerProps";
import FamilyTreeContext from "../models/FamilyTreeSettings";
import { getClientFamilyTree, setClientFamilyTree } from "../Utils";

const FamilyTreeProvider: React.FC<ProviderProps> = ({children}) => {

    return (
        <FamilyTreeContext.Provider value={{familyTreeResponse: getClientFamilyTree(), setFamilyTreeResponse: setClientFamilyTree}}>
                {children}
        </FamilyTreeContext.Provider>
    )
};

export default FamilyTreeProvider;
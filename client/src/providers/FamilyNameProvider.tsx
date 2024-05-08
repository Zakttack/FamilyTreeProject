import React from "react";
import FamilyNameContext from "../context/FamilyNameContext";
import { ProviderProps } from "../models/ProviderProps";
import { getClientFamilyName, setClientFamilyName } from "../Utils";

const FamilyNameProvider: React.FC<ProviderProps> = ({children}) => {
    return (
        <FamilyNameContext.Provider value={{familyName: getClientFamilyName(), setFamilyName: setClientFamilyName}}>
            {children}
        </FamilyNameContext.Provider>
    );
};

export default FamilyNameProvider;
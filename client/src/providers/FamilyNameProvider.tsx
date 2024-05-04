import React from "react";
import FamilyNameContext from "../models/familyNameContext";
import { ProviderProps } from "../models/providerProps";
import { getClientFamilyName, setClientFamilyName } from "../Utils";

const FamilyNameProvider: React.FC<ProviderProps> = ({children}) => {
    return (
        <FamilyNameContext.Provider value={{familyName: getClientFamilyName(), setFamilyName: setClientFamilyName}}>
            {children}
        </FamilyNameContext.Provider>
    );
};

export default FamilyNameProvider;
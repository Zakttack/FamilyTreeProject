import React, {useState} from "react";
import FamilyNameContext from "../models/familyNameContext";
import { ProviderProps } from "../models/providerProps";
import { StringDefault } from "../Utils";

const FamilyNameProvider: React.FC<ProviderProps> = ({children}) => {
    const [familyName, setFamilyName] = useState<string>(StringDefault);
    return (
        <FamilyNameContext.Provider value={{familyName, setFamilyName}}>
            {children}
        </FamilyNameContext.Provider>
    );
};

export default FamilyNameProvider;
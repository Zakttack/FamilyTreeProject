import React, {useState} from "react";
import FamilyNameContext from "../contexts/FamilyNameContext";
import ProviderProps from "../models/ProviderProps";

const FamilyNameProvider: React.FC<ProviderProps> = ({children}) => {
    const [familyName, setFamilyName] = useState<string>('');
    return (
        <FamilyNameContext.Provider value={{familyName, setFamilyName}}>
            {children}
        </FamilyNameContext.Provider>
    );
};

export default FamilyNameProvider;
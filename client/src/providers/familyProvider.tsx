import React, {useState} from "react";
import { ProviderProps } from "../models/providerProps";
import FamilyContext, { FamilyDefault, FamilyElement } from "../models/familyContext";

const FamilyProvider: React.FC<ProviderProps> = ({children}) => {
    const [family, setFamily] = useState<FamilyElement>(FamilyDefault);

    return (
        <FamilyContext.Provider value={{family: family, setFamily: setFamily}}>
            {children}
        </FamilyContext.Provider>
    );
}

export default FamilyProvider;
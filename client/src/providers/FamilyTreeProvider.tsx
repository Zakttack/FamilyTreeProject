import React, {useState} from "react";
import { ProviderProps } from "../models/ProviderProps";
import FamilyTreeContext from "../context/FamilyTreeContext";
import FamilyElement from "../models/FamilyElement";
import OutputResponse from "../models/OutputResponse";

const FamilyTreeProvider: React.FC<ProviderProps> = ({children}) => {
    const [familyTreeResponse, setFamilyTreeResponse] = useState<OutputResponse<FamilyElement[]>>({});
    return (
        <FamilyTreeContext.Provider value={{familyTreeResponse, setFamilyTreeResponse}}>
            {children}
        </FamilyTreeContext.Provider>
    );
};

export default FamilyTreeProvider;
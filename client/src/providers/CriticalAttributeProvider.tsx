import React, {useState} from "react";
import CriticalAttributeContext from "../contexts/CriticalAttributeContext";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import ProviderProps from "../models/ProviderProps";
import { EmptyResponse } from "../Constants";

const CriticalAttributeProvider: React.FC<ProviderProps> = ({children}) => {
    const [criticalAttributeResponse, setCriticalAttributeResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);
    return (
        <CriticalAttributeContext.Provider value={{criticalAttributeResponse, setCriticalAttributeResponse}}>
            {children}
        </CriticalAttributeContext.Provider>
    );
};

export default CriticalAttributeProvider;
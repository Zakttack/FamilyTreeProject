import React from "react";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { EmptyResponse } from "../Constants";

interface CriticalAttributeContextType {
    criticalAttributeResponse: FamilyTreeApiResponse;
    setCriticalAttributeResponse: (criticalAttributeResponse: FamilyTreeApiResponse) => void;
}

const CriticalAttributeContext = React.createContext<CriticalAttributeContextType>({criticalAttributeResponse: EmptyResponse, setCriticalAttributeResponse: () => {}});

export default CriticalAttributeContext;
import React, { useState } from "react";
import ReportActionsContext from "../contexts/ReportActionsContext";
import ProviderProps from "../models/ProviderProps";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { EmptyResponse } from "../Constants";

const ReportActionsProvider: React.FC<ProviderProps> = ({children}) => {
    const [response, setResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [reportMade, isReportMade] = useState<boolean>(false);
    return (
        <ReportActionsContext.Provider value={{response, reportMade, isReportMade, setResponse}}>
            {children}
        </ReportActionsContext.Provider>
    );
};

export default ReportActionsProvider;
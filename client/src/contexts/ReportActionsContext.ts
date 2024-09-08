import React from "react";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { EmptyResponse } from "../Constants";

interface ReportActionsContextType {
    reportMade: boolean;
    response: FamilyTreeApiResponse;
    isReportMade: (reportMade: boolean) => void;
    setResponse: (response: FamilyTreeApiResponse) => void;
}

const ReportActionsContext = React.createContext<ReportActionsContextType>({response: EmptyResponse, reportMade: false, isReportMade: () => {}, setResponse: () => {}});

export default ReportActionsContext;
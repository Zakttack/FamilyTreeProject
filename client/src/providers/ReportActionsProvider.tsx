import React, { useState } from "react";
import ReportActionsContext from "../context/ReportActionsContext";
import { ProviderProps } from "../models/ProviderProps";
import OutputResponse from "../models/OutputResponse";
import MessageResponse from "../models/MessageResponse";

const ReportActionsProvider: React.FC<ProviderProps> = ({children}) => {
    const [response, setResponse] = useState<OutputResponse<MessageResponse>>({});
    return (
        <ReportActionsContext.Provider value={{response: response, setResponse: setResponse}}>
            {children}
        </ReportActionsContext.Provider>
    );
};

export default ReportActionsProvider;
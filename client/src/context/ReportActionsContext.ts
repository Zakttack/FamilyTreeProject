import React from "react";
import OutputResponse from "../models/OutputResponse";
import MessageResponse from "../models/MessageResponse";

interface ReportActionsContextType {
    response: OutputResponse<MessageResponse>;
    setResponse: (response: OutputResponse<MessageResponse>) => void;
}

const ReportActionsContext = React.createContext<ReportActionsContextType>({response: {}, setResponse: () => {}});

export default ReportActionsContext;
import React from "react";
import "./ErrorDisplayComponent.css";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { isProcessing, isSuccess } from "../Utils";

const ErrorDisplayComponent: React.FC<{response: FamilyTreeApiResponse}> = (params) => {
    if (!(isProcessing(params.response) || isSuccess(params.response))) {
        return (
            <p className="error">{params.response.message}</p>
        );
    }
    return null;
};

export default ErrorDisplayComponent;
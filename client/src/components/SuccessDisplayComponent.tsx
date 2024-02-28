import React from "react";
import SuccessResponse from "../models/successResponse";
import './SuccessDisplayComponent.css';

const SuccessDisplay: React.FC<SuccessResponse> = (response) => {
    return (
        <p className="success">{response.message}</p>
    );
};

export default SuccessDisplay;
import React from "react";
import ExceptionResponse from "../models/exceptionResponse";
import "./ErrorDisplayComponent.css";

const ErrorDisplayComponent: React.FC<ExceptionResponse> = (response) => {
    return (
        <p className="error">{response.name}: {response.message}</p>
    );
};

export default ErrorDisplayComponent;
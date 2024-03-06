import React, { useContext, useEffect, useState } from "react";
import FamilyNameContext from "../models/familyNameContext";
import OutputResponse from "../models/outputResponse";
import MessageResponse from "../models/MessageResponse";
import { initializeService } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";

const Title: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const [title, setTitle] = useState<OutputResponse<MessageResponse>>({});

    useEffect(() => {
        const getTitle = async() => {
            const response: OutputResponse<MessageResponse> = await initializeService(familyName);
            setTitle(response);
        };
        getTitle();
    }, [familyName]);

    if (title.problem) {
        return (
            <ErrorDisplayComponent message={title.problem.message}/>
        );
    }
    else if (title.output) {
        return (
            <h1>{title.output.message}</h1>
        );
    }
    return (
        <h1>Initializing Service...</h1>
    );
};

export default Title;
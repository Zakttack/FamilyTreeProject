import React, { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import OutputResponse from "../models/OutputResponse";
import MessageResponse from "../models/MessageResponse";
import {initializeService, setClientFamilyName, setClientPageTitle } from "../Utils";
import _ from "lodash";
import ErrorDisplayComponent from "../components/ErrorDisplayComponent";


const ChooseFamilyNamePage: React.FC = () => {
    let navigate = useNavigate();
    const [familyName, setfamilyName] = useState<string>('');
    const [familyNameResponse, setFamilyNameResponse] = useState<OutputResponse<MessageResponse>>({});
    const handleSubmitFamilyName = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        await setClientFamilyName(familyName);
        if (!_.isEqual(familyName, '')) {
            const response: OutputResponse<MessageResponse> = await initializeService(familyName);
            if (response.output) {
                await setClientPageTitle(response.output.message);
                navigate('/family-tree');
            }
        }
        else {
            setFamilyNameResponse({problem: {message: 'No name has been provided.', isSuccess: false}});
        }
    };
    return (
        <form onSubmit={handleSubmitFamilyName}>
            <h1>Welcome to the Client-Side of my Family Tree Project</h1><br/>
            <label>Enter a Family Name:&nbsp;<input type="text" value={familyName} onChange={(e) => setfamilyName(e.target.value)}/></label><br/>
            <button type="submit">Go To Family Tree</button>
            {!_.isUndefined(familyNameResponse.problem) && <ErrorDisplayComponent message={familyNameResponse.problem.message}/>}
        </form>
    );
};
export default ChooseFamilyNamePage;
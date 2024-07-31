import React, { FormEvent, useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import OutputResponse from "../models/OutputResponse";
import MessageResponse from "../models/MessageResponse";
import {initializeService, setClientFamilyName, setClientPageTitle, StringDefault } from "../Utils";
import _ from "lodash";
import ErrorDisplayComponent from "../components/ErrorDisplayComponent";
import FamilyNameContext from "../context/FamilyNameContext";
import TitleContext from "../context/TitleContext";


const ChooseFamilyNamePage: React.FC = () => {
    let navigate = useNavigate();
    const {name, setName} = useContext(FamilyNameContext);
    const {title, setTitle} = useContext(TitleContext);
    const [familyNameResponse, setFamilyNameResponse] = useState<OutputResponse<MessageResponse>>({});
    const handleSubmitFamilyName = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        await setClientFamilyName(name);
        if (!_.isEqual(name, StringDefault)) {
            const response: OutputResponse<MessageResponse> = await initializeService(name);
            if (response.output) {
                setTitle(response.output.message);
                await setClientPageTitle(title);
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
            <label>Enter a Family Name:&nbsp;<input type="text" value={name} onChange={(e) => setName(e.target.value)}/></label><br/>
            <button type="submit">Go To Family Tree</button>
            {!_.isUndefined(familyNameResponse.problem) && <ErrorDisplayComponent message={familyNameResponse.problem.message}/>}
        </form>
    );
};
export default ChooseFamilyNamePage;
import React, { useContext } from "react";
import { useNavigate } from "react-router-dom";
import FamilyNameInputComponent from "../components/FamilyNameInputComponent";
import TitleContext from "../models/TitleContext";
import OutputResponse from "../models/outputResponse";
import MessageResponse from "../models/MessageResponse";
import { initializeService } from "../Utils";
import FamilyNameContext from "../models/familyNameContext";


const ChooseFamilyNamePage: React.FC = () => {
    let navigate = useNavigate();
    const {familyName} = useContext(FamilyNameContext);
    const {setTitle} = useContext(TitleContext);
    const handleSubmitFamilyName = async() => {
        const response: OutputResponse<MessageResponse> = await initializeService(familyName);
        if (response.output) {
            setTitle(response.output.message);
            navigate('/family-tree');
        }
    };
    return (
        <div>
            <h1>Welcome to the Client-Side of my Family Tree Project</h1>
            <p>Enter a Family Name:&nbsp;<FamilyNameInputComponent/></p>
            <button id="submitFamilyNameButton" onClick={handleSubmitFamilyName}>Go To Family Tree</button>
        </div>
    );
};
export default ChooseFamilyNamePage;
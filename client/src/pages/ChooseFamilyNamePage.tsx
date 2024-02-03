import React from "react";
import { useNavigate } from "react-router-dom";
import FamilyNameInputComponent from "../components/FamilyNameInputComponent";


const ChooseFamilyNamePage: React.FC = () => {
    let navigate = useNavigate();
    const handleSubmitFamilyName = () => {
        navigate('/dashboard');
    };
    return (
        <div>
            <h1>Welcome to the Client-Side of my Family Tree Project</h1>
            <p>Enter a Family Name:&nbsp;<FamilyNameInputComponent/></p>
            <button id="submitFamilyNameButton" onClick={handleSubmitFamilyName}>Go To Dashboard</button>
        </div>
    );
};
export default ChooseFamilyNamePage;
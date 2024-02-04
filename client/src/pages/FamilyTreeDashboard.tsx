import React, { useContext } from "react";
import FamilyNameContext from "../models/familyNameContext";
import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";

const FamilyTreeDashboard: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    return (
        <div>
            <h1>This is the {familyName} family</h1>
            <GetNumberOfGenerationsComponent />
            <GetNumberOfFamiliesComponent />
        </div>
    );
}

export default FamilyTreeDashboard;
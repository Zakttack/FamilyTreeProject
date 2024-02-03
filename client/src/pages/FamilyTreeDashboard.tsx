import React, { useContext } from "react";
import FamilyNameContext from "../models/familyNameContext";

const FamilyTreeDashboard: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    return (
        <h1>This is the {familyName} family</h1>
    );
}

export default FamilyTreeDashboard;
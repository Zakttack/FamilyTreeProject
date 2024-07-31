import React from "react";
import FamilyTreeDisplay from "../components/FamilyTreeDisplay";
import FamilyTreeInput from "../components/FamilyTreeInput";
import { useNavigate } from "react-router-dom";
import Title from "../components/TitleComponent";
import FamilyTreeProvider from "../providers/FamilyTreeProvider";
const FamilySubTreePage: React.FC = () => {
    let navigate = useNavigate();
    return (
        <FamilyTreeProvider>
            <Title />
            <FamilyTreeInput includesEntireTree={false}/>
            <FamilyTreeDisplay />
            <button type="button" onClick={() => navigate('/family-tree')}>Back To Tree</button>
        </FamilyTreeProvider>
    );
};

export default FamilySubTreePage;
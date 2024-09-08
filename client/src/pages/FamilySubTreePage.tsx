import React from "react";
import { useNavigate } from "react-router-dom";
import FamilyTreeDisplay from "../components/FamilyTreeDisplay";
import FamilyTreeInput from "../components/FamilyTreeInput";
import Title from "../components/TitleComponent";
import FamilyTreeProvider from "../providers/FamilyTreeProvider";

const FamilySubTreePage: React.FC = () => {
    let navigate = useNavigate();
    return (
        <div>
            <Title />
            <FamilyTreeProvider>
                <FamilyTreeInput includesEntireTree={false}/>
                <FamilyTreeDisplay />
            </FamilyTreeProvider>
            <button type="button" onClick={() => navigate('/family-tree')}>Back To Tree</button>
        </div>
    );
};

export default FamilySubTreePage;
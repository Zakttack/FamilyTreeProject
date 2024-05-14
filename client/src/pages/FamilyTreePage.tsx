import React, { useEffect, useState } from "react";
//import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
//import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";
import FamilyTreeInput from "../components/FamilyTreeInput";
import FamilyTreeDisplay from "../components/FamilyTreeDisplay";
import SelectedFileProvider from "../providers/SelectedFileProvider";
import RevertTreeSection from "../components/RevertTreeSection";
import Title from "../components/TitleComponent";
import { getClientFamilyName } from "../Utils";
const FamilyTreePage: React.FC = () => {
    const [familyName, setFamilyName] = useState<string>('');
    useEffect(() => {
        const fetchFamilyName = async() => {
            setFamilyName(await getClientFamilyName());
        };
        fetchFamilyName();
    }, [familyName])
    return (
        <div>
            <Title />
            <SelectedFileProvider>
                <RevertTreeSection familyName={familyName}/>
            </SelectedFileProvider>
            <FamilyTreeInput includesEntireTree={true}/>
            <FamilyTreeDisplay />
        </div>
    );
}

export default FamilyTreePage;
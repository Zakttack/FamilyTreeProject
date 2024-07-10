import React, { useContext, useEffect} from "react";
//import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
//import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";
import _ from "lodash"
import FamilyTreeInput from "../components/FamilyTreeInput";
import FamilyTreeDisplay from "../components/FamilyTreeDisplay";
import SelectedFileProvider from "../providers/SelectedFileProvider";
import RevertTreeSection from "../components/RevertTreeSection";
import Title from "../components/TitleComponent";
import { getClientFamilyName, StringDefault } from "../Utils";
import FamilyNameContext from "../context/FamilyNameContext";
const FamilyTreePage: React.FC = () => {
    const {name, setName} = useContext(FamilyNameContext);
    useEffect(() => {
        const fetchFamilyName = async() => {
            if (_.isEqual(name, StringDefault)) {
                const familyName = await getClientFamilyName();
                setName(familyName);
            }
        };
        fetchFamilyName();
    }, [name, setName])
    return (
        <div>
            <Title />
            <SelectedFileProvider>
                <RevertTreeSection familyName={name}/>
            </SelectedFileProvider>
            <FamilyTreeInput includesEntireTree={true}/>
            <FamilyTreeDisplay />
        </div>
    );
}

export default FamilyTreePage;
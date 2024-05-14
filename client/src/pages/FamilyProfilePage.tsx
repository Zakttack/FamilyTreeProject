import React, { useEffect, useState} from "react";
import ParentOfSelectedElement from "../components/ParentOfSelectedElementDisplay";
import FamilyInfoElement from "../components/FamilyInfoElementComponent";
import { FamilyDefault, createURL, getClientSelectedFamily, setClientPageTitle } from "../Utils";
import ReportActionsSection from "../components/ReportActionsSection";
import ChildrenOfSelectedElement from "../components/ChildrenOfSelectedElement";
import { useNavigate } from "react-router-dom";
import Title from "../components/TitleComponent";
import './FamilyProfilePage.css';
import FamilyElement from "../models/FamilyElement";

const FamilyProfilePage: React.FC = () => {
    const [selectedFamily, setSelectedFamily] = useState<FamilyElement>(FamilyDefault);
    let navigate = useNavigate();
    useEffect(() => {
        const fetchSelectedFamily = async() => {
            setSelectedFamily(await getClientSelectedFamily());
        }
        fetchSelectedFamily();
    }, [selectedFamily]);

    const handleViewSubtree = async() => {
        await setClientPageTitle(`This is the subtree of ${selectedFamily.member.name}`).then(() => {
            navigate(createURL('/sub-tree', {memberName: selectedFamily.member.name}));
        });
    }

    return (
        <div>
            <Title />
            <ParentOfSelectedElement selectedFamily={selectedFamily}/>
            <FamilyInfoElement selectedFamily={selectedFamily}/>
            <ReportActionsSection selectedFamily={selectedFamily}/>
            <ChildrenOfSelectedElement selectedFamily={selectedFamily}/>
            <div id="family-profile-controls">
                <button type="button" className="family-profile-control" onClick={() => navigate('/family-tree')}>Back To Tree</button>
                <button type="button" className="family-profile-control" onClick={handleViewSubtree}>View Subtree</button>
            </div>
        </div>
    );
};

export default FamilyProfilePage;
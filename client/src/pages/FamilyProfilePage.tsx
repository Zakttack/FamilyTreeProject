import React, { useContext, useEffect} from "react";
import _ from "lodash";
import SelectedFamilyContext from "../context/SelectedFamilyContext";
import ParentOfSelectedElement from "../components/ParentOfSelectedElementDisplay";
import FamilyInfoElement from "../components/FamilyInfoElementComponent";
import { StringDefault, createURL } from "../Utils";
import ReportActionsSection from "../components/ReportActionsSection";
import ChildrenOfSelectedElement from "../components/ChildrenOfSelectedElement";
import { useNavigate } from "react-router-dom";
import TitleContext from "../context/TitleContext";
import Title from "../components/TitleComponent";
import './FamilyProfilePage.css';

const FamilyProfilePage: React.FC = () => {
    const {selectedFamily} = useContext(SelectedFamilyContext);
    const {setTitle} = useContext(TitleContext);
    let navigate = useNavigate();
    useEffect(() => {
        if (!_.isEqual(selectedFamily.member.name, StringDefault) && !_.isEqual(selectedFamily.inLaw.name, StringDefault)) {
            setTitle(`This is the family of ${selectedFamily.member.name} and ${selectedFamily.inLaw.name}`);
        }
        else if (!_.isEqual(selectedFamily.member.name, StringDefault)) {
            setTitle(`This is the family of ${selectedFamily.member.name}`);
        }
        else {
            setTitle('Unknown Family Element');
        }
    }, [selectedFamily, setTitle]);

    const handleViewSubtree = () => {
        setTitle(`This is the subtree of ${selectedFamily.member.name}`);
        navigate(createURL('/sub-tree', {memberName: selectedFamily.member.name}));
    }

    return (
        <div>
            <Title />
            <ParentOfSelectedElement/>
            <FamilyInfoElement/>
            <ReportActionsSection />
            <ChildrenOfSelectedElement />
            <div id="family-profile-controls">
                <button type="button" className="family-profile-control" onClick={() => navigate('/family-tree')}>Back To Tree</button>
                <button type="button" className="family-profile-control" onClick={handleViewSubtree}>View Subtree</button>
            </div>
        </div>
    );
};

export default FamilyProfilePage;
import React, { useContext, useEffect} from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import ChildrenOfSelectedElement from "../components/ChildrenOfSelectedElement";
import FamilyInfoElement from "../components/FamilyInfoElementComponent";
import ParentOfSelectedElement from "../components/ParentOfSelectedElementDisplay";
import Title from "../components/TitleComponent";
import ReportActionsSection from "../components/ReportActionsSection";
import FamilyElementContext from "../contexts/FamilyElementContext";
import TitleContext from "../contexts/TitleContext";
import { StringDefault } from "../Constants";
import { createURL } from "../Utils";
import './FamilyProfilePage.css';

const FamilyProfilePage: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {setTitle} = useContext(TitleContext);
    let navigate = useNavigate();
    useEffect(() => {
        if (!_.isEqual(selectedElement.member.name, StringDefault) && !_.isEqual(selectedElement.inLaw.name, StringDefault)) {
            setTitle(`This is the family of ${selectedElement.member.name} and ${selectedElement.inLaw.name}`);
        }
        else if (!_.isEqual(selectedElement.member.name, StringDefault)) {
            setTitle(`This is the family of ${selectedElement.member.name}`);
        }
        else {
            setTitle('Unknown Family Element');
        }
    }, [selectedElement, setTitle]);

    const handleViewSubtree = () => {
        setTitle(`This is the subtree of ${selectedElement.member.name}`);
        navigate(createURL('/sub-tree', {memberName: selectedElement.member.name}));
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
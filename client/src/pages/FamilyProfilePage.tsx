import React, { useContext, useEffect} from "react";
import ParentOfSelectedElement from "../components/ParentOfSelectedElementDisplay";
import FamilyInfoElement from "../components/FamilyInfoElementComponent";
import { FamilyDefault, createURL, getClientSelectedFamily, setClientPageTitle } from "../Utils";
import ReportActionsSection from "../components/ReportActionsSection";
import ChildrenOfSelectedElement from "../components/ChildrenOfSelectedElement";
import { useNavigate } from "react-router-dom";
import Title from "../components/TitleComponent";
import './FamilyProfilePage.css';
import SelectedFamilyElementContext from "../context/SelectedFamilyElementContext";
import _ from "lodash";
import TitleContext from "../context/TitleContext";
import SelectedFamilyElementProvider from "../providers/SelectedFamilyElementProvider";

const FamilyProfilePage: React.FC = () => {
    const {selectedFamilyElement, changeFamilyElement} = useContext(SelectedFamilyElementContext);
    const {title, setTitle} = useContext(TitleContext);
    let navigate = useNavigate();
    useEffect(() => {
        const fetchSelectedFamily = async() => {
            if (_.isEqual(selectedFamilyElement, FamilyDefault)) {
                changeFamilyElement(await getClientSelectedFamily());
            }
        }
        fetchSelectedFamily();
    }, [selectedFamilyElement, changeFamilyElement]);

    const handleViewSubtree = async() => {
        setTitle(`This is the subtree of ${selectedFamilyElement.member.name}`);
        await setClientPageTitle(title);
        navigate(createURL('/sub-tree', {memberName: selectedFamilyElement.member.name}));
    }

    return (
        <SelectedFamilyElementProvider>
            <Title />
            <ParentOfSelectedElement selectedFamily={selectedFamilyElement}/>
            <FamilyInfoElement selectedFamily={selectedFamilyElement}/>
            <ReportActionsSection selectedFamily={selectedFamilyElement}/>
            <ChildrenOfSelectedElement selectedFamily={selectedFamilyElement}/>
            <div id="family-profile-controls">
                <button type="button" className="family-profile-control" onClick={() => navigate('/family-tree')}>Back To Tree</button>
                <button type="button" className="family-profile-control" onClick={handleViewSubtree}>View Subtree</button>
            </div>
        </SelectedFamilyElementProvider>
    );
};

export default FamilyProfilePage;
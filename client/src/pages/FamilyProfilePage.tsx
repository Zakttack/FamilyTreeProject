import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
import { FamilyElementContext } from "../models/FamilyElement";
import ParentOfSelectedElement from "../components/ParentOfSelectedElementDisplay";
import FamilyInfoElement from "../components/FamilyInfoElementComponent";
import { StringDefault } from "../Utils";
import ReportActionsSection from "../components/ReportActionsSection";

const FamilyProfilePage: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const [title,setTitle] = useState<string>('Unable to find selected family element');
    useEffect(() => {
        if (!_.isEqual(selectedElement.member.name, StringDefault) && !_.isEqual(selectedElement.inLaw.name, StringDefault)) {
            setTitle(`This is the family of ${selectedElement.member.name} and ${selectedElement.inLaw.name}`);
        }
        else if (!_.isEqual(selectedElement.member.name, StringDefault)) {
            setTitle(`This is the family of ${selectedElement.member.name}`);
        }
    }, [selectedElement]);

    return (
        <div>
            <h1>{title}</h1>
            <ParentOfSelectedElement/>
            <FamilyInfoElement/>
            <ReportActionsSection />
        </div>
    );
};

export default FamilyProfilePage;
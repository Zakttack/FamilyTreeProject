import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
import { FamilyElementContext } from "../models/FamilyElement";

const FamilyProfilePage: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const [title,setTitle] = useState<string>('Unable to find selected family element');
    useEffect(() => {
        if (!_.isNull(selectedElement.member.name) && !_.isNull(selectedElement.inLaw) && !_.isNull(selectedElement.inLaw.name)) {
            setTitle(`This is the family of ${selectedElement.member.name} and ${selectedElement.inLaw.name}`);
        }
        else if (!_.isNull(selectedElement.member.name)) {
            setTitle(`This is the family of ${selectedElement.member.name}`);
        }
    }, [selectedElement]);

    return (
        <h1>{title}</h1>
    );
};

export default FamilyProfilePage;
import React, { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import FamilyElement from "../models/FamilyElement";
import RepresentationElement from "../models/RepresentationElement";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import "./FamilyElementDisplay.css"
import OutputResponse from "../models/OutputResponse";
import { createURL, StringDefault, PersonDefault, familyElementToRepresentation, representationToFamilyElement, setClientSelectedFamily, setClientPageTitle } from "../Utils";
import SelectedFamilyElementContext from "../context/SelectedFamilyElementContext";
import TitleContext from "../context/TitleContext";

const FamilyElementDisplay: React.FC<FamilyElement> = (element) => {
    const [representationOutput, setRepresentationOutput] = useState<OutputResponse<RepresentationElement>>({});
    const {selectedFamilyElement, changeFamilyElement} = useContext(SelectedFamilyElementContext);
    const {title, setTitle} = useContext(TitleContext);
    let navigate = useNavigate();

    const handleClick = async(e: React.MouseEvent<HTMLParagraphElement>) => {
        const input = _.isNull(e.currentTarget.textContent) ? StringDefault : e.currentTarget.textContent;
        const response: OutputResponse<FamilyElement> = await representationToFamilyElement({representation: input});
        if (response.problem) {
            throw new Error(response.problem.message);
        }
        else if (response.output) {
            changeFamilyElement(response.output);
            await setClientSelectedFamily(selectedFamilyElement);
            if (_.isEqual(selectedFamilyElement.inLaw, PersonDefault)) {
                setTitle(`This is the family of ${selectedFamilyElement.member.name}`);
                await setClientPageTitle(title);
                navigate(createURL('/family-profile', {member: selectedFamilyElement.member.name}));
            }
            else if (!_.isEqual(selectedFamilyElement.member, PersonDefault) && !_.isEqual(selectedFamilyElement.inLaw, PersonDefault)) {
                setTitle(`This is the family of ${selectedFamilyElement.member.name} and ${selectedFamilyElement.inLaw.name}`)
                await setClientPageTitle(title);
                navigate(createURL('/family-profile', {member: selectedFamilyElement.member.name, inLaw: selectedFamilyElement.inLaw.name}));
            }
        }
    };

    useEffect(() => {
        const handleRender = async () => {
            const response: OutputResponse<RepresentationElement> = await familyElementToRepresentation(element);
            setRepresentationOutput(response);
        };
        handleRender();
    }, [element]);

    if (!_.isUndefined(representationOutput.problem)) {
        return (
            <ErrorDisplayComponent message={representationOutput.problem.message}/>
        );
    }
    else if (!_.isUndefined(representationOutput.output)) {
        return (
            <p className="familyElement" onClick={handleClick}>{representationOutput.output.representation}</p>
        );
    }
    return (
        <ErrorDisplayComponent message="Something Went Wrong"/>
    );
};

export default FamilyElementDisplay;
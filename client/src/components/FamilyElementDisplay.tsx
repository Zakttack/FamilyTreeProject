import React, { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import SelectedFamilyContext from "../context/SelectedFamilyContext";
import FamilyElement from "../models/FamilyElement";
import RepresentationElement from "../models/RepresentationElement";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import "./FamilyElementDisplay.css"
import OutputResponse from "../models/OutputResponse";
import { createURL, StringDefault, familyElementToRepresentation, representationToFamilyElement } from "../Utils";
import { FamilyDefault, PersonDefault } from "../Utils";

const FamilyElementDisplay: React.FC<FamilyElement> = (element) => {
    const {changeSelectedFamily} = useContext(SelectedFamilyContext);
    const [representationOutput, setRepresentationOutput] = useState<OutputResponse<RepresentationElement>>({});
    let navigate = useNavigate();

    const handleClick = async(e: React.MouseEvent<HTMLParagraphElement>) => {
        const input = _.isNull(e.currentTarget.textContent) ? StringDefault : e.currentTarget.textContent;
        const response: OutputResponse<FamilyElement> = await representationToFamilyElement({representation: input});
        changeSelectedFamily(_.isUndefined(response.output) ? FamilyDefault : response.output);
        if (_.isUndefined(response.output) || _.isEqual(response.output.member, PersonDefault)) {
            navigate('/dashboard');
        }
        else if (_.isEqual(response.output.inLaw, PersonDefault)) {
            navigate(createURL('/family-profile', {member: response.output.member.name}));
        }
        else {
            navigate(createURL('/family-profile', {member: response.output.member.name, inLaw: response.output.inLaw.name}));
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
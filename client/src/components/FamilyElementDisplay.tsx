import React, { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import FamilyElement, { FamilyDefault, FamilyElementContext } from "../models/FamilyElement";
import RepresentationElement from "../models/RepresentationElement";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import "./FamilyElementDisplay.css"
import OutputResponse from "../models/outputResponse";
import { elementToRepresentation, representationToElement } from "../Utils";
import { PersonDefault } from "../models/PersonElement";

const FamilyElementDisplay: React.FC<FamilyElement> = (element) => {
    const {changeSelectedElement} = useContext(FamilyElementContext);
    const [representationOutput, setRepresentationOutput] = useState<OutputResponse<RepresentationElement>>({problem: null, output: null});
    let navigate = useNavigate();

    const createURL = (path: string, queryParams = {}) => {
        const queryString = new URLSearchParams(queryParams).toString();
        return `${path}?${queryString}`;
    }

    const handleClick = async(e: React.MouseEvent<HTMLParagraphElement>) => {
        const response: OutputResponse<FamilyElement> = await representationToElement({representation: e.currentTarget.textContent});
        changeSelectedElement(_.isNull(response.output) ? FamilyDefault : response.output);
        if (_.isNull(element.member.name)) {
            navigate('/dashboard');
        }
        else if (_.isEqual(element.inLaw, PersonDefault)) {
            navigate(createURL('/family-profile', {member: element.member.name}));
        }
        else {
            navigate(createURL('/family-profile', {member: element.member.name, inLaw: element.inLaw.name}));
        }
    };

    useEffect(() => {
        const handleRender = async () => {
            const response: OutputResponse<RepresentationElement> = await elementToRepresentation(element);
            setRepresentationOutput(response);
        };
        handleRender();
    }, [element]);

    if (!_.isNull(representationOutput.problem)) {
        return (
            <ErrorDisplayComponent message={representationOutput.problem.message}/>
        );
    }
    else if (!_.isNull(representationOutput.output)) {
        return (
            <p className="familyElement" onClick={handleClick}>{representationOutput.output.representation}</p>
        );
    }
    return (
        <ErrorDisplayComponent message="Something Went Wrong"/>
    );
};

export default FamilyElementDisplay;
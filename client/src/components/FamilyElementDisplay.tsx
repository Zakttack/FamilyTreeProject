import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import FamilyElement from "../models/FamilyElement";
import RepresentationElement from "../models/RepresentationElement";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import "./FamilyElementDisplay.css"
import OutputResponse from "../models/OutputResponse";
import { createURL, StringDefault, PersonDefault, familyElementToRepresentation, representationToFamilyElement, setClientSelectedFamily, setClientPageTitle } from "../Utils";

const FamilyElementDisplay: React.FC<FamilyElement> = (element) => {
    const [representationOutput, setRepresentationOutput] = useState<OutputResponse<RepresentationElement>>({});
    let navigate = useNavigate();

    const handleClick = async(e: React.MouseEvent<HTMLParagraphElement>) => {
        const input = _.isNull(e.currentTarget.textContent) ? StringDefault : e.currentTarget.textContent;
        const response: OutputResponse<FamilyElement> = await representationToFamilyElement({representation: input});
        if (response.problem) {
            throw new Error(response.problem.message);
        }
        else if (response.output) {
            const selectedFamilyAdjustmentTask: Promise<void> = setClientSelectedFamily(response.output);
            if (_.isEqual(response.output.member, PersonDefault)) {
                await selectedFamilyAdjustmentTask.then(() => {
                    window.location.reload();
                });
            }
            else if (_.isEqual(response.output.inLaw, PersonDefault)) {
                await selectedFamilyAdjustmentTask.then(() => {
                    setClientPageTitle(`This is the family of ${response.output?.member.name}`);
                }).then(() => {
                    navigate(createURL('/family-profile', {member: response.output?.member.name}));
                });
            }
            else {
                await selectedFamilyAdjustmentTask.then(() => {
                    setClientPageTitle(`This is the family of ${response.output?.member.name} and ${response.output?.inLaw.name}`);
                }).then(() => {
                    navigate(createURL('/family-profile', {member: response.output?.member.name, inLaw: response.output?.inLaw.name}));
                });
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
import React, { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import FamilyElement from "../models/FamilyElement";
import { FamilyDefault, StringDefault, PersonDefault, EmptyResponse } from "../Constants";
import FamilyElementContext from "../contexts/FamilyElementContext";
import RepresentationElement from "../models/RepresentationElement";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import "./FamilyElementDisplay.css"
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { familyElementToRepresentation, representationToFamilyElement } from "../ApiCalls";
import { createURL, isProcessing, isSuccess } from "../Utils";
import useLoadingContext from "../hooks/useLoadingContext";
import { LoadingContext } from "../Enums";
import LoadingComponent from "./LoadingComponent";

const FamilyElementDisplay: React.FC<FamilyElement> = (element) => {
    const {selectedElement, changeSelectedElement} = useContext(FamilyElementContext);
    const {addLoadingContext, removeLoadingContext, isLoading} = useLoadingContext();
    const [representationOutput, setRepresentationOutput] = useState<FamilyTreeApiResponse>(EmptyResponse);
    let navigate = useNavigate();

    const handleClick = async(e: React.MouseEvent<HTMLParagraphElement>) => {
        if (!isLoading()) {
            const input = _.isNull(e.currentTarget.textContent) ? StringDefault : e.currentTarget.textContent;
            addLoadingContext(LoadingContext.RepresentationToFamilyElement);
            const response = await representationToFamilyElement({representation: input});
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.RepresentationToFamilyElement);
                changeSelectedElement(isSuccess(response) && response.result ? response.result as FamilyElement : FamilyDefault);
                if (!isSuccess(response) || _.isEqual(selectedElement.member, PersonDefault)) {
                    navigate('/dashboard');
                }
                else if (_.isEqual(selectedElement.inLaw, PersonDefault)) {
                    navigate(createURL('/family-profile', {member: selectedElement.member.name}));
                }
                else {
                    navigate(createURL('/family-profile', {member: selectedElement.member.name, inLaw: selectedElement.inLaw.name}));
                }
            }
        }
    };

    useEffect(() => {
        const handleRender = async () => {
            addLoadingContext(LoadingContext.FamilyElementToRepresentation);
            const response = await familyElementToRepresentation(element);
            setRepresentationOutput(response);
            if (!isProcessing(representationOutput)) {
                removeLoadingContext(LoadingContext.FamilyElementToRepresentation);
            }
        };
        handleRender();
    }, [element, addLoadingContext, removeLoadingContext, representationOutput]);
    return (
        <>
            <LoadingComponent context={LoadingContext.FamilyElementToRepresentation} response={representationOutput}/>
            <ErrorDisplayComponent response={representationOutput} />
            {isSuccess(representationOutput) && <p className="familyElement" onClick={handleClick}>{(representationOutput.result as RepresentationElement).representation}</p>}
        </>
    )
};

export default FamilyElementDisplay;
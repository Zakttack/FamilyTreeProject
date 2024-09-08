import React, { useEffect, useState } from "react";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import LoadingComponent from "./LoadingComponent";
import useLoadingContext from "../hooks/useLoadingContext";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import RepresentationElement from "../models/RepresentationElement";
import PersonInfoInput from "../models/PersonInfoInput";
import { personElementToRepresentation } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";
import './PersonInfoElementComponent.css';


const PersonInfoElement: React.FC<PersonInfoInput> = (input) => {
    const [personInfoResult, setPersonInfoResult] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [personInfoShown, showPersonInfo] = useState<boolean>(false);
    const handleChecked = () => {
        showPersonInfo(!personInfoShown);
    };
    useEffect(() => {
        const handleRender = async() => {
            addLoadingContext(LoadingContext.PersonElementToRepresentation);
            setPersonInfoResult(await personElementToRepresentation(input.element));
            if (!isProcessing(personInfoResult)) {
                removeLoadingContext(LoadingContext.PersonElementToRepresentation);
            }
        };
        handleRender();
    }, [input, addLoadingContext, removeLoadingContext, personInfoResult]);
    return (
        <div>
            <h3>{input.type}:</h3>
            <LoadingComponent context={LoadingContext.PersonElementToRepresentation} response={personInfoResult} />
            <ErrorDisplayComponent response={personInfoResult} />
            {isSuccess(personInfoResult) && (
                <p>{(personInfoResult.result as RepresentationElement).representation}<span id="checkBoxPadder"><label><input type="checkbox" checked={personInfoShown} onChange={handleChecked}/>Show Person Info</label></span></p>
            )}
            {personInfoShown && (
                <div>
                    <p>Name: {input.element.name}</p>
                    <p>Birth Date: {input.element.birthDate}</p>
                    <p>Deceased Date: {input.element.deceasedDate}</p>
                </div>
            )}
        </div>
    )
};

export default PersonInfoElement;
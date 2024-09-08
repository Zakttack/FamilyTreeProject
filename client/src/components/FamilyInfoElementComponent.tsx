import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
import FamilyElementContext from "../contexts/FamilyElementContext";
import PersonInfoElement from "./PersonInfoElementComponent";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { generationNumberOf } from "../ApiCalls";
import { LoadingContext, PersonType } from "../Enums";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import { EmptyResponse, StringDefault } from "../Constants";
import useLoadingContext from "../hooks/useLoadingContext";
import { isProcessing, isSuccess } from "../Utils";
import LoadingComponent from "./LoadingComponent";

const FamilyInfoElement: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [generationNumberResponse, setGenerationNumberResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);

    useEffect(() => {
        const handleGenerationNumberOf = async() => {
            addLoadingContext(LoadingContext.GenerationNumber);
            const response = await generationNumberOf(selectedElement);
            setGenerationNumberResponse(response);
            if (!isProcessing(generationNumberResponse)) {
                removeLoadingContext(LoadingContext.GenerationNumber);
            }
        };
        handleGenerationNumberOf();
    }, [selectedElement, addLoadingContext, removeLoadingContext, generationNumberResponse]);
    return (
        <div>
            <h2>Info:</h2>
            <LoadingComponent context={LoadingContext.GenerationNumber} response={generationNumberResponse} />
            <ErrorDisplayComponent response={generationNumberResponse}/>
            {isSuccess(generationNumberResponse) && (
                <>
                    <h3>Generation Number: {generationNumberResponse.result as number}</h3>
                    <PersonInfoElement type={PersonType.Member} element={selectedElement.member}/>
                    <PersonInfoElement type={PersonType.InLaw} element={selectedElement.inLaw}/>
                    <h3>Marriage Date: {_.isNull(selectedElement.marriageDate) ? StringDefault : selectedElement.marriageDate}</h3>
                </>
            )}
        </div>
    );
};

export default FamilyInfoElement;

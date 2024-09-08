import React, { useContext, useEffect, useState } from "react";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FamilyElementDisplay from "./FamilyElementDisplay";
import LoadingComponent from "./LoadingComponent";
import FamilyElementContext from "../contexts/FamilyElementContext";
import useLoadingContext from "../hooks/useLoadingContext";
import FamilyElement from "../models/FamilyElement";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { retrieveParent } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";

const ParentOfSelectedElement: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [parentResult,setParentResult] = useState<FamilyTreeApiResponse>(EmptyResponse);
    useEffect(() => {
        const getParentElement = async () => {
            addLoadingContext(LoadingContext.RetrieveParent);
            setParentResult(await retrieveParent(selectedElement));
            if (!isProcessing(parentResult)) {
                removeLoadingContext(LoadingContext.RetrieveParent);
            }
        }
        getParentElement();
    }, [selectedElement, addLoadingContext, removeLoadingContext, parentResult]);

    return (
        <div>
            <h2>Parent:</h2>
            <LoadingComponent context={LoadingContext.RetrieveParent} response={parentResult} />
            <ErrorDisplayComponent response={parentResult} />
            {isSuccess(parentResult) && <FamilyElementDisplay member={(parentResult.result as FamilyElement).member} inLaw={(parentResult.result as FamilyElement).inLaw} marriageDate={(parentResult.result as FamilyElement).marriageDate}/>}
        </div>
    );
};

export default ParentOfSelectedElement;
import React, { useContext, useEffect, useState } from "react";
import FamilyElementContext from "../contexts/FamilyElementContext";
import FamilyElement from "../models/FamilyElement";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { retrieveChildren } from "../ApiCalls";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FamilyElementDisplay from "./FamilyElementDisplay";
import { EmptyResponse } from "../Constants";
import { isProcessing, isSuccess } from "../Utils";
import useLoadingContext from "../hooks/useLoadingContext";
import { LoadingContext } from "../Enums";
import LoadingComponent from "./LoadingComponent";

const ChildrenOfSelectedElement: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [childrenResponse, setChildrenResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);

    useEffect(() => {
        const fetchChildren = async() => {
            addLoadingContext(LoadingContext.RetrieveChildren);
            setChildrenResponse(await retrieveChildren(selectedElement));
            if (!isProcessing(childrenResponse)) {
                removeLoadingContext(LoadingContext.ReportChildren);
            }
        };
        fetchChildren();
    }, [selectedElement, addLoadingContext, removeLoadingContext, childrenResponse]);

    return (
        <div>
            <h2>Children:</h2>
            <LoadingComponent context={LoadingContext.RetrieveChildren} response={childrenResponse} />
            <ErrorDisplayComponent response={childrenResponse} />
            {isSuccess(childrenResponse) && (
                <>
                {(childrenResponse.result as FamilyElement[]).map((element: FamilyElement) => (
                    <FamilyElementDisplay member={element.member} inLaw={element.inLaw} marriageDate={element.marriageDate}/>
                ))}
                </>
            )}
        </div>
    );
};

export default ChildrenOfSelectedElement;
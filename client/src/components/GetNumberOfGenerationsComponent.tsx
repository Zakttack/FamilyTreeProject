import React, {useContext, useEffect, useState} from "react";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import LoadingComponent from "./LoadingComponent";
import FamilyNameContext from "../contexts/FamilyNameContext";
import useLoadingContext from "../hooks/useLoadingContext";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { getNumberOfGenerations } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";

const GetNumberOfGenerationsComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [numericOutput, setNumericOutput] = useState<FamilyTreeApiResponse>(EmptyResponse);
    useEffect(() => {
        const fetchNumberOfGenerations = async () => {
            addLoadingContext(LoadingContext.NumberOfGenerations);
            setNumericOutput(await getNumberOfGenerations());
            if (!isProcessing(numericOutput)) {
                removeLoadingContext(LoadingContext.NumberOfGenerations);
            }
        };
        fetchNumberOfGenerations();
    }, [familyName, addLoadingContext, removeLoadingContext, numericOutput]);
    return (
        <>
            <LoadingComponent context={LoadingContext.NumberOfGenerations} response={numericOutput} />
            <ErrorDisplayComponent response={numericOutput} />
            {isSuccess(numericOutput) && <p>There are {numericOutput.result as number} generations in the {familyName} family tree.</p>}
        </>
    )
}

export default GetNumberOfGenerationsComponent;
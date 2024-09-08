import React, {useContext, useEffect, useState} from "react";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import LoadingComponent from "./LoadingComponent";
import FamilyNameContext from "../contexts/FamilyNameContext";
import useLoadingContext from "../hooks/useLoadingContext";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { getNumberOfFamilies } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";

const GetNumberOfFamiliesComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();
    const [numericOutput, setNumericOutput] = useState<FamilyTreeApiResponse>(EmptyResponse);
    useEffect(() => {
        const fetchNumberOfFamilies = async () => {
            addLoadingContext(LoadingContext.NumberOfFamilies);
            setNumericOutput(await getNumberOfFamilies());
            if (!isProcessing(numericOutput)) {
                removeLoadingContext(LoadingContext.NumberOfFamilies);
            }
        };
        fetchNumberOfFamilies();
    }, [familyName, addLoadingContext, removeLoadingContext, numericOutput]);
    return (
        <>
            <LoadingComponent context={LoadingContext.NumberOfFamilies} response={numericOutput} />
            <ErrorDisplayComponent response={numericOutput} />
            {isSuccess(numericOutput) && <p>There are {numericOutput.result as number} families in the {familyName} family.</p>}
        </>
    )
};

export default GetNumberOfFamiliesComponent;
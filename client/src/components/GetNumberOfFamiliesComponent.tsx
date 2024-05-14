import React, {useEffect, useState} from "react";
import OutputResponse from "../models/OutputResponse";
import { NumberDefault, getNumberOfFamilies } from "../Utils";
import ClientFamilyNameElement from "../models/ClientFamilyNameElement";

const GetNumberOfFamiliesComponent: React.FC<ClientFamilyNameElement> = (params) => {
    const [numericOutput, setNumericOutput] = useState<OutputResponse<number>>({});
    useEffect(() => {
        const fetchNumberOfFamilies = async () => {
            const response: OutputResponse<number> = await getNumberOfFamilies();
            setNumericOutput(response);
        };
        fetchNumberOfFamilies();
    }, [params.familyName]);
    return (
        <p>There are {numericOutput.output ? NumberDefault : numericOutput.output} families in the {params.familyName} family.</p>
    )
};

export default GetNumberOfFamiliesComponent;
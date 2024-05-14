import React, {useEffect, useState} from "react";
import OutputResponse from "../models/OutputResponse";
import { NumberDefault, getNumberOfGenerations } from "../Utils";
import ClientFamilyNameElement from "../models/ClientFamilyNameElement";

const GetNumberOfGenerationsComponent: React.FC<ClientFamilyNameElement> = (params) => {
    const [numericOutput, setNumericOutput] = useState<OutputResponse<number>>({});
    useEffect(() => {
        const fetchNumberOfGenerations = async () => {
            const response: OutputResponse<number> = await getNumberOfGenerations();
            setNumericOutput(response);
        };
        fetchNumberOfGenerations();
    }, [params.familyName]);
    return (
        <p>There are {numericOutput.output ? NumberDefault : numericOutput.output} generations in the {params.familyName} family tree.</p>
    )
}

export default GetNumberOfGenerationsComponent;
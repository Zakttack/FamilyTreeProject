import React, {useContext, useEffect, useState} from "react";
import FamilyNameContext from "../models/familyNameContext";
import MessageResponse from "../models/MessageResponse";
import OutputResponse from "../models/outputResponse";

const GetNumberOfGenerationsComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const [numericOutput, setNumericOutput] = useState<OutputResponse<number>>({output: null, problem: null});
    useEffect(() => {
        const fetchNumberOfGenerations = async () => {
            const url = `http://localhost:5201/api/familytree/${familyName}/numberofgenerations`;
            const response = await fetch(url);
            if (!response.ok)
            {
                const errorOutput: MessageResponse = await response.json();
                setNumericOutput({output: null, problem: errorOutput});
            }
            else {
                const output: number = await response.json();
                setNumericOutput({output: output, problem: null});
            }
        };
        fetchNumberOfGenerations();
    }, [familyName]);
    return (
        <p>There are {numericOutput.output} generations in the {familyName} family tree.</p>
    )
}

export default GetNumberOfGenerationsComponent;
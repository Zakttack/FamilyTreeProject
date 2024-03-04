import React, {useContext, useEffect, useState} from "react";
import FamilyNameContext from "../models/familyNameContext";
import MessageResponse from "../models/MessageResponse";
import OutputResponse from "../models/outputResponse";

const GetNumberOfFamiliesComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const [numericOutput, setNumericOutput] = useState<OutputResponse<number>>({output: null, problem: null});
    useEffect(() => {
        const fetchNumberOfFamilies = async () => {
            const url = `http://localhost:5201/api/familytree/${familyName}/numberoffamilies`;
            const response = await fetch(url);
            if (!response.ok) {
                const errorData: MessageResponse = await response.json();
                setNumericOutput({output: null, problem: errorData});
            }
            else {
                const value: number = await response.json();
                setNumericOutput({output: value, problem: null});
            }
        };
        fetchNumberOfFamilies();
    }, [familyName]);
    return (
        <p>There are {numericOutput.output} families in the {familyName} family.</p>
    )
};

export default GetNumberOfFamiliesComponent;
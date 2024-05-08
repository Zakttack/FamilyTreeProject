import React, {useContext, useEffect, useState} from "react";
import FamilyNameContext from "../context/FamilyNameContext";
import OutputResponse from "../models/OutputResponse";
import { NumberDefault, getNumberOfFamilies } from "../Utils";

const GetNumberOfFamiliesComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const [numericOutput, setNumericOutput] = useState<OutputResponse<number>>({});
    useEffect(() => {
        const fetchNumberOfFamilies = async () => {
            const response: OutputResponse<number> = await getNumberOfFamilies();
            setNumericOutput(response);
        };
        fetchNumberOfFamilies();
    }, [familyName]);
    return (
        <p>There are {numericOutput.output ? NumberDefault : numericOutput.output} families in the {familyName} family.</p>
    )
};

export default GetNumberOfFamiliesComponent;
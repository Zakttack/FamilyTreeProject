import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
import SelectedFamilyContext from "../context/SelectedFamilyContext";
import PersonInfoElement from "./PersonInfoElementComponent";
import { PersonType } from "../enums/PersonType";
import OutputResponse from "../models/OutputResponse";
import { StringDefault, generationNumberOf } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";

const FamilyInfoElement: React.FC = () => {
    const {selectedFamily} = useContext(SelectedFamilyContext);
    const [generationNumberResponse, setGenerationNumberResponse] = useState<OutputResponse<number>>({});

    useEffect(() => {
        const handleGenerationNumberOf = async() => {
            const response: OutputResponse<number> = await generationNumberOf(selectedFamily);
            setGenerationNumberResponse(response);
        };
        handleGenerationNumberOf();
    }, [selectedFamily]);
    if (generationNumberResponse.problem) {
        return (
            <ErrorDisplayComponent message={generationNumberResponse.problem.message}/>
        );
    }
    else if (generationNumberResponse.output) {
        return (
            <div>
                <h2>Info:</h2>
                <h3>Generation Number: {generationNumberResponse.output}</h3>
                <PersonInfoElement type={PersonType.Member} element={selectedFamily.member}/>
                <PersonInfoElement type={PersonType.InLaw} element={selectedFamily.inLaw}/>
                <h3>Marriage Date: {_.isNull(selectedFamily.marriageDate) ? StringDefault : selectedFamily.marriageDate}</h3>
            </div>
        );
    }
    return (
        <h2>Loading Element...</h2>
    )
};

export default FamilyInfoElement;

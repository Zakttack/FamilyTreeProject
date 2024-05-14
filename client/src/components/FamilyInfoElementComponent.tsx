import React, { useEffect, useState } from "react";
import _ from "lodash";
import PersonInfoElement from "./PersonInfoElementComponent";
import { PersonType } from "../enums/PersonType";
import OutputResponse from "../models/OutputResponse";
import { StringDefault, generationNumberOf } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import ClientSelectedFamilyElement from "../models/ClientSelectedFamilyElement";

const FamilyInfoElement: React.FC<ClientSelectedFamilyElement> = (params) => {
    const [generationNumberResponse, setGenerationNumberResponse] = useState<OutputResponse<number>>({});

    useEffect(() => {
        const handleGenerationNumberOf = async() => {
            const response: OutputResponse<number> = await generationNumberOf(params.selectedFamily);
            setGenerationNumberResponse(response);
        };
        handleGenerationNumberOf();
    }, [params.selectedFamily]);
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
                <PersonInfoElement type={PersonType.Member} element={params.selectedFamily.member}/>
                <PersonInfoElement type={PersonType.InLaw} element={params.selectedFamily.inLaw}/>
                <h3>Marriage Date: {_.isNull(params.selectedFamily.marriageDate) ? StringDefault : params.selectedFamily.marriageDate}</h3>
            </div>
        );
    }
    return (
        <h2>Loading Element...</h2>
    )
};

export default FamilyInfoElement;

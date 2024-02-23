import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
// import FamilyNameContext from "../models/familyNameContext";
// import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
// import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";
// import OrderTypeProvider from "../providers/orderTypeProvider";
// import SelectOrderTypeComponent from "../components/SelectOrderTypeComponent";
// import FamilyTreeDisplayComponent from "../components/FamilyTreeDisplayComponent";
import FamilyRepresentationElementContext from "../models/familyRepresentationElementContext";
import ExceptionResponse from "../models/exceptionResponse";

interface PersonElement {
    name: string | null;
    birthDate: string | null;
    deceasedDate: string | null;
}

interface FamilyElement {
    member: PersonElement;
    inLaw: PersonElement | null;
    marriageDate: string | null;
}

interface FamilyProfileOutput {
    problem: ExceptionResponse | null;
    result: FamilyElement | null;
}

const FamilyDashboard: React.FC = () => {
    //const {familyName} = useContext(FamilyNameContext);
    const {representationElement} = useContext(FamilyRepresentationElementContext);
    const [output, setOutput] = useState<FamilyProfileOutput>({problem: null, result: null});
    let familyDashboardPageTitle: string = 'Something went wrong';
    useEffect(() => {
        const analyzeRepresentation = async () => {
            const url = 'http://localhost:5201/api/subfamilytree';
            const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(representationElement),
                }
            );
            if (!response.ok) {
                const errorOutput: ExceptionResponse = await response.json();
                setOutput({problem: errorOutput, result: null});
            }
            else {
                const profileOutput: FamilyElement = await response.json();
                setOutput({problem: null, result: profileOutput});
            }
        };
        analyzeRepresentation();
    }, [representationElement]);
    if (!_.isNull(output.result) && !_.isNull(output.result.member.name) && !_.isNull(output.result.inLaw) && !_.isNull(output.result.inLaw.name)) {
        familyDashboardPageTitle = `This is the family of ${output.result.member.name} and ${output.result.inLaw.name}.`
    }
    else if (!_.isNull(output.result) && !_.isNull(output.result.member.name)) {
        familyDashboardPageTitle = `This is the family of ${output.result.member.name}.`;
    }
    else if (!_.isNull(output.result)) {
        familyDashboardPageTitle = `Representation result is: ${representationElement.representation}.`
    }
    return (
        <h1>{familyDashboardPageTitle}</h1>
    )
};

export default FamilyDashboard;
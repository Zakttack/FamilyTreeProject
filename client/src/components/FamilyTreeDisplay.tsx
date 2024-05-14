import React, { useEffect, useState } from "react";
import _ from "lodash";
import FamilyElementDisplay from "./FamilyElementDisplay";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import OutputResponse from "../models/OutputResponse";
import FamilyElement from "../models/FamilyElement";
import { getClientFamilyTree } from "../Utils";

const FamilyTreeDisplay: React.FC = () => {
    const [familyTreeResponse, setFamilyTreeResponse] = useState<OutputResponse<FamilyElement[]>>({});
    useEffect(() => {
        const fetchFamilyTree = async() => {
            setFamilyTreeResponse(await getClientFamilyTree());
        }
        fetchFamilyTree();
    }, [familyTreeResponse])
    return (
        <>
            {!_.isUndefined(familyTreeResponse.problem) && (
                <ErrorDisplayComponent message={familyTreeResponse.problem.message}/>
            )}
            {!_.isUndefined(familyTreeResponse.output) && (
                <div>
                    {familyTreeResponse.output.map((family) => (
                        <FamilyElementDisplay member={family.member} inLaw={family.inLaw} marriageDate={family.marriageDate} />
                    ))}
                </div>
            )}
        </>
    );
};

export default FamilyTreeDisplay;
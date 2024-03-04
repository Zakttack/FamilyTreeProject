import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
import FamilyElement, { FamilyElementContext } from "../models/FamilyElement";
import FamilyNameContext from "../models/familyNameContext";
import OutputResponse from "../models/outputResponse";
import { retrieveParent } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FamilyElementDisplay from "./FamilyElementDisplay";

const ParentOfSelectedElement: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {selectedElement} = useContext(FamilyElementContext);
    const [parentResult,setParentResult] = useState<OutputResponse<FamilyElement>>({output: null, problem: null});
    useEffect(() => {
        const getParentElement = async () => {
            const response: OutputResponse<FamilyElement> = await retrieveParent(familyName, selectedElement);
            setParentResult(response);
        }
        getParentElement();
    }, [familyName,selectedElement]);

    return (
        <div>
            <h2>Parent:</h2>
            {!_.isNull(parentResult.problem) && <ErrorDisplayComponent message={parentResult.problem.message}/>}
            {!_.isNull(parentResult.output) && <FamilyElementDisplay member={parentResult.output.member} inLaw={parentResult.output.inLaw} marriageDate={parentResult.output.marriageDate}/>}
        </div>
    );
};

export default ParentOfSelectedElement;
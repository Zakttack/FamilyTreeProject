import React, { useContext, useEffect, useState } from "react";
import _ from "lodash";
import SelectedFamilyContext from "../context/SelectedFamilyContext";
import FamilyElement from "../models/FamilyElement";
import OutputResponse from "../models/OutputResponse";
import { retrieveParent } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FamilyElementDisplay from "./FamilyElementDisplay";

const ParentOfSelectedElement: React.FC = () => {
    const {selectedFamily} = useContext(SelectedFamilyContext);
    const [parentResult,setParentResult] = useState<OutputResponse<FamilyElement>>({});
    useEffect(() => {
        const getParentElement = async () => {
            const response: OutputResponse<FamilyElement> = await retrieveParent(selectedFamily);
            setParentResult(response);
        }
        getParentElement();
    }, [selectedFamily]);

    return (
        <div>
            <h2>Parent:</h2>
            {!_.isUndefined(parentResult.problem) && <ErrorDisplayComponent message={parentResult.problem.message}/>}
            {!_.isUndefined(parentResult.output) && <FamilyElementDisplay member={parentResult.output.member} inLaw={parentResult.output.inLaw} marriageDate={parentResult.output.marriageDate}/>}
        </div>
    );
};

export default ParentOfSelectedElement;
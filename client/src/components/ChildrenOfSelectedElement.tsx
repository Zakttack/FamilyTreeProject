import React, { useContext, useEffect, useState } from "react";
import SelectedFamilyContext from "../context/SelectedFamilyContext";
import FamilyElement from "../models/FamilyElement";
import OutputResponse from "../models/OutputResponse";
import { retrieveChildren } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FamilyElementDisplay from "./FamilyElementDisplay";

const ChildrenOfSelectedElement: React.FC = () => {
    const {selectedFamily} = useContext(SelectedFamilyContext);
    const [childrenResponse, setChildrenResponse] = useState<OutputResponse<FamilyElement[]>>({});

    useEffect(() => {
        const fetchChildren = async() => {
            const response: OutputResponse<FamilyElement[]> = await retrieveChildren(selectedFamily);
            setChildrenResponse(response);
        };
        fetchChildren();
    }, [selectedFamily]);

    return (
        <div>
            <h2>Children:</h2>
            {childrenResponse.problem && <ErrorDisplayComponent message={childrenResponse.problem.message} />}
            {childrenResponse.output && (
                <>
                {childrenResponse.output.map((element: FamilyElement) => (
                    <FamilyElementDisplay member={element.member} inLaw={element.inLaw} marriageDate={element.marriageDate}/>
                ))}
                </>
            )}
            {(!childrenResponse.problem && !childrenResponse.output) && <h3>Loading Children...</h3>}
        </div>
    );
};

export default ChildrenOfSelectedElement;
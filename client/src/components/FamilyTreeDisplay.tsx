import React, { useContext, useEffect } from "react";
import _ from "lodash";
import FamilyElementDisplay from "./FamilyElementDisplay";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import { getClientFamilyTree } from "../Utils";
import FamilyTreeContext from "../context/FamilyTreeContext";
import SelectedFamilyElementProvider from "../providers/SelectedFamilyElementProvider";

const FamilyTreeDisplay: React.FC = () => {
    const {familyTreeResponse, setFamilyTreeResponse} = useContext(FamilyTreeContext);
    useEffect(() => {
        const fetchFamilyTree = async() => {
            if (_.isEqual(familyTreeResponse, {})) {
                setFamilyTreeResponse(await getClientFamilyTree());
            }
        }
        fetchFamilyTree();
    }, [familyTreeResponse, setFamilyTreeResponse])
    return (
        <>
            {!_.isUndefined(familyTreeResponse.problem) && (
                <ErrorDisplayComponent message={familyTreeResponse.problem.message}/>
            )}
            {!_.isUndefined(familyTreeResponse.output) && (
                <SelectedFamilyElementProvider>
                    {familyTreeResponse.output.map((family) => (
                        <FamilyElementDisplay member={family.member} inLaw={family.inLaw} marriageDate={family.marriageDate} />
                    ))}
                </SelectedFamilyElementProvider>
            )}
        </>
    );
};

export default FamilyTreeDisplay;
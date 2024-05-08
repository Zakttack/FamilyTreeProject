import React, { useContext} from "react";
import _ from "lodash";
import FamilyElementDisplay from "./FamilyElementDisplay";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FamilyTreeContext from "../context/FamilyTreeContext";

const FamilyTreeDisplay: React.FC = () => {
    const {familyTreeResponse} = useContext(FamilyTreeContext);

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
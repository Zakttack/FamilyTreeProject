import React, { useContext} from "react";
import FamilyElementDisplay from "./FamilyElementDisplay";
import FamilyTreeContext from "../contexts/FamilyTreeContext";

const FamilyTreeDisplay: React.FC = () => {
    const {familyTree} = useContext(FamilyTreeContext);

    return (
        <div>
            {familyTree.map((family) => (
                <FamilyElementDisplay member={family.member} inLaw={family.inLaw} marriageDate={family.marriageDate} />
            ))}
        </div>
    );
};

export default FamilyTreeDisplay;
import React, {useState} from "react";
import ProviderProps from "../models/ProviderProps";
import FamilyTreeContext from "../contexts/FamilyTreeContext";
import FamilyElement from "../models/FamilyElement";

const FamilyTreeProvider: React.FC<ProviderProps> = ({children}) => {
    const [familyTree, setFamilyTree] = useState<FamilyElement[]>([]);

    return (
        <FamilyTreeContext.Provider value={{familyTree, setFamilyTree}}>
                {children}
        </FamilyTreeContext.Provider>
    )
};

export default FamilyTreeProvider;
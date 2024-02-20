import React, {useState} from "react";
import FamilyRepresenatationElement, { FamilyRepresentationDefault } from "../models/familyRepresentationElement";
import { ProviderProps } from "../models/providerProps";
import FamilyRepresenationElementContext from "../models/familyRepresentationElementContext";

const FamilyRepresentionElementProvider: React.FC<ProviderProps> = ({children}) => {
    const [familyRepresentationElement, setFamilyRepresenatationElement] = useState<FamilyRepresenatationElement>(FamilyRepresentationDefault);
    return (
        <FamilyRepresenationElementContext.Provider value={{representationElement: familyRepresentationElement, setRepresentationElement: setFamilyRepresenatationElement}}>
            {children}
        </FamilyRepresenationElementContext.Provider>
    )
};

export default FamilyRepresentionElementProvider;
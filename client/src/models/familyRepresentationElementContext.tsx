import React from "react";
import FamilyRepresenationElement, {FamilyRepresentationDefault} from "./familyRepresentationElement";
interface FamilyRepresentationElementContextType {
    representationElement: FamilyRepresenationElement;
    setRepresentationElement: (representationElement: FamilyRepresenationElement) => void;
}

const FamilyRepresentationElementContext = React.createContext<FamilyRepresentationElementContextType>({representationElement: FamilyRepresentationDefault, setRepresentationElement: () => {}});

export default FamilyRepresentationElementContext;
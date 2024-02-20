import React from "react";
import FamilyRepresenationElement, {FamilyRepresentationDefault} from "./familyRepresentationElement";
interface FamilyRepresentationElementContextType {
    representationElement: FamilyRepresenationElement;
    setRepresentationElement: (representationElement: FamilyRepresenationElement) => void;
}

const FamilyRepresenatationElementContext = React.createContext<FamilyRepresentationElementContextType>({representationElement: FamilyRepresentationDefault, setRepresentationElement: () => {}});

export default FamilyRepresenatationElementContext;
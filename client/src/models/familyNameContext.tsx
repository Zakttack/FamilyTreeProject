import React from "react";
import { StringDefault } from "../Utils";
interface FamilyNameContextType {
    familyName: string;
    setFamilyName: (name: string) => void;
}

const FamilyNameContext = React.createContext<FamilyNameContextType>({familyName: StringDefault, setFamilyName: () => {}});

export default FamilyNameContext;
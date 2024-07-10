import React from "react";
import { StringDefault } from "../Utils";

interface FamilyNameContextType {
    name: string;
    setName: (name: string) => void;
}

const FamilyNameContext = React.createContext<FamilyNameContextType>({name: StringDefault, setName: () => {}});
export default FamilyNameContext;
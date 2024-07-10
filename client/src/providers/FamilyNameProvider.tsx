import React, {useState} from "react";
import { ProviderProps } from "../models/ProviderProps";
import { StringDefault } from "../Utils";
import FamilyNameContext from "../context/FamilyNameContext";
 
const FamilyNameProvider: React.FC<ProviderProps> = ({children}) => {
    const [name, setName] = useState<string>(StringDefault);
    return (
        <FamilyNameContext.Provider value={{name, setName}}>
            {children}
        </FamilyNameContext.Provider>
    )
};
export default FamilyNameProvider;
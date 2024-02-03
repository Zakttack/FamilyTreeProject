import React, {useState, ReactNode} from "react";
import FamilyNameContext from "../models/familyNameContext";

interface NameProviderProps {
    children: ReactNode;
}

const FamilyNameProvider: React.FC<NameProviderProps> = ({children}) => {
    const [familyName, setFamilyName] = useState<string>('');
    return (
        <FamilyNameContext.Provider value={{familyName, setFamilyName}}>
            {children}
        </FamilyNameContext.Provider>
    );
};

export default FamilyNameProvider;
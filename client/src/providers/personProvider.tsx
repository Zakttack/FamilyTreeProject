import React, {useState} from "react";
import PersonContext, { PersonDefault, PersonElement } from "../models/personContext";
import { ProviderProps } from "../models/providerProps";

const FamilyNameProvider: React.FC<ProviderProps> = ({children}) => {
    const [person, setPerson] = useState<PersonElement>(PersonDefault);
    return (
        <PersonContext.Provider value={{person: person, setPerson: setPerson}}>
            {children}
        </PersonContext.Provider>
    );
};

export default FamilyNameProvider;
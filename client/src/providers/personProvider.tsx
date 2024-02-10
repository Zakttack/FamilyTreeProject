import React, {useState} from "react";
import PersonContext, { PersonElement } from "../models/personContext";
import { ProviderProps } from "../models/providerProps";

const FamilyNameProvider: React.FC<ProviderProps> = ({children}) => {
    const [person, setPerson] = useState<PersonElement>({name: '', birthDate: '', deceasedDate: ''});
    return (
        <PersonContext.Provider value={{person: person, setPerson: setPerson}}>
            {children}
        </PersonContext.Provider>
    );
};

export default FamilyNameProvider;
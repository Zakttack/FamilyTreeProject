import React from "react";

export interface PersonElement {
    name: string;
    birthDate: string;
    deceasedDate: string;
}

export const PersonDefault: PersonElement = {name: '', birthDate: '', deceasedDate: ''};

interface PersonContextType {
    person: PersonElement;
    setPerson: (person: PersonElement) => void;
}

const PersonContext = React.createContext<PersonContextType>({person: PersonDefault, setPerson: () => {}});

export default PersonContext;
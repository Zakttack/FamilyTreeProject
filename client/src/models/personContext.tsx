import React from "react";

export interface PersonElement {
    name: string | null;
    birthDate: string | null;
    deceasedDate: string | null;
}

export const PersonDefault: PersonElement = {name: null, birthDate: null, deceasedDate: null};

interface PersonContextType {
    person: PersonElement;
    setPerson: (person: PersonElement) => void;
}

const PersonContext = React.createContext<PersonContextType>({person: PersonDefault, setPerson: () => {}});

export default PersonContext;
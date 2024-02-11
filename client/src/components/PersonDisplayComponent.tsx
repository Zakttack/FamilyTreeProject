import React from "react";
import _ from "lodash";
import { PersonElement } from "../models/personContext";

const PersonDisplayComponent: React.FC<PersonElement> = (person) => {
    const personHasName: boolean = !_.isNull(person.name);
    const personHasBirthDate: boolean = !_.isNull(person.birthDate);
    const personHasDeceasedDate: boolean = !_.isNull(person.deceasedDate);

    if (personHasName && personHasBirthDate && personHasDeceasedDate) {
        return (
            <p>{person.name}&nbsp;&#40;{person.birthDate} - {person.deceasedDate}&#41;</p>
        );
    }
    else if (personHasName && personHasBirthDate) {
        return (
            <p>{person.name}&nbsp;&#40;{person.birthDate} - Present&#41;</p>
        );
    }
    else if (personHasName) {
        return (
            <p>{person.name}</p>
        );
    }
    return (
        <p>Person Unknown</p>
    );
};

export default PersonDisplayComponent;
import React from "react";
import _ from "lodash";
import { FamilyDefault, FamilyElement } from "../models/familyContext";
import PersonDisplayComponent from "./PersonDisplayComponent";

const FamilyDisplayComponent: React.FC<FamilyElement> = (family) => {
    const familyHasMember: boolean = !_.isEqual(family.member, FamilyDefault.member);
    const familyHasInLaw: boolean = !_.isNull(family.inLaw);
    const familyHasMarriageDate: boolean = !_.isNull(family.marriageDate);

    if (familyHasMember && familyHasInLaw && familyHasMarriageDate) {
        return (
            <p>&#91;<PersonDisplayComponent name={family.member.name} birthDate={family.member.birthDate} deceasedDate={family.member.deceasedDate}/>&#93;&nbsp;-&nbsp;&#91;<PersonDisplayComponent name={_.isNull(family.inLaw) ? null : family.inLaw.name} birthDate={_.isNull(family.inLaw) ? null : family.inLaw.birthDate} deceasedDate={_.isNull(family.inLaw) ? null : family.inLaw.deceasedDate}/>&#93;:&nbsp;{family.marriageDate}</p>
        );
    }
    else if (familyHasMember && familyHasInLaw) {
        return (
            <p>&#91;<PersonDisplayComponent name={family.member.name} birthDate={family.member.birthDate} deceasedDate={family.member.deceasedDate}/>&#93;&nbsp;-&nbsp;&#91;<PersonDisplayComponent name={_.isNull(family.inLaw) ? null : family.inLaw.name} birthDate={_.isNull(family.inLaw) ? null : family.inLaw.birthDate} deceasedDate={_.isNull(family.inLaw) ? null : family.inLaw.deceasedDate}/>&#93;</p>
        );
    }
    else if (familyHasMember) {
        return (
            <PersonDisplayComponent name={family.member.name} birthDate={family.member.birthDate} deceasedDate={family.member.deceasedDate}></PersonDisplayComponent>
        );
    }
    return (
        <p>Unknown Family</p>
    );
};

export default FamilyDisplayComponent;
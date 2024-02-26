import React, { useContext } from "react";
import _ from "lodash";
import { FamilyElementContext } from "../models/FamilyElement";
import PersonInfoElement from "./PersonInfoElementComponent";
import { PersonType } from "../models/personInfoInput";

const FamilyInfoElement: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    return (
        <div>
            <h2>Info:</h2>
            <PersonInfoElement type={PersonType.Member} element={selectedElement.member}/>
            <PersonInfoElement type={PersonType.InLaw} element={selectedElement.inLaw}/>
            <h3>Marriage Date: {_.isNull(selectedElement.marriageDate) ? 'unknown' : selectedElement.marriageDate}</h3>
        </div>
    );
};

export default FamilyInfoElement;

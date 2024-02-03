import React, {useContext} from "react";
import FamilyNameContext from "../models/familyNameContext";

const FamilyNameInputComponent: React.FC = () => {
    const {familyName, setFamilyName} = useContext(FamilyNameContext);
    return (
        <input id="familyNameInputBox" type="text" value={familyName} onChange={(e) => setFamilyName(e.target.value)}/>
    );
};

export default FamilyNameInputComponent;
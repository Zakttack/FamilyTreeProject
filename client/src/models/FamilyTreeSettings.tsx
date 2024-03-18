import React from "react";
import OutputResponse from "./outputResponse";
import FamilyElement from "./FamilyElement";

export enum OrderTypeOptions {
    Empty = 'Order Family By:',
    ParentFirstThenChildren = 'parent first then children',
    AscendingByName = 'ascending by name'
};

interface FamilyTreeContextType {
    familyTreeResponse: OutputResponse<FamilyElement[]>;
    setFamilyTreeResponse: (familyTreeResponse: OutputResponse<FamilyElement[]>) => void;
}

const FamilyTreeSettingsContext = React.createContext<FamilyTreeContextType>({familyTreeResponse: {}, setFamilyTreeResponse: () => {}});

export default FamilyTreeSettingsContext;
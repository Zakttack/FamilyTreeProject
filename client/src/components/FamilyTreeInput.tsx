import React, {FormEvent, useContext, useState} from "react";
import FamilyTreeSettingsContext, {OrderTypeOptions} from "../models/FamilyTreeSettings";
import OutputResponse from "../models/outputResponse";
import FamilyElement from "../models/FamilyElement";
import { getFamilies } from "../Utils";

const FamilyTreeInput: React.FC = () => {
    const [orderOption, changeOrderOption] = useState<OrderTypeOptions>(OrderTypeOptions.Empty);
    const [memberName, setMemberName] = useState<string>('');
    const {setFamilyTreeResponse} = useContext(FamilyTreeSettingsContext);

    const getFamilyTree = async(e: FormEvent<HTMLFormElement>) => {
        const response: OutputResponse<FamilyElement[]> = await getFamilies(orderOption, memberName);
        setFamilyTreeResponse(response);
    }
    return (
        <form onSubmit={getFamilyTree}>
            <label>Choose Order Option:&nbsp;<select id="orderTypeSelector" value={orderOption} onChange={e => changeOrderOption(OrderTypeOptions[e.target.value as keyof typeof OrderTypeOptions])}>
                <option key={OrderTypeOptions.Empty} value={OrderTypeOptions.Empty}>{OrderTypeOptions.Empty}</option>
                <option key={OrderTypeOptions.ParentFirstThenChildren} value={OrderTypeOptions.ParentFirstThenChildren}>{OrderTypeOptions.ParentFirstThenChildren}</option>
                <option key={OrderTypeOptions.AscendingByName} value={OrderTypeOptions.AscendingByName}>{OrderTypeOptions.AscendingByName}</option>
            </select></label>
            <label>Filter By Member Name:&nbsp;<input type="text" value={memberName} onChange={e => setMemberName(e.target.value)}/></label>
            <button type="submit">Get Family Tree</button>
        </form>
    );
};

export default FamilyTreeInput;
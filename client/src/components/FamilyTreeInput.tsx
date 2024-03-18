import React, {FormEvent, useContext, useState} from "react";
import FamilyTreeSettingsContext from "../models/FamilyTreeSettings";
import OutputResponse from "../models/outputResponse";
import FamilyElement from "../models/FamilyElement";
import { getFamilies } from "../Utils";

const FamilyTreeInput: React.FC = () => {
    const orderOptions: string[] = ['unknown', 'parent first then children', 'ascending by name'];
    const [orderOption, changeOrderOption] = useState<string>(orderOptions[0]);
    const [memberName, setMemberName] = useState<string>('');
    const {setFamilyTreeResponse} = useContext(FamilyTreeSettingsContext);

    const getFamilyTree = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const response: OutputResponse<FamilyElement[]> = await getFamilies(orderOption, memberName);
        setFamilyTreeResponse(response);
    }
    return (
        <form onSubmit={getFamilyTree}>
            <label>Choose Order Option:&nbsp;<select id="orderTypeSelector" value={orderOption} onChange={e => changeOrderOption(e.target.value)}>
                {orderOptions.map(option => (
                    <option key={option} value={option}>{option}</option>
                ))}
            </select></label>
            <label>Filter By Member Name:&nbsp;<input type="text" value={memberName} onChange={e => setMemberName(e.target.value)}/></label>
            <button type="submit">Get Family Tree</button>
        </form>
    );
};

export default FamilyTreeInput;
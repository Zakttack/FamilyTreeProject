import React, {FormEvent, useContext, useState} from "react";
import OutputResponse from "../models/OutputResponse";
import FamilyElement from "../models/FamilyElement";
import { getClientSelectedFamily, getFamilies, setClientFamilyTree, viewSubtree } from "../Utils";
import FamilyTreeContext from "../context/FamilyTreeContext";

const FamilyTreeInput: React.FC<{includesEntireTree: boolean}> = ({includesEntireTree}) => {
    const orderOptions: string[] = ['unknown', 'parent first then children', 'ascending by name'];
    const {familyTreeResponse, setFamilyTreeResponse} = useContext(FamilyTreeContext);
    const [orderOption, changeOrderOption] = useState<string>(orderOptions[0]);
    const [memberName, setMemberName] = useState<string>('');

    const getFamilyTree = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (includesEntireTree) {
            const response: OutputResponse<FamilyElement[]> = await getFamilies(orderOption, memberName);
            setFamilyTreeResponse(response);
            await setClientFamilyTree(familyTreeResponse);
        }
        else {
            const response: OutputResponse<FamilyElement[]> = await viewSubtree(orderOption, memberName, await getClientSelectedFamily());
            setFamilyTreeResponse(response);
            await setClientFamilyTree(familyTreeResponse);
        }
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
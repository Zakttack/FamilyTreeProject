import React, {FormEvent, useState} from "react";
import OutputResponse from "../models/OutputResponse";
import FamilyElement from "../models/FamilyElement";
import { getClientSelectedFamily, getFamilies, setClientFamilyTree, viewSubtree } from "../Utils";

const FamilyTreeInput: React.FC<{includesEntireTree: boolean}> = ({includesEntireTree}) => {
    const orderOptions: string[] = ['unknown', 'parent first then children', 'ascending by name'];
    const [orderOption, changeOrderOption] = useState<string>(orderOptions[0]);
    const [memberName, setMemberName] = useState<string>('');

    const getFamilyTree = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (includesEntireTree) {
            const response: OutputResponse<FamilyElement[]> = await getFamilies(orderOption, memberName);
            await setClientFamilyTree(response).then(() => {
                window.location.reload();
            });
        }
        else {
            const response: OutputResponse<FamilyElement[]> = await viewSubtree(orderOption, memberName, await getClientSelectedFamily());
            await setClientFamilyTree(response).then(() => {
                window.location.reload();
            });
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
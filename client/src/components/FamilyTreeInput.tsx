import React, {FormEvent, useContext, useState} from "react";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import LoadingComponent from "./LoadingComponent";
import FamilyElementContext from "../contexts/FamilyElementContext";
import FamilyTreeContext from "../contexts/FamilyTreeContext";
import useLoadingContext from "../hooks/useLoadingContext";
import FamilyElement from "../models/FamilyElement";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { getFamilies, viewSubtree } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";

const FamilyTreeInput: React.FC<{includesEntireTree: boolean}> = (params) => {
    const orderOptions: string[] = ['unknown', 'parent first then children', 'ascending by name'];
    const [familyTreeResponse, setFamilyTreeResponse] = useState<FamilyTreeApiResponse>(EmptyResponse)
    const [orderOption, changeOrderOption] = useState<string>(orderOptions[0]);
    const [isClicked, setIsClicked] = useState<boolean>(false);
    const [memberName, setMemberName] = useState<string>('');
    const {addLoadingContext, removeLoadingContext, isLoading} = useLoadingContext();
    const {selectedElement} = useContext(FamilyElementContext);
    const {setFamilyTree} = useContext(FamilyTreeContext);

    const getFamilyTree = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            setIsClicked(true);
            setFamilyTreeResponse(EmptyResponse);
            e.preventDefault();
            if (params.includesEntireTree) {
                addLoadingContext(LoadingContext.RetrieveFamilyTree);
                setFamilyTreeResponse(await getFamilies(orderOption, memberName));
                if (!isProcessing(familyTreeResponse)) {
                    removeLoadingContext(LoadingContext.ViewSubtree);
                    setFamilyTree(isSuccess(familyTreeResponse) ? familyTreeResponse.result as FamilyElement[] : []);
                    setIsClicked(false);
                }
            }
            else {
                addLoadingContext(LoadingContext.ViewSubtree);
                setFamilyTreeResponse(await viewSubtree(orderOption, memberName, selectedElement));
                if (!isProcessing(familyTreeResponse)) {
                    removeLoadingContext(LoadingContext.ViewSubtree);
                    setFamilyTree(isSuccess(familyTreeResponse) ? familyTreeResponse.result as FamilyElement[] : []);
                    setIsClicked(false);
                }
            }
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
            {isClicked && <LoadingComponent context={params.includesEntireTree ? LoadingContext.RetrieveFamilyTree : LoadingContext.ViewSubtree} response={familyTreeResponse} />}
            <ErrorDisplayComponent response={familyTreeResponse} />
        </form>
    );
};

export default FamilyTreeInput;
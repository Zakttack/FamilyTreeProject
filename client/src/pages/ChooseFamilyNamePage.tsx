import React, { FormEvent, useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import ErrorDisplayComponent from "../components/ErrorDisplayComponent";
import LoadingComponent from "../components/LoadingComponent";
import FamilyNameContext from "../contexts/FamilyNameContext";
import TitleContext from "../contexts/TitleContext";
import useLoadingContext from "../hooks/useLoadingContext";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { initializeService } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import {FamilyTreeApiResponseStatus, LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";


const ChooseFamilyNamePage: React.FC = () => {
    const {familyName, setFamilyName} = useContext(FamilyNameContext);
    const {setTitle} = useContext(TitleContext);
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    let navigate = useNavigate();
    const [clicked, isClicked] = useState<boolean>(false);
    const [familyNameResponse, setFamilyNameResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const handleSubmitFamilyName = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            e.preventDefault();
            isClicked(true);
            if (!_.isEqual(familyName, '')) {
                addLoadingContext(LoadingContext.Default);
                setFamilyNameResponse (await initializeService(familyName));
                if (!isProcessing(familyNameResponse)) {
                    removeLoadingContext(LoadingContext.Default);
                    if (isSuccess(familyNameResponse)) {
                        setTitle(familyNameResponse.message);
                        navigate('/family-tree');
                    }
                }
            }
            else {
                setFamilyNameResponse({message: 'No name was provided.', status: FamilyTreeApiResponseStatus.Failure});
            }
            isClicked(false);
        }
    };
    return (
        <form onSubmit={handleSubmitFamilyName}>
            <h1>Welcome to the Client-Side of my Family Tree Project</h1><br/>
            <label>Enter a Family Name:&nbsp;<input type="text" value={familyName} onChange={(e) => setFamilyName(e.target.value)}/></label><br/>
            <button type="submit">Go To Family Tree</button>
            {clicked && <LoadingComponent context={LoadingContext.Default} response={familyNameResponse} />}
            <ErrorDisplayComponent response={familyNameResponse} />
        </form>
    );
};
export default ChooseFamilyNamePage;
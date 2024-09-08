import React, { FormEvent, useContext, useState } from "react";
import _ from "lodash";
import ArrowComponent from "./ArrowComponent";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import FileUpload from "./FileUpload";
import LoadingComponent from "./LoadingComponent";
import SuccessDisplay from "./SuccessDisplayComponent";
import FamilyNameContext from "../contexts/FamilyNameContext";
import SelectedFileContext from "../contexts/SelectedFileContext";
import useLoadingContext from "../hooks/useLoadingContext";
import FamilyTreeApiResponse from "../models/FamilyTreeApiResponse";
import { revertTree } from "../ApiCalls";
import { EmptyResponse } from "../Constants";
import { FamilyTreeApiResponseStatus, LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";
import './RevertTreeSection.css';

const RevertTreeSection: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {selectedFile} = useContext(SelectedFileContext);
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [revertTreeResponse, setRevertTreeResponse] = useState<FamilyTreeApiResponse>(EmptyResponse);
    const [clicked, isClicked] = useState<boolean>(false);
    const [isVisible, setIsVisible] = useState<boolean>(false);

    const handleRevertTree = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            e.preventDefault();
            isClicked(true);
            if (selectedFile) {
                addLoadingContext(LoadingContext.RevertTree)
                setRevertTreeResponse(await revertTree(selectedFile));
                if (!isProcessing(revertTreeResponse)) {
                    removeLoadingContext(LoadingContext.RevertTree);
                }
            }
            else {
                setRevertTreeResponse({message: 'No file was selected.', status: FamilyTreeApiResponseStatus.Failure});
            }
            isClicked(false);
        }
    };

    const handleVisibility = () => {
        if (!isLoading()) {
            setIsVisible(!isVisible);
        }
    }

    return (
        <section id="revert-tree-section">
            <header id="revert-tree-header" onClick={handleVisibility}>
                <h2>Revert Tree&nbsp;&nbsp;<ArrowComponent isVisible={isVisible}/></h2>
            </header>
            {isVisible && (
                <form onSubmit={handleRevertTree}>
                    <FileUpload /><br/>
                    {!_.isUndefined(selectedFile) && <><p>{selectedFile.name}</p><br/></>}
                    <button type="submit">Revert {familyName} Tree</button>
                    {clicked && <LoadingComponent context={LoadingContext.RevertTree} response={revertTreeResponse}/>}
                    <ErrorDisplayComponent response={revertTreeResponse}/>
                    {isSuccess(revertTreeResponse) && <SuccessDisplay response={revertTreeResponse} />}
                </form>
            )}
        </section>
    )
};

export default RevertTreeSection;
import React, { FormEvent, useContext, useState } from "react";
import OutputResponse from "../models/OutputResponse";
import MessageResponse from "../models/MessageResponse";
import { revertTree } from "../Utils";
import ArrowComponent from "./ArrowComponent";
import FileUpload from "./FileUpload";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import './RevertTreeSection.css';
import SelectedFileContext from "../context/SelectedFileContext";
import _ from "lodash";
import SuccessDisplay from "./SuccessDisplayComponent";
import ClientFamilyNameElement from "../models/ClientFamilyNameElement";

const RevertTreeSection: React.FC<ClientFamilyNameElement> = (params) => {
    const {selectedFile} = useContext(SelectedFileContext);
    const [revertTreeResponse, setRevertTreeResponse] = useState<OutputResponse<MessageResponse>>({});
    const [isVisible, setIsVisible] = useState<boolean>(false);

    const handleRevertTree = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (selectedFile) {
            const response: OutputResponse<MessageResponse> = await revertTree(selectedFile);
            setRevertTreeResponse(response);
            if (response.output) {
                setIsVisible(!isVisible);
            }
        }
        else {
            setRevertTreeResponse({problem: {message: 'No file was selected.', isSuccess: false}});
        }
    };

    const handleVisability = () => {
        setIsVisible(!isVisible);
    }

    return (
        <section id="revert-tree-section">
            <header id="revert-tree-header" onClick={handleVisability}>
                <h2>Revert Tree&nbsp;&nbsp;<ArrowComponent isVisible={isVisible}/></h2>
            </header>
            {isVisible && (
                <form onSubmit={handleRevertTree}>
                    <FileUpload /><br/>
                    {!_.isUndefined(selectedFile) && <><p>{selectedFile.name}</p><br/></>}
                    <button type="submit">Revert {params.familyName} Tree</button>
                    {revertTreeResponse.problem && <ErrorDisplayComponent message={revertTreeResponse.problem.message}/>}
                </form>
            )}
            {revertTreeResponse.output && <SuccessDisplay message={revertTreeResponse.output.message}/>}
        </section>
    )
};

export default RevertTreeSection;
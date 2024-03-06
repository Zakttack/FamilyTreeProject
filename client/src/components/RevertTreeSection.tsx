import React, { FormEvent, useContext, useState } from "react";
import { FileElementContext } from "../models/FileElement";
import FamilyNameContext from "../models/familyNameContext";
import OutputResponse from "../models/outputResponse";
import MessageResponse from "../models/MessageResponse";
import { revertTree } from "../Utils";
import ArrowComponent from "./ArrowComponent";
import FileUpload from "./FileUploadComponent";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import SuccessDisplay from "./SuccessDisplayComponent";
import './RevertTreeSection.css';

const RevertTreeSection: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {selectedFile} = useContext(FileElementContext);
    const [revertTreeResponse, setRevertTreeResponse] = useState<OutputResponse<MessageResponse>>({});
    const [isVisible, setIsVisible] = useState<boolean>(false);

    const handleRevertTree = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const response: OutputResponse<MessageResponse> = await revertTree(selectedFile);
        setRevertTreeResponse(response);
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
                    <FileUpload />
                    <button type="submit">Revert {familyName} Tree</button>
                    {revertTreeResponse.problem && <ErrorDisplayComponent message={revertTreeResponse.problem.message}/>}
                    {revertTreeResponse.output && <SuccessDisplay message={revertTreeResponse.output.message}/>}
                </form>
            )}
        </section>
    )
};

export default RevertTreeSection;
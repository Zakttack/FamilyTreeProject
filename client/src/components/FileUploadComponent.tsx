import React, { useContext, useState } from "react";
import FileRequest from "../models/fileRequest";
import OutputResponse from "../models/outputResponse";
import SuccessResponse from "../models/successResponse";
import { revertTree } from "../Utils";
import FamilyNameContext from "../models/familyNameContext";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import SuccessDisplay from "./SuccessDisplayComponent";

interface FileUploadAction {
    action: string;
}

const FileUpload: React.FC<FileUploadAction> = (uploadAction) => {
    const {familyName} = useContext(FamilyNameContext);
    const [request, setRequest] = useState<FileRequest | null>(null);
    const [response, setResponse] = useState<OutputResponse<SuccessResponse>>({output: null, problem: null});

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const files = event.target.files;
        if (files && files[0]) {
            setRequest({templateFilePath: URL.createObjectURL(files[0])});
        }
    };

    const handleUploadFileAction = async () => {
        if (request) {
            if (uploadAction.action === 'Revert Tree') {
                const responseOut: OutputResponse<SuccessResponse> = await revertTree(familyName, request);
                setResponse(responseOut);
            }
        }
        else {
            console.log('No file path provided.');
        }
    };

    return (
        <div>
            <p>Choose a file:&nbsp;<input type="file" onChange={handleFileChange}/></p>
            {request && <p>Selected File Path:&nbsp;{request.templateFilePath}</p>}
            <button onClick={handleUploadFileAction}>{uploadAction.action}</button>
            {response.problem && <ErrorDisplayComponent name={response.problem.name} message={response.problem.message}/>}
            {response.output && <SuccessDisplay message={response.output.message}/>}
        </div>
    );
};

export default FileUpload;
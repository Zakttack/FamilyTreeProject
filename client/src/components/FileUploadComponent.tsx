import React, { useContext, useState } from "react"
import _ from "lodash";
import FileElement, { FileElementContext } from "../models/FileElement"
import OutputResponse from "../models/outputResponse";
import { getFilePaths } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
const FileUpload: React.FC = () => {
    const {selectedFile, changeSelectedFile} = useContext(FileElementContext);
    const [pathsResponse, changePathsResponse] = useState<OutputResponse<FileElement[]>>({});

    const handleFileNameChange = async(e: React.ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (files) {
            const fileName = files[0].name;
            const response: OutputResponse<FileElement[]> = await getFilePaths(fileName);
            changePathsResponse(response);
        }
        else {
            changePathsResponse({problem: {message: 'No file was selected.', isSuccess: false}});
        }
    };

    const handleSelectFile = (e: React.ChangeEvent<HTMLInputElement>) => {
        changeSelectedFile({filePath: e.target.value});
    };

    return (
        <>
            <label>Choose a Family Tree Template:&nbsp;<input type="file" onChange={handleFileNameChange}/></label>
            {!_.isUndefined(pathsResponse.problem) && <ErrorDisplayComponent message={pathsResponse.problem.message}/>}
            {!_.isUndefined(pathsResponse.output) && (
            <div>
                {pathsResponse.output.map((element: FileElement) => (
                    <label><input type="radio" value={element.filePath} checked={_.isEqual(selectedFile, element)} onChange={handleSelectFile}/>{element.filePath}</label>
                ))}
            </div>
            )}
        </>
    );
};

export default FileUpload;
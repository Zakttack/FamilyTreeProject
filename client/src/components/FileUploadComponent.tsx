import React, { useContext, useState } from "react"
import _ from "lodash";
import FileElement, { FileElementContext } from "../models/FileElement"
import OutputResponse from "../models/outputResponse";
import { getFilePaths } from "../Utils";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
const FileUpload: React.FC = () => {
    const {selectedFile, changeSelectedFile} = useContext(FileElementContext);
    const [fileName, setFileName] = useState<string>('');
    const [pathsResponse, changePathsResponse] = useState<OutputResponse<FileElement[]>>({output: null, problem: null});

    const handleFileNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFileName(e.target.value);
    };

    const handleSelectFile = (e: React.ChangeEvent<HTMLInputElement>) => {
        changeSelectedFile({filePath: e.target.value});
    }

    const handleFindPaths = async () => {
        const response: OutputResponse<FileElement[]> = await getFilePaths(fileName);
        changePathsResponse(response);
    };

    return (
        <>
            <label>Enter File Name Including Extension:&nbsp;<input type="text" value={fileName} onChange={handleFileNameChange}/>&nbsp;<button onClick={handleFindPaths}>Find Paths</button></label>
            {!_.isNull(pathsResponse.problem) && <ErrorDisplayComponent message={pathsResponse.problem.message}/>}
            {!_.isNull(pathsResponse.output) && (
            <div>
                {pathsResponse.output.map((element: FileElement) => (
                    <label><input type="radio" value={element.filePath} checked={!_.isNull(selectedFile) && selectedFile.filePath === element.filePath} onChange={handleSelectFile}/>{element.filePath}</label>
                ))}
            </div>
            )}
        </>
    );
};

export default FileUpload;
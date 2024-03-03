import React, { useState } from "react";
import FileElement, {FileElementContext} from "../models/FileElement";
import { ProviderProps } from "../models/providerProps";

const FileElementProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedFile, changeSelectedFile] = useState<FileElement>({filePath: ''});
    return (
        <FileElementContext.Provider value={{selectedFile: selectedFile, changeSelectedFile: changeSelectedFile}}>
            {children}
        </FileElementContext.Provider>
    );
};

export default FileElementProvider;
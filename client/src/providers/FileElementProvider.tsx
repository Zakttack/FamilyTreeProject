import React, { useState } from "react";
import FileElement, {FileDefault, FileElementContext} from "../models/FileElement";
import { ProviderProps } from "../models/providerProps";

const FileElementProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedFile, changeSelectedFile] = useState<FileElement>(FileDefault);
    return (
        <FileElementContext.Provider value={{selectedFile, changeSelectedFile}}>
            {children}
        </FileElementContext.Provider>
    );
};

export default FileElementProvider;
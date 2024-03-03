import React from "react";

export default interface FileElement {
    filePath: string;
}

interface FileElementContextType {
    selectedFile: FileElement;
    changeSelectedFile: (element: FileElement) => void;
}

export const FileElementContext = React.createContext<FileElementContextType>({selectedFile: {filePath: ''}, changeSelectedFile: () => {}});
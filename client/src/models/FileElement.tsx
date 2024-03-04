import React from "react";
import { StringDefault } from "../Utils";

export default interface FileElement {
    filePath: string;
}

export const FileDefault: FileElement = {filePath: StringDefault};

interface FileElementContextType {
    selectedFile: FileElement;
    changeSelectedFile: (element: FileElement) => void;
}

export const FileElementContext = React.createContext<FileElementContextType>({selectedFile: FileDefault, changeSelectedFile: () => {}});
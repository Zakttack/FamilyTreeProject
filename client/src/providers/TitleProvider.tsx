import React, {useState} from "react";
import { ProviderProps } from "../models/ProviderProps";
import { StringDefault } from "../Utils";
import TitleContext from "../context/TitleContext";

const TitleProvider: React.FC<ProviderProps> = ({children}) => {
    const [title, setTitle] = useState<string>(StringDefault);
    return (
        <TitleContext.Provider value={{title, setTitle}}>
            {children}
        </TitleContext.Provider>
    )
};
export default TitleProvider;
import React, { useState } from "react";
import TitleContext from "../contexts/TitleContext";
import ProviderProps from "../models/ProviderProps";

const TitleProvider: React.FC<ProviderProps> = ({children}) => {
    const [title, setTitle] = useState<string>('');

    return (
        <TitleContext.Provider value={{title, setTitle}}>
            {children}
        </TitleContext.Provider>
    );
};

export default TitleProvider;
import React from "react";
import TitleContext from "../context/TitleContext";
import { ProviderProps } from "../models/ProviderProps";
import { getClientPageTitle, setClientPageTitle } from "../Utils";

const TitleProvider: React.FC<ProviderProps> = ({children}) => {
    return (
        <TitleContext.Provider value={{title: getClientPageTitle(), setTitle: setClientPageTitle}}>
            {children}
        </TitleContext.Provider>
    );
};

export default TitleProvider;
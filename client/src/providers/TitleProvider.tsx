import React from "react";
import TitleContext from "../models/TitleContext";
import { ProviderProps } from "../models/providerProps";
import { getClientPageTitle, setClientPageTitle } from "../Utils";

const TitleProvider: React.FC<ProviderProps> = ({children}) => {
    return (
        <TitleContext.Provider value={{title: getClientPageTitle(), setTitle: setClientPageTitle}}>
            {children}
        </TitleContext.Provider>
    );
};

export default TitleProvider;
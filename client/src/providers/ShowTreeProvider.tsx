import React, { useState } from "react";
import ShowTreeContext from "../models/ShowTreeContext";
import { ProviderProps } from "../models/providerProps";

const ShowTreeProvider: React.FC<ProviderProps> = ({children}) => {
    const [treeShown, showTree] = useState<boolean>(true);
    return (
        <ShowTreeContext.Provider value={{treeShown: treeShown, showTree: showTree}}>
            {children}
        </ShowTreeContext.Provider>
    );
};

export default ShowTreeProvider;
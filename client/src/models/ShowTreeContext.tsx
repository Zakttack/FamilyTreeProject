import React from "react";
interface ShowTreeContextType {
    treeShown: boolean;
    showTree: (treeShown: boolean) => void;
}

const ShowTreeContext = React.createContext<ShowTreeContextType>({treeShown: true, showTree: () => {}});

export default ShowTreeContext;
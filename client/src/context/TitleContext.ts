import React from "react";
import { StringDefault } from "../Utils";

interface TitleContextType {
    title: string;
    setTitle: (title: string) => void;
}

const TitleContext = React.createContext<TitleContextType>({title: StringDefault, setTitle: () => {}});
export default TitleContext;
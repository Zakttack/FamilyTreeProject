import React, { useContext } from "react";
import TitleContext from "../context/TitleContext";

const Title: React.FC = () => {
    const {title} = useContext(TitleContext);
    return (
        <h1>{title}</h1>
    );
};

export default Title;
import React, {useContext, useEffect} from "react";
import { getClientPageTitle, StringDefault } from "../Utils";
import TitleContext from "../context/TitleContext";
import _ from "lodash"

const Title: React.FC = () => {
    const {title, setTitle} = useContext(TitleContext);
    useEffect(() => {
        const fetchTitle = async() => {
            if (_.isEqual(title, StringDefault)) {
                setTitle(await getClientPageTitle());
            }
        };
        fetchTitle();
    }, [title, setTitle])

    return (
        <h1>{title}</h1>
    )
};

export default Title;
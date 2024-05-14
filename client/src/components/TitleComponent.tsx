import React, {useEffect, useState} from "react";
import { getClientPageTitle } from "../Utils";

const Title: React.FC = () => {
    const [title, setTitle] = useState<string>('');
    useEffect(() => {
        const fetchTitle = async() => {
            setTitle(await getClientPageTitle());
        };
        fetchTitle();
    }, [title])

    return (
        <h1>{title}</h1>
    )
};

export default Title;
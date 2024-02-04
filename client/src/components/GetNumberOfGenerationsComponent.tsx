import React, {useContext, useEffect, useState} from "react";
import FamilyNameContext from "../models/familyNameContext";

const GetNumberOfGenerationsComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const [generationCount, setGenerationCount] = useState<number>(0);
    useEffect(() => {
        const fetchNumberOfGenerations = async () => {
            const url = `https://localhost:5201/api/familytree/${familyName}/numberofgenerations`;
            try {
                const response = await fetch(url);
                if (!response.ok)
                {
                    const errorOutput = await response.json();
                    throw new Error(`${errorOutput.Name}: ${errorOutput.Message}`);
                }
                const output: number = await response.json();
                setGenerationCount(output);
            } catch (error) {
                console.error("Failed to fetch number of generations:", error);
            }
        };
        fetchNumberOfGenerations();
    }, [familyName]);
    return (
        <p>There are {generationCount} generations in the {familyName} family tree.</p>
    )
}

export default GetNumberOfGenerationsComponent;
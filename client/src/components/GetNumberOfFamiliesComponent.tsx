import React, {useContext, useEffect, useState} from "react";
import FamilyNameContext from "../models/familyNameContext";

const GetNumberOfFamiliesComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const [familyCount, setFamilyCount] = useState<number>(0);
    useEffect(() => {
        const fetchNumberOfFamilies = async () => {
            const url = `https://localhost:5201/api/familytree/${familyName}/numberoffamilies`;
            try {
                const response = await fetch(url);
                if (!response.ok) {
                    const errorData = await response.json();
                    throw new Error(`${errorData.Name}: ${errorData.Message}`);
                }
                const data: number = await response.json();
                setFamilyCount(data);
            } catch (error) {
                console.error("Failed to fetch number of families:", error);
            }
        };
        fetchNumberOfFamilies();
    }, [familyName]);
    return (
        <p>There are {familyCount} families in the {familyName} family.</p>
    )
};

export default GetNumberOfFamiliesComponent;
import React, { useContext, useEffect, useState} from "react";
import { FamilyElement } from "../models/familyContext";
import FamilyNameContext from "../models/familyNameContext";
import OrderTypeContext from "../models/orderTypeContext";
import { ErrorResponse } from "../models/errorResponse";
import FamilyDisplayComponent from "./FamilyDisplayComponent";

interface FamilyTreeDisplayResponse {
    familyElements?: FamilyElement[];
    errorMessage?: string;
}

const FamilyTreeDisplayComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {selectedOrderType} = useContext(OrderTypeContext);
    const [handlerResponse, setHandlerResponse] = useState<FamilyTreeDisplayResponse>({});
    useEffect(() => {
        const handleRender = async () => {
            const url = `http://localhost:5201/api/familytree/${familyName}/getfamilies/${selectedOrderType}`;
            const response = await fetch(url);
            if (response.ok) {
                let familyElements: FamilyElement[] = await response.json();
                setHandlerResponse({familyElements: familyElements});
            }
            else if (response.status === 400) {
                let errorMessage: string = await response.json();
                setHandlerResponse({errorMessage: errorMessage});
            }
            else if (response.status === 500) {
                let errorOutput: ErrorResponse = await response.json();
                setHandlerResponse({errorMessage: `${errorOutput.name}: ${errorOutput.message}`});
            }
        };
        handleRender();
    }, [familyName, selectedOrderType]);

    return (
        <div>
            {handlerResponse.familyElements && (
                <div>
                    {handlerResponse.familyElements.map(element => (
                        <FamilyDisplayComponent member={element.member} inLaw={element.inLaw} marriageDate={element.marriageDate}/>
                    ))}
                </div>
            )}
            {handlerResponse.errorMessage && (
                <p>{handlerResponse.errorMessage}</p>
            )}
        </div>
    );
};

export default FamilyTreeDisplayComponent;
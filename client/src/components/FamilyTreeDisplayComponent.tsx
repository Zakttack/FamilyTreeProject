import React, { useContext, useEffect, useState} from "react";
import _ from "lodash";
import FamilyNameContext from "../models/familyNameContext";
import OrderTypeContext from "../models/orderTypeContext";
import { ErrorResponse } from "../models/errorResponse";

interface FamilyTreeDisplayResponse {
    familyElements: string[] | null;
    errorMessage: string | null;
}

const FamilyTreeDisplayComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {selectedOrderType} = useContext(OrderTypeContext);
    const [handlerResponse, setHandlerResponse] = useState<FamilyTreeDisplayResponse>({familyElements: null, errorMessage: null});
    useEffect(() => {
        const handleRender = async () => {
            const url = `http://localhost:5201/api/familytree/${familyName}/getfamilies/${selectedOrderType}`;
            const response = await fetch(url);
            if (response.ok) {
                let familyElements: string[] = await response.json();
                setHandlerResponse({familyElements: familyElements, errorMessage: null});
            }
            else if (response.status === 400) {
                let errorMessage: string = await response.json();
                setHandlerResponse({familyElements: null, errorMessage: errorMessage});
            }
            else if (response.status === 500) {
                let errorOutput: ErrorResponse = await response.json();
                setHandlerResponse({familyElements: null, errorMessage: `${errorOutput.name}: ${errorOutput.message}`});
            }
        };
        handleRender();
    }, [familyName, selectedOrderType]);

    return (
        <div>
            {!_.isNull(handlerResponse.familyElements) && (
                <div>
                    {handlerResponse.familyElements.map(element => (
                        <p>{element}</p>
                    ))}
                </div>
            )}
            {!_.isNull(handlerResponse.errorMessage) && (
                <p>{handlerResponse.errorMessage}</p>
            )}
        </div>
    );
};

export default FamilyTreeDisplayComponent;
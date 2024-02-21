import React, { useContext, useEffect, useState} from "react";
import { useNavigate } from "react-router-dom";
import _ from "lodash";
import FamilyNameContext from "../models/familyNameContext";
import OrderTypeContext from "../models/orderTypeContext";
import ExceptionResponse  from "../models/exceptionResponse";
import "./FamilyTreeDisplayComponent.css";
import FamilyRepresenatationElementContext from "../models/familyRepresentationElementContext";
import FamilyRepresentationElement from "../models/familyRepresentationElement";

interface FamilyTreeDisplayResponse {
    familyElements: string[] | null;
    errorOutput: ExceptionResponse | null;
}

const FamilyTreeDisplayComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {selectedOrderType} = useContext(OrderTypeContext);
    const [handlerResponse, setHandlerResponse] = useState<FamilyTreeDisplayResponse>({familyElements: null, errorOutput: null});
    const {setRepresentationElement} = useContext(FamilyRepresenatationElementContext);
    let navigate = useNavigate();
    const handleClick = (e: React.MouseEvent<HTMLParagraphElement>) => {
        const text = e.currentTarget.textContent;
        const represenation: FamilyRepresentationElement = {representation: text};
        setRepresentationElement(represenation);
        navigate('/subtree-dashboard');
    }
    useEffect(() => {
        const handleRender = async () => {
            const url = `http://localhost:5201/api/familytree/${familyName}/getfamilies/${selectedOrderType}`;
            const response = await fetch(url);
            if (!response.ok) {
                let errorOutput: ExceptionResponse = await response.json();
                setHandlerResponse({familyElements: null, errorOutput: errorOutput});
            }
            else {
                let familyElements: string[] = await response.json();
                setHandlerResponse({familyElements: familyElements, errorOutput: null});
            }
        };
        handleRender();
    }, [familyName, selectedOrderType]);

    return (
        <div>
            <p>Selected Order: {selectedOrderType}</p>
            {!_.isNull(handlerResponse.errorOutput) && (
                <p className="error">{handlerResponse.errorOutput.name}: {handlerResponse.errorOutput.message}</p>
            )}
            {!_.isNull(handlerResponse.familyElements) && (
                <div>
                    {handlerResponse.familyElements.map(element => (
                        <p className="familyElement" onClick={handleClick}>{element}</p>
                    ))}
                </div>
            )}
        </div>
    );
};

export default FamilyTreeDisplayComponent;
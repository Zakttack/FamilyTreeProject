import React, { useContext, useEffect, useState} from "react";
import _ from "lodash";
import FamilyNameContext from "../models/familyNameContext";
import OrderTypeContext from "../models/orderTypeContext";
import MessageResponse from "../models/MessageResponse";
import FamilyElement from "../models/FamilyElement";
import FamilyElementDisplay from "./FamilyElementDisplay";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import OutputResponse from "../models/outputResponse";

const FamilyTreeDisplayComponent: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    const {selectedOrderType} = useContext(OrderTypeContext);
    const [handlerResponse, setHandlerResponse] = useState<OutputResponse<FamilyElement[]>>({problem: null, output: null});
    useEffect(() => {
        const handleRender = async () => {
            const url = `http://localhost:5201/api/familytree/${familyName}/getfamilies/${selectedOrderType}`;
            const response = await fetch(url);
            if (!response.ok) {
                const errorOutput: MessageResponse = await response.json();
                setHandlerResponse({output: null, problem: errorOutput});
            }
            else {
                const familyElements: FamilyElement[] = await response.json();
                setHandlerResponse({output: familyElements, problem: null});
            }
        };
        handleRender();
    }, [familyName, selectedOrderType]);

    return (
        <div>
            <p>Selected Order: {selectedOrderType}</p>
            {!_.isNull(handlerResponse.problem) && (
                <ErrorDisplayComponent message={handlerResponse.problem.message}/>
            )}
            {!_.isNull(handlerResponse.output) && (
                <div>
                    {handlerResponse.output.map((element: FamilyElement) => (
                        <FamilyElementDisplay member={element.member} inLaw={element.inLaw} marriageDate={element.marriageDate}/>
                    ))}
                </div>
            )}
        </div>
    );
};

export default FamilyTreeDisplayComponent;
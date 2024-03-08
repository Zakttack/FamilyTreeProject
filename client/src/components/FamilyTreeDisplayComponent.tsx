import React, { useContext, useEffect, useState} from "react";
import _ from "lodash";
import OrderTypeContext from "../models/orderTypeContext";
import FamilyElement from "../models/FamilyElement";
import FamilyElementDisplay from "./FamilyElementDisplay";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import OutputResponse from "../models/outputResponse";
import { getFamilies } from "../Utils";

const FamilyTreeDisplayComponent: React.FC = () => {
    const {selectedOrderType} = useContext(OrderTypeContext);
    const [handlerResponse, setHandlerResponse] = useState<OutputResponse<FamilyElement[]>>({});
    useEffect(() => {
        const handleRender = async () => {
            const response: OutputResponse<FamilyElement[]> = await getFamilies(selectedOrderType);
            setHandlerResponse(response);
        };
        handleRender();
    }, [selectedOrderType]);

    return (
        <div>
            <p>Selected Order: {selectedOrderType}</p>
            {!_.isUndefined(handlerResponse.problem) && (
                <ErrorDisplayComponent message={handlerResponse.problem.message}/>
            )}
            {!_.isUndefined(handlerResponse.output) && (
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
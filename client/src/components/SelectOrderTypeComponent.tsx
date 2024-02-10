import React, {useContext} from "react";
import OrderTypeContext, { OrderTypeOptions } from "../models/orderTypeContext";

const SelectOrderTypeComponent: React.FC = () => {
    const {selectedOrderType, changeOrderType} = useContext(OrderTypeContext);

    return (
        <p>Order Family By:&nbsp;
            <select id="orderTypeSelector" value={selectedOrderType} onChange={e => changeOrderType(e.target.value)}>
                {OrderTypeOptions.map(option => (
                    <option key={option} value={option}>{option}</option>
                ))}
            </select>
        </p>
    );
};

export default SelectOrderTypeComponent;
import React, {useState} from "react";
import { ProviderProps } from "../models/providerProps";
import OrderTypeContext, { OrderTypeOptions } from "../models/orderTypeContext";

const OrderTypeProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedOrderType, changeOrderType] = useState<string>(OrderTypeOptions[0]);

    return (
        <OrderTypeContext.Provider value={{selectedOrderType, changeOrderType}}>
            {children}
        </OrderTypeContext.Provider>
    );
};

export default OrderTypeProvider;
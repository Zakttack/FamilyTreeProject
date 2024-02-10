import React, {useState} from "react";
import { ProviderProps } from "../models/providerProps";
import OrderTypeContext from "../models/orderTypeContext";

const OrderTypeProvider: React.FC<ProviderProps> = ({children}) => {
    const [selectedOrderType, changeOrderType] = useState<string>('');

    return (
        <OrderTypeContext.Provider value={{selectedOrderType, changeOrderType}}>
            {children}
        </OrderTypeContext.Provider>
    );
};

export default OrderTypeProvider;
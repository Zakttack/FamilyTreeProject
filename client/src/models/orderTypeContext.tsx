import React from "react";

export const OrderTypeOptions: string[] = ['Order Family By:','ascending by name', 'parent first then children'];

interface OrderTypeContextType {
    selectedOrderType: string;
    changeOrderType: (orderType: string) => void;
}

const OrderTypeContext = React.createContext<OrderTypeContextType>({selectedOrderType: OrderTypeOptions[0], changeOrderType: () => {}});

export default OrderTypeContext;
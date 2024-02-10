import React from "react";

export const OrderTypeOptions: string[] = ['','ascending by name', 'parent first then children'];

interface OrderTypeContextType {
    selectedOrderType: string;
    changeOrderType: (orderType: string) => void;
}

const OrderTypeContext = React.createContext<OrderTypeContextType>({selectedOrderType: '', changeOrderType: () => {}});

export default OrderTypeContext;
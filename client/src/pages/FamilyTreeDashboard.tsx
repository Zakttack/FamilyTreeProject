import React, { useContext } from "react";
import FamilyNameContext from "../models/familyNameContext";
import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";
import OrderTypeProvider from "../providers/orderTypeProvider";
import SelectOrderTypeComponent from "../components/SelectOrderTypeComponent";
import FamilyTreeDisplayComponent from "../components/FamilyTreeDisplayComponent";

const FamilyTreeDashboard: React.FC = () => {
    const {familyName} = useContext(FamilyNameContext);
    return (
        <div>
            <h1>This is the {familyName} family</h1>
            <GetNumberOfGenerationsComponent />
            <GetNumberOfFamiliesComponent />
            <OrderTypeProvider>
                <SelectOrderTypeComponent />
                <FamilyTreeDisplayComponent />
            </OrderTypeProvider>
        </div>
    );
}

export default FamilyTreeDashboard;
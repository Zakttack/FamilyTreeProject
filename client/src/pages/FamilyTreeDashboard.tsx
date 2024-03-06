import React from "react";
//import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
//import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";
import OrderTypeProvider from "../providers/orderTypeProvider";
import SelectOrderTypeComponent from "../components/SelectOrderTypeComponent";
import FamilyTreeDisplayComponent from "../components/FamilyTreeDisplayComponent";
import FileElementProvider from "../providers/FileElementProvider";
import RevertTreeSection from "../components/RevertTreeSection";
import Title from "../components/TitleComponent";

const FamilyTreeDashboard: React.FC = () => {
    return (
        <div>
            <Title />
            <FileElementProvider>
                <RevertTreeSection />
            </FileElementProvider>
            <OrderTypeProvider>
                <SelectOrderTypeComponent />
                <FamilyTreeDisplayComponent />
            </OrderTypeProvider>
        </div>
    );
}

export default FamilyTreeDashboard;
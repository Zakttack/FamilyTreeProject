import React, { useContext } from "react";
//import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
//import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";
import OrderTypeProvider from "../providers/orderTypeProvider";
import SelectOrderTypeComponent from "../components/SelectOrderTypeComponent";
import FamilyTreeDisplayComponent from "../components/FamilyTreeDisplayComponent";
import FileElementProvider from "../providers/FileElementProvider";
import RevertTreeSection from "../components/RevertTreeSection";
import Title from "../components/TitleComponent";
import ShowTreeProvider from "../providers/ShowTreeProvider";
import ShowTreeContext from "../models/ShowTreeContext";
const FamilyTreePage: React.FC = () => {
    const {treeShown} = useContext(ShowTreeContext);
    return (
        <ShowTreeProvider>
            <Title />
            <FileElementProvider>
                <RevertTreeSection />
            </FileElementProvider>
            {treeShown && (
                <OrderTypeProvider>
                    <SelectOrderTypeComponent />
                    <FamilyTreeDisplayComponent />
                </OrderTypeProvider>
            )}
        </ShowTreeProvider>
    );
}

export default FamilyTreePage;
import React, { useContext } from "react";
// import FamilyNameContext from "../models/familyNameContext";
// import GetNumberOfFamiliesComponent from "../components/GetNumberOfFamiliesComponent";
// import GetNumberOfGenerationsComponent from "../components/GetNumberOfGenerationsComponent";
// import OrderTypeProvider from "../providers/orderTypeProvider";
// import SelectOrderTypeComponent from "../components/SelectOrderTypeComponent";
// import FamilyTreeDisplayComponent from "../components/FamilyTreeDisplayComponent";
import FamilyRepresenatationElementContext from "../models/familyRepresentationElementContext";

const SubTreeDashboard: React.FC = () => {
    //const {familyName} = useContext(FamilyNameContext);
    const {representationElement} = useContext(FamilyRepresenatationElementContext);

    return (
        <div>
            <h1>{representationElement.familyRepresenatation}</h1>
        </div>
    );
};

export default SubTreeDashboard;
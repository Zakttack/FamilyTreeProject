import React, {useContext} from "react";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import LoadingComponent from "./LoadingComponent";
import SuccessDisplay from "./SuccessDisplayComponent";
import ReportActionsContext from "../contexts/ReportActionsContext";
import { LoadingContext, ReportSections } from "../Enums";
import { isSuccess } from "../Utils";

const ReportActionsOutput: React.FC<{section: ReportSections}> = (params) => {
    const {reportMade, response} = useContext(ReportActionsContext);

    const getLoadingContext = () => {
        switch (params.section) {
            case ReportSections.ReportChildren: return LoadingContext.ReportChildren;
            case ReportSections.ReportDeceased: return LoadingContext.ReportDeceased;
            case ReportSections.ReportMarriage: return LoadingContext.ReportMarriage;
        }
        return LoadingContext.Default;
    };

    return (
        <>
            {reportMade && <LoadingComponent context={getLoadingContext()} response={response} />}
            <ErrorDisplayComponent response={response} />
            {isSuccess(response) && <SuccessDisplay response={response} />}
        </>
    );
};

export default ReportActionsOutput;
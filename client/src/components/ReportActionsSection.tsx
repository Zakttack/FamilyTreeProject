import React, { useContext, useState } from "react";
import ReportActionsContext from "../context/ReportActionsContext";
import ReportActionsProvider from "../providers/ReportActionsProvider";
import './ReportActionsSection.css';
import ReportMarriageForm from "./ReportMarriageForm";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import ReportDeceasedForm from "./ReportDeceasedForm";
import ReportChildrenForm from "./ReportChildrenForm";
import SuccessDisplay from "./SuccessDisplayComponent";
import ClientSelectedFamilyElement from "../models/ClientSelectedFamilyElement";

enum Sections {
    Default,
    ReportMarriage,
    ReportDeceased,
    ReportChildren
};

const ReportActionsSection: React.FC<ClientSelectedFamilyElement> = (params) => {
    const [selectedSection, changeSelectedSection] = useState<Sections>(Sections.Default);
    const {response} = useContext(ReportActionsContext);

    return (
        <ReportActionsProvider>
            <h2>Report Actions:</h2>
            <section id="report-actions-section">
                <div id="report-action-headers">
                    <header className="report-action-header" onClick={() => changeSelectedSection(Sections.ReportMarriage)} tabIndex={0} role="button">
                        Report Marriage
                    </header>
                    <header className="report-action-header" onClick={() => changeSelectedSection(Sections.ReportDeceased)} tabIndex={0} role="button">
                        Report Deceased
                    </header>
                    <header className="report-action-header" onClick={() => changeSelectedSection(Sections.ReportChildren)} tabIndex={0} role="button">
                        Report Children
                    </header>
                </div>
                <div id="report-action-form">
                    {selectedSection === Sections.ReportMarriage && <ReportMarriageForm selectedFamily={params.selectedFamily}/>}
                    {selectedSection === Sections.ReportDeceased && <ReportDeceasedForm selectedFamily={params.selectedFamily}/>}
                    {selectedSection === Sections.ReportChildren && <ReportChildrenForm selectedFamily={params.selectedFamily}/>}
                </div>
                {response.problem && <ErrorDisplayComponent message={response.problem.message}/>}
                {response.output && <SuccessDisplay message={response.output.message}/>}
            </section>
        </ReportActionsProvider>
    );
};

export default ReportActionsSection;
import React, { useState } from "react";
import ReportActionsOutput from "./ReportActionsOutput";
import ReportChildrenForm from "./ReportChildrenForm";
import ReportDeceasedForm from "./ReportDeceasedForm";
import ReportMarriageForm from "./ReportMarriageForm";
import ReportActionsProvider from "../providers/ReportActionsProvider";
import { ReportSections } from "../Enums";
import './ReportActionsSection.css';



const ReportActionsSection: React.FC = () => {
    const [selectedSection, changeSelectedSection] = useState<ReportSections>(ReportSections.Default);

    return (
        <ReportActionsProvider>
            <h2>Report Actions:</h2>
            <section id="report-actions-section">
                <div id="report-action-headers">
                    <header className="report-action-header" onClick={() => changeSelectedSection(ReportSections.ReportMarriage)} tabIndex={0} role="button">
                        Report Marriage
                    </header>
                    <header className="report-action-header" onClick={() => changeSelectedSection(ReportSections.ReportDeceased)} tabIndex={0} role="button">
                        Report Deceased
                    </header>
                    <header className="report-action-header" onClick={() => changeSelectedSection(ReportSections.ReportChildren)} tabIndex={0} role="button">
                        Report Children
                    </header>
                </div>
                <div id="report-action-form">
                    {selectedSection === ReportSections.ReportMarriage && <ReportMarriageForm />}
                    {selectedSection === ReportSections.ReportDeceased && <ReportDeceasedForm />}
                    {selectedSection === ReportSections.ReportChildren && <ReportChildrenForm />}
                </div>
                <ReportActionsOutput section={selectedSection} />
            </section>
        </ReportActionsProvider>
    );
};

export default ReportActionsSection;
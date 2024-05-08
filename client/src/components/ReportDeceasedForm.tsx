import React, { ChangeEvent, FormEvent, useContext, useState } from "react";
import SelectedFamilyContext from "../context/SelectedFamilyContext";
import ReportDeceasedRequest from "../models/ReportDeceasedRequest";
import ReportActionsContext from "../context/ReportActionsContext";
import PersonElement from "../models/PersonElement";
import { StringDefault, reportDeceased } from "../Utils";
import OutputResponse from "../models/OutputResponse";
import MessageResponse from "../models/MessageResponse";
import _ from "lodash";

const ReportDeceasedForm: React.FC = () => {
    const {selectedFamily} = useContext(SelectedFamilyContext);
    const {setResponse} = useContext(ReportActionsContext);
    const [person,setPerson] = useState<PersonElement>(selectedFamily.member);
    const [deceasedDate, setDeceasedDate] = useState<string>(StringDefault);

    const handleChoosenPerson = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.checked) {
            const name = e.target.value;
            if (selectedFamily.member.name === name) {
                setPerson(selectedFamily.member);
            }
            else if (selectedFamily.inLaw.name === name) {
                setPerson(selectedFamily.inLaw);
            }
        }
    };

    const handleReportDeceased = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const request: ReportDeceasedRequest = {
            element: person,
            deceasedDate: deceasedDate
        };
        const response: OutputResponse<MessageResponse> = await reportDeceased(request);
        setResponse(response);
    };

    return (
        <form onSubmit={handleReportDeceased}>
            <h3>Person To Report:</h3>
            <label><input type="radio" checked={_.isEqual(selectedFamily.member, person)} value={selectedFamily.member.name} onChange={handleChoosenPerson}/>{selectedFamily.member.name}</label><br/>
            <label><input type="radio" checked={_.isEqual(selectedFamily.inLaw, person)} value={selectedFamily.inLaw.name} onChange={handleChoosenPerson}/>{selectedFamily.inLaw.name}</label><br/>
            <h3>Deceased Date To Report:</h3>
            <label>Deceased Date: <input type="text" value={deceasedDate} onChange={(e) => setDeceasedDate(e.target.value)}/></label><br/>
            <button type="submit">Report Deceased</button>
        </form>
    );
};

export default ReportDeceasedForm;
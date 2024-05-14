import React, { ChangeEvent, FormEvent, useContext, useState } from "react";
import ReportDeceasedRequest from "../models/ReportDeceasedRequest";
import ReportActionsContext from "../context/ReportActionsContext";
import PersonElement from "../models/PersonElement";
import { StringDefault, reportDeceased } from "../Utils";
import OutputResponse from "../models/OutputResponse";
import MessageResponse from "../models/MessageResponse";
import _ from "lodash";
import ClientSelectedFamilyElement from "../models/ClientSelectedFamilyElement";

const ReportDeceasedForm: React.FC<ClientSelectedFamilyElement> = (params) => {
    const {setResponse} = useContext(ReportActionsContext);
    const [person,setPerson] = useState<PersonElement>(params.selectedFamily.member);
    const [deceasedDate, setDeceasedDate] = useState<string>(StringDefault);

    const handleChoosenPerson = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.checked) {
            const name = e.target.value;
            if (params.selectedFamily.member.name === name) {
                setPerson(params.selectedFamily.member);
            }
            else if (params.selectedFamily.inLaw.name === name) {
                setPerson(params.selectedFamily.inLaw);
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
            <label><input type="radio" checked={_.isEqual(params.selectedFamily.member, person)} value={params.selectedFamily.member.name} onChange={handleChoosenPerson}/>{params.selectedFamily.member.name}</label><br/>
            <label><input type="radio" checked={_.isEqual(params.selectedFamily.inLaw, person)} value={params.selectedFamily.inLaw.name} onChange={handleChoosenPerson}/>{params.selectedFamily.inLaw.name}</label><br/>
            <h3>Deceased Date To Report:</h3>
            <label>Deceased Date: <input type="text" value={deceasedDate} onChange={(e) => setDeceasedDate(e.target.value)}/></label><br/>
            <button type="submit">Report Deceased</button>
        </form>
    );
};

export default ReportDeceasedForm;
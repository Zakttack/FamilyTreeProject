import React, { ChangeEvent, FormEvent, useContext, useState } from "react";
import _ from 'lodash';
import FamilyElementContext from "../contexts/FamilyElementContext";
import ReportActionsContext from "../contexts/ReportActionsContext";
import useLoadingContext from "../hooks/useLoadingContext";
import PersonElement from "../models/PersonElement";
import ReportDeceasedRequest from "../models/ReportDeceasedRequest";
import { reportDeceased } from "../ApiCalls";
import { StringDefault } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing } from "../Utils";

const ReportDeceasedForm: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {response, isReportMade, setResponse} = useContext(ReportActionsContext);
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [person,setPerson] = useState<PersonElement>(selectedElement.member);
    const [deceasedDate, setDeceasedDate] = useState<string>('');

    const handleChosenPerson = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.checked) {
            const name = e.target.value;
            if (selectedElement.member.name === name) {
                setPerson(selectedElement.member);
            }
            else if (selectedElement.inLaw.name === name) {
                setPerson(selectedElement.inLaw);
            }
        }
    };

    const handleReportDeceased = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            isReportMade(true);
            e.preventDefault();
            if (_.isEqual(deceasedDate, '')) {
                setDeceasedDate(StringDefault);
            }
            const request: ReportDeceasedRequest = {
                element: person,
                deceasedDate: deceasedDate
            };
            addLoadingContext(LoadingContext.ReportDeceased);
            setResponse(await reportDeceased(request));
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.ReportDeceased);
                setDeceasedDate('');
                isReportMade(false);
            }
        }
    };

    return (
        <form onSubmit={handleReportDeceased}>
            <h3>Person To Report:</h3>
            <label><input type="radio" checked={_.isEqual(selectedElement.member, person)} value={selectedElement.member.name} onChange={handleChosenPerson}/>{selectedElement.member.name}</label><br/>
            <label><input type="radio" checked={_.isEqual(selectedElement.inLaw, person)} value={selectedElement.inLaw.name} onChange={handleChosenPerson}/>{selectedElement.inLaw.name}</label><br/>
            <h3>Deceased Date To Report:</h3>
            <label>Deceased Date: <input type="text" value={deceasedDate} onChange={(e) => setDeceasedDate(e.target.value)}/></label><br/>
            <button type="submit">Report Deceased</button>
        </form>
    );
};

export default ReportDeceasedForm;
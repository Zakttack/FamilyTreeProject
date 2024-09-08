import React, { FormEvent, useContext, useState } from "react";
import _ from "lodash";
import FamilyElementContext from "../contexts/FamilyElementContext";
import ReportActionsContext from "../contexts/ReportActionsContext";
import useLoadingContext from "../hooks/useLoadingContext";
import ReportChildrenRequest from "../models/ReportChildrenRequest";
import { reportChildren } from "../ApiCalls";
import { PersonDefault, StringDefault } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing } from "../Utils";

const ReportChildrenForm: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {response, isReportMade, setResponse} = useContext(ReportActionsContext);
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [name, setName] = useState<string>('');
    const [birthDate, setBirthDate] = useState<string>('');
    const [deceasedDate, setDeceasedDate] = useState<string>('');

    const handleReportChildren = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            e.preventDefault();
            isReportMade(true);
            if (_.isEqual(name, '')) {
                setName(StringDefault);
            }
            if (_.isEqual(birthDate, '')) {
                setBirthDate(StringDefault);
            }
            if (_.isEqual(deceasedDate, '')) {
                setDeceasedDate(StringDefault);
            }
            const request: ReportChildrenRequest = {
                parent: selectedElement,
                child: {
                    member: {
                        name: name,
                        birthDate: birthDate,
                        deceasedDate: deceasedDate
                    },
                    inLaw: PersonDefault,
                    marriageDate: StringDefault
                }
            };
            addLoadingContext(LoadingContext.ReportChildren);
            setResponse(await reportChildren(request));
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.ReportChildren);
                setName('');
                setBirthDate('');
                setDeceasedDate('');
                isReportMade(false);
            }
        }
    };

    return (
        <form onSubmit={handleReportChildren}>
            <label>Child Name: <input type="text" value={name} onChange={(e) => setName(e.target.value)}/></label><br/>
            <label>Child Birth Date: <input type="text" value={birthDate} onChange={(e) => setBirthDate(e.target.value)}/></label><br/>
            <label>Child Deceased Date: <input type="text" value={deceasedDate} onChange={(e) => setDeceasedDate(e.target.value)}/></label><br/>
            <button type="submit">Report Children</button>
        </form>
    );
};

export default ReportChildrenForm;
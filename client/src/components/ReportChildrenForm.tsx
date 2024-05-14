import React, { FormEvent, useContext, useState } from "react";
import ReportChildrenRequest from "../models/ReportChildrenRequest";
import ReportActionsContext from "../context/ReportActionsContext";
import { StringDefault, PersonDefault, reportChildren } from "../Utils";
import OutputResponse from "../models/OutputResponse";
import MessageResponse from "../models/MessageResponse";
import ClientSelectedFamilyElement from "../models/ClientSelectedFamilyElement";

const ReportChildrenForm: React.FC<ClientSelectedFamilyElement> = (params) => {
    const {setResponse} = useContext(ReportActionsContext);
    const [name, setName] = useState<string>(StringDefault);
    const [birthDate, setBirthDate] = useState<string>(StringDefault);
    const [deceasedDate, setDeceasedDate] = useState<string>(StringDefault);

    const handleReportChildren = async(e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const request: ReportChildrenRequest = {
            parent: params.selectedFamily,
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
        const response: OutputResponse<MessageResponse> = await reportChildren(request);
        setResponse(response);
    };

    return (
        <form onSubmit={handleReportChildren}>
            <label>Child Name: <input type="text" value={name} onChange={(e) => setName(e.target.value)}/></label><br/>
            <label>Child Birth Date: <input type="text" value={birthDate} onChange={(e) => setBirthDate(e.target.value)}/></label><br/>
            <label>Child Name: <input type="text" value={deceasedDate} onChange={(e) => setDeceasedDate(e.target.value)}/></label><br/>
            <button type="submit">Report Children</button>
        </form>
    );
};

export default ReportChildrenForm;
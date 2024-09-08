import React, {ChangeEvent, FormEvent, useContext, useState} from "react";
import _ from "lodash";
import FamilyElementContext from "../contexts/FamilyElementContext";
import ReportActionsContext from "../contexts/ReportActionsContext";
import useLoadingContext from "../hooks/useLoadingContext";
import ReportMarriageRequest from "../models/ReportMarriageRequest";
import { reportMarriage } from "../ApiCalls";
import { StringDefault } from "../Constants";
import { LoadingContext } from "../Enums";
import { isProcessing } from "../Utils";

function stringToBoolean(value: string) {
    if (value.toLowerCase() === 'true') {
        return true;
    }
    else if (value.toLowerCase() === 'false') {
        return false;
    }
    throw new Error(`"${value}" can't be converted to a boolean.`);
}

const ReportMarriageForm: React.FC = () => {
    const {selectedElement} = useContext(FamilyElementContext);
    const {response, isReportMade, setResponse} = useContext(ReportActionsContext);
    const {addLoadingContext, isLoading, removeLoadingContext} = useLoadingContext();
    const [memberName, setMemberName] = useState<string>(selectedElement.member.name);
    const [memberNameIsBeingCustomized, isMemberNameBeingCustomized] = useState<boolean>(false);
    const [inLawName, setInLawName] = useState<string>('');
    const [inLawBirthDate, setInLawBirthDate] = useState<string>('');
    const [inLawDeceasedDate, setInLawDeceasedDate] = useState<string>('');
    const [marriageDate, setMarriageDate] = useState<string>('');

    const handleMemberNameCustomizationOptions = (e: React.ChangeEvent<HTMLInputElement>) => {
        isMemberNameBeingCustomized(stringToBoolean(e.target.value));
    };

    const handleMemberNameChange = (e: ChangeEvent<HTMLInputElement>) => {
        setMemberName(e.target.value);
    };

    const handleInLawNameChange = (e: ChangeEvent<HTMLInputElement>) => {
        setInLawName(e.target.value);
    };

    const handleInLawBirthDateChange = (e: ChangeEvent<HTMLInputElement>) => {
        setInLawBirthDate(e.target.value);
    };

    const handleInLawDeceasedDateChange = (e: ChangeEvent<HTMLInputElement>) => {
        setInLawDeceasedDate(e.target.value);
    };

    const handleMarriageDateChange = (e: ChangeEvent<HTMLInputElement>) => {
        setMarriageDate(e.target.value);
    };

    const handleReportMarriage = async(e: FormEvent<HTMLFormElement>) => {
        if (!isLoading()) {
            e.preventDefault();
            isReportMade(true);
            if (_.isEqual(memberName, '')) {
                setMemberName(StringDefault);
            }
            if (_.isEqual(inLawName, '')) {
                setInLawName(StringDefault);
            }
            if (_.isEqual(inLawBirthDate, '')) {
                setInLawBirthDate(StringDefault);
            }
            if (_.isEqual(inLawDeceasedDate, '')) {
                setInLawDeceasedDate(StringDefault);
            }
            const request: ReportMarriageRequest = {
                initialMember: selectedElement.member,
                family: {
                    member: {
                        name: memberName,
                        birthDate: selectedElement.member.birthDate,
                        deceasedDate: selectedElement.member.deceasedDate
                    },
                    inLaw: {
                        name: inLawName,
                        birthDate: inLawBirthDate,
                        deceasedDate: inLawDeceasedDate
                    },
                    marriageDate: marriageDate
                }
            };
            addLoadingContext(LoadingContext.ReportMarriage);
            setResponse(await reportMarriage(request));
            if (!isProcessing(response)) {
                removeLoadingContext(LoadingContext.ReportMarriage);
                isReportMade(false);
                setMemberName('');
                setInLawName('');
                setInLawBirthDate('');
                setInLawDeceasedDate('');
            }
        }
    };

    return (
        <form onSubmit={handleReportMarriage}>
            <h3>Member:</h3>
            <label><input type="radio" checked={!memberNameIsBeingCustomized} value={String(false)} onChange={handleMemberNameCustomizationOptions}/>Use {selectedElement.member.name}</label><br/>
            <label><input type="radio" checked={memberNameIsBeingCustomized} value={String(true)} onChange={handleMemberNameCustomizationOptions}/><input type="text" disabled={!memberNameIsBeingCustomized} value={memberName} onChange={handleMemberNameChange}/></label><br/>
            <h3>InLaw:</h3>
            <label>Name: <input type="text" value={inLawName} onChange={handleInLawNameChange}/></label><br/>
            <label>Birth Date: <input type="text" value={inLawBirthDate} onChange={handleInLawBirthDateChange}/></label><br/>
            <label>{'Deceased Date (leave unknown if not deceased): '}<input type="text" value={inLawDeceasedDate} onChange={handleInLawDeceasedDateChange}/></label><br/>
            <h3>Marriage Date:</h3>
            <label>Marriage Date: <input type="text" value={marriageDate} onChange={handleMarriageDateChange}/></label><br/>
            <button type="submit">Report Marriage</button>
        </form>
    )
};

export default ReportMarriageForm;
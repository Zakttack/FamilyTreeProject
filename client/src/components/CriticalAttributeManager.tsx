import React, {useContext, useEffect} from "react";
import _ from "lodash";
import ErrorDisplayComponent from "./ErrorDisplayComponent";
import LoadingComponent from "./LoadingComponent";
import CriticalAttributeContext from "../contexts/CriticalAttributeContext";
import FamilyElementContext from "../contexts/FamilyElementContext";
import FamilyNameContext from "../contexts/FamilyNameContext";
import FamilyTreeContext from "../contexts/FamilyTreeContext";
import TitleContext from "../contexts/TitleContext";
import useLoadingContext from "../hooks/useLoadingContext";
import FamilyElement from "../models/FamilyElement";
import { getClientFamilyName, getClientFamilyTree, getClientSelectedFamily, getClientTitle } from "../ApiCalls";
import { FamilyDefault } from "../Constants";
import { CriticalAttribute, LoadingContext } from "../Enums";
import { isProcessing, isSuccess } from "../Utils";

const CriticalAttributeManager: React.FC<{criticalAttribute: CriticalAttribute}> = (params) => {
    const {criticalAttributeResponse, setCriticalAttributeResponse} = useContext(CriticalAttributeContext);
    const {selectedElement, changeSelectedElement} = useContext(FamilyElementContext);
    const {familyName, setFamilyName} = useContext(FamilyNameContext);
    const {familyTree, setFamilyTree} = useContext(FamilyTreeContext);
    const {title, setTitle} = useContext(TitleContext);
    const {addLoadingContext, removeLoadingContext} = useLoadingContext();

    const getLoadingContext = () => {
        switch (params.criticalAttribute) {
            case CriticalAttribute.FamilyName: return LoadingContext.RetrieveClientFamilyName;
            case CriticalAttribute.FamilyTree: return LoadingContext.RetrieveClientFamilyTree;
            case CriticalAttribute.SelectedFamily: return LoadingContext.RetrieveClientSelectedFamily;
        }
        return LoadingContext.RetrieveClientTitle;
    }

    useEffect(() => {
        const manageCriticalAttribute = async() => {
            switch (params.criticalAttribute) {
                case CriticalAttribute.FamilyName: 
                    if (_.isEqual(familyName, '')) {
                        addLoadingContext(LoadingContext.RetrieveClientFamilyName);
                        setCriticalAttributeResponse(await getClientFamilyName());
                        if (!isProcessing(criticalAttributeResponse)) {
                            removeLoadingContext(LoadingContext.RetrieveClientFamilyName);
                            if (isSuccess(criticalAttributeResponse)) {
                                setFamilyName(criticalAttributeResponse.result as string);
                            }
                        }
                    }
                    break;
                case CriticalAttribute.FamilyTree:
                    if (_.isEqual(familyTree, [])) {
                        addLoadingContext(LoadingContext.RetrieveClientFamilyTree);
                        setCriticalAttributeResponse(await getClientFamilyTree());
                        if (!isProcessing(criticalAttributeResponse)) {
                            removeLoadingContext(LoadingContext.RetrieveClientFamilyTree);
                            if (isSuccess(criticalAttributeResponse)) {
                                setFamilyTree(criticalAttributeResponse.result as FamilyElement[]);
                            }
                        }
                    }
                    break;
                case CriticalAttribute.SelectedFamily:
                    if (_.isEqual(selectedElement, FamilyDefault)) {
                        addLoadingContext(LoadingContext.RetrieveClientSelectedFamily);
                        setCriticalAttributeResponse(await getClientSelectedFamily());
                        if (!isProcessing(criticalAttributeResponse)) {
                            removeLoadingContext(LoadingContext.RetrieveClientSelectedFamily);
                            if (isSuccess(criticalAttributeResponse)) {
                                changeSelectedElement(criticalAttributeResponse.result as FamilyElement);
                            }
                        }
                    }
                    break;
            }
            if (_.isEqual(title, '')) {
                addLoadingContext(LoadingContext.RetrieveClientTitle);
                setCriticalAttributeResponse(await getClientTitle());
                if (!isProcessing(criticalAttributeResponse)) {
                    removeLoadingContext(LoadingContext.RetrieveClientTitle);
                    if (isSuccess(criticalAttributeResponse)) {
                        setTitle(criticalAttributeResponse.result as string);
                    }
                }
            }
        };
        manageCriticalAttribute()
    }, [params, addLoadingContext, removeLoadingContext, setCriticalAttributeResponse, setFamilyName, setFamilyTree, changeSelectedElement, setTitle, criticalAttributeResponse, familyName, familyTree, selectedElement, title]);
    
    return (
        <div>
            <LoadingComponent context={getLoadingContext()} response={criticalAttributeResponse} />
            <ErrorDisplayComponent response={criticalAttributeResponse} />
        </div>
    )
};

export default CriticalAttributeManager;
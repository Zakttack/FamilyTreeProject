export enum PersonType {
    Member = 'Member',
    InLaw = 'InLaw'
};

export enum LoadingContext {
    Default = '',
    FamilyElementToRepresentation = 'Family Element to Representation',
    GenerationNumber = 'Generation Number',
    NumberOfFamilies = 'Number Of Families',
    NumberOfGenerations = 'Number Of Generations',
    PersonElementToRepresentation = 'Person Element To Representation',
    ReportChildren = 'Report Children',
    ReportDeceased = 'Report Deceased',
    ReportMarriage = 'Report Marriage',
    RepresentationToFamilyElement = 'Representation To Family Element',
    RetrieveChildren = 'Retrieve Children',
    RetrieveClientFamilyName = 'Retrieve Client Family Name',
    RetrieveClientFamilyTree = 'Retrieve Client Family Tree',
    RetrieveClientSelectedFamily = 'Retrieve Client Selected Family',
    RetrieveClientTitle = 'Retrieve Client Title',
    RetrieveFamilyTree = 'Retrieve Family Tree',
    RetrieveParent = 'Retrieve Parent',
    RevertTree = 'Revert Tree',
    UpdateClientFamilyName = 'Update Client Family Name',
    UpdateClientFamilyTree = 'Update Client Family Tree',
    UpdateClientSelectedFamily = 'Update Client Selected Family',
    UpdateClientTitle = 'Update Client Title',
    ViewSubtree = 'View Subtree'
}

export enum FamilyTreeApiResponseStatus {
    Failure = 0,
    Processing = -1,
    Success = 1
}

export enum ReportSections {
    Default,
    ReportMarriage,
    ReportDeceased,
    ReportChildren
};

export enum CriticalAttribute {
    FamilyName,
    FamilyTree,
    SelectedFamily,
    Title
}
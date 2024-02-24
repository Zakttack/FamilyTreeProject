export default interface PersonElement {
    name: string | null;
    birthDate: string | null;
    deceasedDate: string | null;
}

export const PersonDefault: PersonElement = {
    name: null,
    birthDate: null,
    deceasedDate: null
};
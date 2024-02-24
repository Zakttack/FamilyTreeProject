import ExceptionResponse from "./exceptionResponse";
export default interface OutputResponse<T> {
    output: T | null,
    problem: ExceptionResponse | null;
}
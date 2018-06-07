export interface SpfConfig{
    records: SpfRecord[];
    errors: string[];
}

export interface SpfRecord{
    index: number;
    version : Version;
    terms: Term[];
}

export interface Version{
    value: string;
    explanation: string;
    errors: string[];
}

export interface Term{
    value: string;
    explanation: string;
    isImplicit: boolean;
    errors: Error[];
}

export interface Error{
    errorType : ErrorType;
    errorScope : string;
    message: string;
}

export enum ErrorType{
    Error,
    Warning,
    Info
}
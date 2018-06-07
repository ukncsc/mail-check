export interface DmarcConfig{
    records: DmarcRecord[];
    errors: Error[];
    totalErrorCount: number;
    maxErrorSeverity: ErrorType;
}

export interface DmarcRecord{
    index: number;
    tags: Tag[];
    errors: Error[];
}

export interface Tag{
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
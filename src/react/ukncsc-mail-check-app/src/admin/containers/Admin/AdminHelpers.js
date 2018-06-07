export const domainRegex = /(?=^.{4,253}$)(^((?!-)[a-zA-Z0-9-]{1,63}\.)+[a-zA-Z]{2,63}\.?$)/;
export const nameRegex = /^[^\r\n$<>%;/\\]{1,255}$/;
export const emailRegex = /\S+@\S+\.\S+/;

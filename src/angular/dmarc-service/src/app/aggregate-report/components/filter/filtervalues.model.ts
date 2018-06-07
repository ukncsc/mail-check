export class FilterValues {
  constructor(
    public beginDate: Date,
    public endDate: Date,
    public domain: string,
    public domainId: number,
    public clear: boolean
  ) {}
}

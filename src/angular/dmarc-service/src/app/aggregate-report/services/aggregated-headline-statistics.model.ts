export class HeadlineStatistics {
  constructor(
    public domainCount: number,
    public emailCount: number,
    public trustedPercentage: number,
    public untrustedPercentage: number,
    public fullyCompliantPercentage: number,
    public untrustedFilteredPercentage: number
  ) {}
}

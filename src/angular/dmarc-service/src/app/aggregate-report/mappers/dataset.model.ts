export class Series {
  public data: number[];

  constructor(public name: string) {
    this.data = [];
  }
}

export class Dataset {
  constructor(public labels: string[], public series: Series[]) {}
}

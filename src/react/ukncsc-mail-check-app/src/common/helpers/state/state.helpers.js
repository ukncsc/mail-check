export const shouldShowLoader = (...args) => args.some(_ => _.loading);

export const isPending = obj =>
  Object.prototype.hasOwnProperty.call(obj, 'records') &&
  !obj.loading &&
  !obj.error &&
  !obj.records;

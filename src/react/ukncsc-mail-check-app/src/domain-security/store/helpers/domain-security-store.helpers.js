import findIndex from 'lodash/findIndex';
import map from 'lodash/map';
import startsWith from 'lodash/startsWith';
import reduce from 'lodash/reduce';

const getRecord = (record, property, inheritedFrom) => {
  const index = findIndex(record[property], _ => startsWith(_.value, 'sp='));

  if (inheritedFrom && index !== -1) {
    record[property][index].explanation +=
      ' This is the policy currently being applied to this domain.';
  }

  return record[property];
};

const implicitTagsReducer = (accumulator, tag) => [
  ...accumulator,
  ...(tag.isImplicit ? [] : [tag.value]),
];

export const recordErrorsReducer = (accumulator, error) => {
  switch (error.errorType) {
    case 'Warning':
      return {
        ...accumulator,
        warnings: [...accumulator.warnings, error.message],
      };
    case 'Error':
      return {
        ...accumulator,
        failures: [...accumulator.failures, error.message],
      };
    case 'Inconclusive':
      return {
        ...accumulator,
        inconclusives: [...accumulator.inconclusives, error.message],
      };
    default:
      return accumulator;
  }
};

export const recordsReducer = (
  property,
  separator = ' ',
  inheritedFrom = null
) => (accumulator, record) => [
  ...accumulator,
  {
    ...record,
    [property]: [
      ...(record.version ? [record.version] : []),
      ...getRecord(record, property, inheritedFrom),
    ],
    record: [
      ...map(record.version ? [record.version] : [], 'value'),
      ...reduce(record[property], implicitTagsReducer, []),
    ].join(separator),
  },
];

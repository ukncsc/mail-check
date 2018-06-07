import map from 'lodash/map';

/* eslint-disable import/prefer-default-export */
export const getSearchUrl = (type, selectedItems = [], term, limit) => {
  const selectedItemsUri =
    selectedItems.length > 0
      ? map(selectedItems, item => `&includedIds=${item}`).join('')
      : '';
  return `/admin/${type}/search/${term}?limit=${limit}${selectedItemsUri}`;
};

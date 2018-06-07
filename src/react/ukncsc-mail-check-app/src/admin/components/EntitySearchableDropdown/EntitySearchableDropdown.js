import React from 'react';
import { Dropdown } from 'semantic-ui-react';

export default ({
  search,
  results,
  typeDescription,
  isLoading,
  onChange,
  onSearchOpen,
  onSearchChange,
}) => (
  <Dropdown
    fluid
    multiple
    selection
    search={search}
    placeholder={`Search for ${typeDescription}...`}
    onChange={onChange}
    loading={isLoading}
    onOpen={onSearchOpen}
    onSearchChange={onSearchChange}
    noResultsMessage={isLoading ? '' : 'No results found'}
    options={results}
  />
);

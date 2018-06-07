import React from 'react';
import { Search } from 'semantic-ui-react';

export default ({
  fetchSearchSuggestions,
  getDomainById,
  getGroupById,
  getUserById,
  isLoading,
  results,
  searchTerm,
  selectCurrentEntity,
  updateSearchTerm,
}) => {
  const categoryRenderer = ({ name }) => <p>{name}</p>;

  const getUserFullName = id => {
    const { firstName, lastName } = getUserById(id);
    return `${firstName} ${lastName}`;
  };

  const getEntityDisplayText = (id, type) => {
    switch (type) {
      case 'user':
        return getUserFullName(id);
      case 'domain':
        return getDomainById(id.toString()).name;
      case 'group':
        return getGroupById(id).name;
      default:
        return null; // TODO: error?
    }
  };

  const onResultSelected = (event, { result: { entityId, entityType } }) => {
    updateSearchTerm(getEntityDisplayText(entityId, entityType));
    selectCurrentEntity({ id: entityId, type: entityType });
  };

  const onSearchChanged = (event, { value }) => {
    updateSearchTerm(value);
    fetchSearchSuggestions(value);
  };

  const resultRenderer = ({ entityId, entityType, id }) => [
    <p key={id}>{getEntityDisplayText(entityId, entityType)}</p>,
  ];

  return (
    <Search
      fluid
      category
      selectFirstResult
      size="big"
      loading={isLoading}
      results={results}
      showNoResults={!isLoading}
      input={{
        fluid: true,
        placeholder: 'Search for users, groups or domains...',
      }}
      categoryRenderer={categoryRenderer}
      resultRenderer={resultRenderer}
      onSearchChange={onSearchChanged}
      onResultSelect={onResultSelected}
      value={searchTerm}
    />
  );
};

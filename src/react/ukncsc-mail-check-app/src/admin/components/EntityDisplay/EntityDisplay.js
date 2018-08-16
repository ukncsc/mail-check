import React from 'react';
import { connect } from 'react-redux';
import { DomainAdmin, GroupAdmin, UserAdmin } from 'admin/components';
import {
  fetchAddToEntity,
  fetchDeleteFromEntity,
  fetchEntities,
  getIdamCurrentEntity,
} from 'admin/store/idam/current-entity';
import {
  fetchIdamEntitySuggestions,
  getIdamEntitySuggestions,
  resetIdamEntitySelection,
  updateIdamEntitySuggesionSelected,
  updateIdamEntitySuggestionType,
  updateIdamEntityTerm,
} from 'admin/store/idam/entity-suggestions';
import {
  getIdamDomainById,
  getIdamGroupById,
  getIdamUserById,
} from 'admin/store/idam/entities';

const EntityDisplay = props => {
  switch (props.currentEntity.type) {
    case 'user':
      return <UserAdmin entityTypes={['group', 'domain']} {...props} />;
    case 'domain':
      return <DomainAdmin entityTypes={['group', 'user']} {...props} />;
    case 'group':
      return <GroupAdmin entityTypes={['domain', 'user']} {...props} />;
    default:
      return <p>Search for users, groups or domains...</p>;
  }
};

const mapStateToProps = state => ({
  currentEntity: getIdamCurrentEntity(state),
  getUserById: getIdamUserById(state),
  getGroupById: getIdamGroupById(state),
  getDomainById: getIdamDomainById(state),
  entitySuggestions: getIdamEntitySuggestions(state),
});

const mapDispatchToProps = dispatch => ({
  fetchEntities: (currentEntity, type) =>
    dispatch(fetchEntities(currentEntity, type)),
  addToEntity: (currentEntity, type, ids) =>
    dispatch(fetchAddToEntity(currentEntity, type, ids)),
  deleteFromEntity: (currentEntity, type, ids) =>
    dispatch(fetchDeleteFromEntity(currentEntity, type, ids)),
  fetchIdamEntitySuggestions: (type, selectedItems, term, limit) =>
    dispatch(fetchIdamEntitySuggestions(type, selectedItems, term, limit)),
  resetIdamEntitySelection: () => dispatch(resetIdamEntitySelection()),
  updateIdamEntityTerm: term => dispatch(updateIdamEntityTerm(term)),
  updateIdamEntitySuggesionSelected: selected =>
    dispatch(updateIdamEntitySuggesionSelected(selected)),
  updateIdamEntitySuggestionType: type =>
    dispatch(updateIdamEntitySuggestionType(type)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(EntityDisplay);

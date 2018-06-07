import React from 'react';
import { Divider } from 'semantic-ui-react';
import map from 'lodash/map';
import { ConfirmationModal } from 'common';
import {
  AddEntitiesModal,
  EntityDetails,
  EntityAccessGroup,
} from 'admin/components';
import withAddRemoveModals from '../withAddRemoveModals';

export const UserComponents = withAddRemoveModals(
  ({
    currentEntity,
    getUserById,
    getUserConfirmationModalMessage,
    hideAddEntityModal,
    hideDeleteEntityModal,
    entitySuggestions,
    mapEntitySuggestions,
    onAddEntityDone,
    onDeleteEntityConfirm,
    onEntitySearchChanged,
    onEntitySearchOpened,
    onEntitySelectionChanged,
    searchEntities,
    shouldShowAddEntityModal,
    shouldShowDeleteEntityModal,
    showAddEntityModal,
    showDeleteEntityModal,
    entityToDelete,
  }) => {
    const { user } = currentEntity.entities;
    return (
      <div>
        <EntityAccessGroup
          title="User Access"
          results={user && map(user.results, getUserById)}
          isLoading={user && user.isLoading}
          onAdd={showAddEntityModal}
          onRemove={showDeleteEntityModal('user')}
        />
        <AddEntitiesModal
          shouldShow={shouldShowAddEntityModal}
          type="user"
          typeDescription="users"
          heading="Add Users to a Group"
          message="Type the name of a user and select them from the list."
          isLoading={entitySuggestions.isLoading}
          results={mapEntitySuggestions(entitySuggestions.results, [
            'firstName',
            'lastName',
          ])}
          onChange={onEntitySelectionChanged}
          search={(dropDownProps, searchTerm) =>
            mapEntitySuggestions(searchEntities(dropDownProps, searchTerm), [
              'firstName',
              'lastName',
            ])
          }
          onSearchOpen={onEntitySearchOpened('user')}
          onSearchChange={onEntitySearchChanged}
          onDone={onAddEntityDone}
          onClose={hideAddEntityModal}
          onCancel={hideAddEntityModal}
        />
        <ConfirmationModal
          shouldShow={shouldShowDeleteEntityModal}
          heading="Remove a User from a Group"
          message={getUserConfirmationModalMessage(entityToDelete)}
          onConfirm={onDeleteEntityConfirm}
          onClose={hideDeleteEntityModal}
          onCancel={hideDeleteEntityModal}
        />
      </div>
    );
  }
);
export const DomainComponents = withAddRemoveModals(
  ({
    currentEntity,
    showAddEntityModal,
    showDeleteEntityModal,
    getDomainConfirmationModalMessage,
    getDomainById,
    shouldShowAddEntityModal,
    entitySuggestions,
    mapEntitySuggestions,
    searchEntities,
    onEntitySelectionChanged,
    onEntitySearchOpened,
    onEntitySearchChanged,
    onAddEntityDone,
    hideAddEntityModal,
    shouldShowDeleteEntityModal,
    onDeleteEntityConfirm,
    hideDeleteEntityModal,
    entityToDelete,
  }) => {
    const { domain } = currentEntity.entities;
    return (
      <div>
        <EntityAccessGroup
          title="Domain Access"
          results={domain && map(domain.results, getDomainById)}
          isLoading={domain && domain.isLoading}
          onAdd={showAddEntityModal}
          onRemove={showDeleteEntityModal('domain')}
        />
        <AddEntitiesModal
          shouldShow={shouldShowAddEntityModal}
          type="domain"
          typeDescription="domains"
          heading="Add Domains to a Group"
          message="Type the name of a domain and select it from the list."
          isLoading={entitySuggestions.isLoading}
          results={mapEntitySuggestions(entitySuggestions.results)}
          onChange={onEntitySelectionChanged}
          search={(dropDownProps, searchTerm) =>
            mapEntitySuggestions(searchEntities(dropDownProps, searchTerm))
          }
          onSearchOpen={onEntitySearchOpened('domain')}
          onSearchChange={onEntitySearchChanged}
          onDone={onAddEntityDone}
          onClose={hideAddEntityModal}
          onCancel={hideAddEntityModal}
        />
        <ConfirmationModal
          shouldShow={shouldShowDeleteEntityModal}
          heading="Remove a Domain from a Group"
          message={getDomainConfirmationModalMessage(entityToDelete)}
          onConfirm={onDeleteEntityConfirm}
          onClose={hideDeleteEntityModal}
          onCancel={hideDeleteEntityModal}
        />
      </div>
    );
  }
);
export default class GroupAdmin extends React.Component {
  componentWillMount() {
    this.props.entityTypes.forEach(type =>
      this.props.fetchEntities(this.props.currentEntity, type)
    );
  }
  componentWillReceiveProps(newProps) {
    const { id, type } = this.props.currentEntity;
    const { id: newId, type: newType } = newProps.currentEntity;
    if (newId !== id || newType !== type) {
      this.props.entityTypes.forEach(entityType =>
        this.props.fetchEntities(this.props.currentEntity, entityType)
      );
    }
  }
  getDomainConfirmationModalMessage = entityToDelete => {
    const { currentEntity, getGroupById, getDomainById } = this.props;
    const { name: groupName = '' } = getGroupById(currentEntity.id);
    const { name: domainName = '' } = getDomainById(entityToDelete.id);
    return `Are you sure you want to remove ${domainName} from ${groupName}`;
  };

  getUserConfirmationModalMessage = entityToDelete => {
    const { currentEntity, getGroupById, getUserById } = this.props;
    const { name: groupName = '' } = getGroupById(currentEntity.id);
    const { firstName = '', lastName = '' } = getUserById(entityToDelete.id);
    return `Are you sure you want to remove ${firstName} ${lastName} from ${groupName}`;
  };

  render() {
    return (
      <div>
        <EntityDetails
          title="Group Details"
          id={this.props.currentEntity.id}
          name={this.props.getGroupById(this.props.currentEntity.id).name}
        />
        <Divider section />
        <UserComponents
          getUserConfirmationModalMessage={this.getUserConfirmationModalMessage}
          {...this.props}
        />
        <Divider section />
        <DomainComponents
          getDomainConfirmationModalMessage={
            this.getDomainConfirmationModalMessage
          }
          {...this.props}
        />
      </div>
    );
  }
}

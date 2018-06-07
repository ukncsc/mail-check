import React from 'react';
import map from 'lodash/map';
import { Divider } from 'semantic-ui-react';
import { ConfirmationModal } from 'common';
import {
  AddEntitiesModal,
  EntityDetails,
  EntityAccessGroup,
} from 'admin/components';
import withAddRemoveModals from '../withAddRemoveModals';

class UserAdmin extends React.Component {
  componentWillMount() {
    this.props.loadAllEntities(this.props.currentEntity);
  }

  componentWillReceiveProps(newProps) {
    const { id, type } = this.props.currentEntity;
    const { id: newId, type: newType } = newProps.currentEntity;

    if (newId !== id || newType !== type) {
      this.props.loadAllEntities(newProps.currentEntity);
    }
  }

  getConfirmationModalMessage = () => {
    const { currentEntity, entityToDelete, getGroupById } = this.props;
    const fullName = this.getUsersFullName(currentEntity.id);
    const { name: groupName = '' } = getGroupById(entityToDelete.id);

    return `Are you sure you want to remove ${fullName} from ${groupName}`;
  };

  getUsersFullName = id => {
    const { firstName, lastName } = this.props.getUserById(id);
    return `${firstName} ${lastName}`;
  };

  getUsersEmail = id => {
    const { email } = this.props.getUserById(id);
    return email;
  };

  render() {
    const {
      currentEntity,
      getDomainById,
      getGroupById,
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
    } = this.props;

    const { group, domain } = currentEntity.entities;

    return (
      <div>
        <EntityDetails
          title="User Details"
          id={currentEntity.id}
          name={this.getUsersFullName(currentEntity.id)}
          email={this.getUsersEmail(currentEntity.id)}
        />
        <Divider section />
        <EntityAccessGroup
          title="Group Membership"
          onAdd={showAddEntityModal}
          onRemove={showDeleteEntityModal('group')}
          results={group && map(group.results, getGroupById)}
          isLoading={group && group.isLoading}
        />
        <Divider section />
        <EntityAccessGroup
          title="Domain Access"
          results={domain && map(domain.results, getDomainById)}
          isLoading={domain && domain.isLoading}
        />
        <AddEntitiesModal
          shouldShow={shouldShowAddEntityModal}
          type="group"
          typeDescription="groups"
          heading="Add a User to Groups"
          message="Type the name of a group and select it from the list."
          isLoading={entitySuggestions.isLoading}
          results={mapEntitySuggestions(entitySuggestions.results)}
          onChange={onEntitySelectionChanged}
          search={(dropDownProps, searchTerm) =>
            mapEntitySuggestions(searchEntities(dropDownProps, searchTerm))
          }
          onSearchOpen={onEntitySearchOpened('group')}
          onSearchChange={onEntitySearchChanged}
          onDone={onAddEntityDone}
          onClose={hideAddEntityModal}
          onCancel={hideAddEntityModal}
        />
        <ConfirmationModal
          shouldShow={shouldShowDeleteEntityModal}
          heading="Remove a User from a Group"
          message={this.getConfirmationModalMessage()}
          onConfirm={onDeleteEntityConfirm}
          onClose={hideDeleteEntityModal}
          onCancel={hideDeleteEntityModal}
        />
      </div>
    );
  }
}

export default withAddRemoveModals(UserAdmin);

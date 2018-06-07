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

class DomainAdmin extends React.Component {
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
    const {
      currentEntity,
      entityToDelete,
      getGroupById,
      getDomainById,
    } = this.props;
    const { name } = getDomainById(currentEntity.id);
    const { name: groupName = '' } = getGroupById(entityToDelete.id);
    return `Are you sure you want to remove ${name} from ${groupName}`;
  };

  render() {
    const {
      currentEntity,
      showAddEntityModal,
      showDeleteEntityModal,
      getGroupById,
      getUserById,
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
    } = this.props;

    const { group, user } = currentEntity.entities;

    return (
      <div>
        <EntityDetails
          title="Domain Details"
          id={currentEntity.id}
          name={getDomainById(currentEntity.id).name}
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
          title="User Access"
          results={user && map(user.results, getUserById)}
          isLoading={user && user.isLoading}
        />
        <AddEntitiesModal
          shouldShow={shouldShowAddEntityModal}
          type="group"
          typeDescription="groups"
          heading="Add a Domain to Groups"
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
          heading="Remove a Domain from a Group"
          message={this.getConfirmationModalMessage()}
          onConfirm={onDeleteEntityConfirm}
          onClose={hideDeleteEntityModal}
          onCancel={hideDeleteEntityModal}
        />
      </div>
    );
  }
}

export default withAddRemoveModals(DomainAdmin);

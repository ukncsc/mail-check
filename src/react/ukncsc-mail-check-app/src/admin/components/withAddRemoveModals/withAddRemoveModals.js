import React from 'react';
import map from 'lodash/map';
import includes from 'lodash/includes';
import indexOf from 'lodash/indexOf';

export default WrappedComponent =>
  class extends React.Component {
    state = {
      shouldShowAddEntityModal: false,
      shouldShowDeleteEntityModal: false,
      entityToDelete: { id: '', type: '' },
    };

    onAddEntityDone = async () => {
      const { currentEntity, addToEntity } = this.props;
      const { type, selected } = this.props.entitySuggestions;
      this.hideAddEntitiesModal();
      await addToEntity(currentEntity, type, selected);
      this.loadAllEntities(currentEntity);
    };

    onDeleteEntityConfirm = async () => {
      const { currentEntity, deleteFromEntity } = this.props;
      const { type, id } = this.state.entityToDelete;
      this.hideDeleteEntityModal();
      await deleteFromEntity(currentEntity, type, [id]);
      this.loadAllEntities(currentEntity);
    };

    onEntitySearchOpened = type => (event, data) => {
      if (event && data) {
        this.props.resetIdamEntitySelection();
        this.props.updateIdamEntitySuggestionType(type);
        this.props.fetchIdamEntitySuggestions(type);
      }
    };

    onEntitySearchChanged = (event, data) => {
      if (event && data) {
        const { type, selected } = this.props.entitySuggestions;
        this.props.updateIdamEntityTerm(data.searchQuery);
        this.props.fetchIdamEntitySuggestions(type, selected, data.searchQuery);
      }
    };

    onEntitySelectionChanged = (event, data) => {
      if (event && data) {
        const { type, term, selected } = this.props.entitySuggestions;
        this.props.updateIdamEntitySuggesionSelected(data.value);
        this.props.fetchIdamEntitySuggestions(type, selected, term);
      }
    };

    hideAddEntitiesModal = () =>
      this.setState({ shouldShowAddEntityModal: false });

    hideDeleteEntityModal = () =>
      this.setState({ shouldShowDeleteEntityModal: false });

    loadAllEntities = currentEntity =>
      this.props.entityTypes.forEach(type =>
        this.props.fetchEntities(currentEntity, type)
      );

    mapEntitySuggestionsToView = (results, keys = ['name']) =>
      map(results, r => ({
        key: r.id,
        value: r.id,
        text: Object.keys(r)
          .filter(k => includes(keys, k))
          .map(k => r[k])
          .join(' '),
      }));

    searchEntities = (dropDownProps, searchTerm = '') => {
      const { results, selected } = this.props.entitySuggestions;
      return results.filter(
        r =>
          r.name
            ? r.name.toLowerCase().startsWith(searchTerm.toLowerCase())
            : (r.firstName.toLowerCase().startsWith(searchTerm.toLowerCase()) ||
                r.lastName
                  .toLowerCase()
                  .startsWith(searchTerm.toLowerCase())) &&
              indexOf(selected, r.id) === -1
      );
    };

    showAddEntityModal = () =>
      this.setState({ shouldShowAddEntityModal: true });

    showDeleteEntityModal = type => ({ id }) =>
      this.setState({
        shouldShowDeleteEntityModal: true,
        entityToDelete: { id, type },
      });

    render() {
      return (
        <WrappedComponent
          hideAddEntityModal={this.hideAddEntitiesModal}
          hideDeleteEntityModal={this.hideDeleteEntityModal}
          loadAllEntities={this.loadAllEntities}
          onAddEntityDone={this.onAddEntityDone}
          onDeleteEntityConfirm={this.onDeleteEntityConfirm}
          onEntitySearchChanged={this.onEntitySearchChanged}
          onEntitySearchOpened={this.onEntitySearchOpened}
          onEntitySelectionChanged={this.onEntitySelectionChanged}
          searchEntities={this.searchEntities}
          showDeleteEntityModal={this.showDeleteEntityModal}
          showAddEntityModal={this.showAddEntityModal}
          mapEntitySuggestions={this.mapEntitySuggestionsToView}
          {...this.state}
          {...this.props}
        />
      );
    }
  };

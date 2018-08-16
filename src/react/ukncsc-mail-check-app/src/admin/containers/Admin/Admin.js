import React from 'react';
import { connect } from 'react-redux';
import includes from 'lodash/includes';
import {
  Grid,
  Segment,
  Button,
  Dropdown,
  Modal,
  Header,
  Icon,
  Form,
} from 'semantic-ui-react';
import { EntityDisplay, EntitiesSearch } from 'admin/components';
import {
  addIdamEntities,
  getIdamUserById,
  getIdamGroupById,
  getIdamDomainById,
} from 'admin/store/idam/entities';
import {
  fetchEntities,
  selectIdamCurrentEntity,
} from 'admin/store/idam/current-entity';
import {
  fetchIdamSearchSuggestions,
  getIdamSearchSuggestions,
  getIdamSearchSuggestionsTerm,
  updateIdamSearchTerm,
} from 'admin/store/idam/search-suggestions';
import { mailCheckApiFetch } from 'common/helpers';
import { domainRegex, nameRegex, emailRegex } from './AdminHelpers';

class Admin extends React.Component {
  state = {
    name: '',
    nameError: false,
    firstName: '',
    firstNameError: false,
    lastName: '',
    lastNameError: false,
    email: '',
    emailError: false,
    createModalType: '',
    shouldShowCreateModal: false,
    isCreating: false,
  };

  onCreateModalClose = () =>
    this.setState({
      name: '',
      nameError: false,
      firstName: '',
      firstNameError: false,
      lastName: '',
      lastNameError: false,
      email: '',
      emailError: false,
      createModalType: '',
      shouldShowCreateModal: false,
      isCreating: false,
    });

  onCreateModalDone = async () => {
    await this.validateCreateForm();
    const { nameError, firstNameError, lastNameError, emailError } = this.state;
    if (nameError || firstNameError || lastNameError || emailError) {
      return;
    }
    this.setState({ shouldShowCreateModal: false });
    try {
      const response = await this.createEntity();
      this.props.updateSearchTerm(
        this.state.name ||
          `${this.state.firstName} ${this.state.lastName} - ${this.state.email}`
      );
      this.props.selectCurrentEntity({
        id: response.id,
        type: this.state.createModalType,
      });
      await this.fetchCreatedSubEntities(response);
      this.onCreateModalClose();
    } catch (err) {
      // TODO: Error handling
      alert(err.message); // eslint-disable-line
    }
  };

  createEntity = async () =>
    mailCheckApiFetch(
      `/admin/${this.state.createModalType}`,
      'POST',
      includes(['group', 'domain'], this.state.createModalType)
        ? {
            name: this.state.name,
          }
        : {
            firstName: this.state.firstName,
            lastName: this.state.lastName,
            email: this.state.email,
          }
    );

  fetchCreatedSubEntities = async response => {
    switch (this.state.createModalType) {
      case 'user':
        await Promise.all([
          this.props.fetchEntities({ id: response.id, type: 'user' }, 'group'),
          this.props.fetchEntities({ id: response.id, type: 'user' }, 'domain'),
        ]);
        break;
      case 'group':
        await Promise.all([
          this.props.fetchEntities({ id: response.id, type: 'group' }, 'user'),
          this.props.fetchEntities(
            { id: response.id, type: 'group' },
            'domain'
          ),
        ]);
        break;
      case 'domain':
        await Promise.all([
          this.props.fetchEntities({ id: response.id, type: 'domain' }, 'user'),
          this.props.fetchEntities(
            { id: response.id, type: 'domain' },
            'group'
          ),
        ]);
        break;
      default:
        break;
    }
  };

  handleNameChange = (event, data) => this.setState({ name: data.value });

  handleFirstNameChange = (event, data) =>
    this.setState({ firstName: data.value });

  handleLastNameChange = (event, data) =>
    this.setState({ lastName: data.value });

  handleEmailChange = (event, data) => this.setState({ email: data.value });

  validateCreateForm = async () => {
    this.setState({
      nameError: false,
      firstNameError: false,
      lastNameError: false,
      emailError: false,
    });

    if (
      this.state.createModalType === 'group' &&
      !nameRegex.test(this.state.name)
    ) {
      await this.setState({ nameError: true });
    } else if (
      this.state.createModalType === 'domain' &&
      !domainRegex.test(this.state.name)
    ) {
      await this.setState({ nameError: true });
    } else if (this.state.createModalType === 'user') {
      if (!nameRegex.test(this.state.firstName)) {
        await this.setState({ firstNameError: true });
      }
      if (!nameRegex.test(this.state.lastName)) {
        await this.setState({ lastNameError: true });
      }
      if (!emailRegex.test(this.state.email)) {
        await this.setState({ emailError: true });
      }
    }
  };

  render() {
    const options = [
      { key: 'user', icon: 'user', text: 'User', value: 'user' },
      { key: 'group', icon: 'users', text: 'Group', value: 'group' },
      { key: 'domain', icon: 'world', text: 'Domain', value: 'domain' },
    ];

    return (
      <Segment raised>
        <Grid stackable>
          <Grid.Row>
            <Grid.Column width={10}>
              <Header as="h1">Admin</Header>
            </Grid.Column>
            <Grid.Column textAlign="right" width={2}>
              <Button.Group primary fluid>
                <Dropdown
                  button
                  multiple
                  closeOnChange
                  value={[]}
                  placeholder="Create"
                  icon={{ name: 'dropdown' }}
                  options={options}
                  onChange={(event, data) =>
                    this.setState({
                      shouldShowCreateModal: true,
                      createModalType: data.value && data.value[0],
                    })
                  }
                />
              </Button.Group>
            </Grid.Column>
          </Grid.Row>
        </Grid>
        <Grid columns={1}>
          <Grid.Row>
            <Grid.Column>
              <EntitiesSearch
                fetchSearchSuggestions={this.props.fetchSearchSuggestions}
                getDomainById={this.props.getDomainById}
                getGroupById={this.props.getGroupById}
                getUserById={this.props.getUserById}
                searchTerm={this.props.searchTerm}
                selectCurrentEntity={this.props.selectCurrentEntity}
                updateSearchTerm={this.props.updateSearchTerm}
                {...this.props.searchSuggestions}
              />
            </Grid.Column>
          </Grid.Row>
          <Grid.Row>
            <Grid.Column>
              <EntityDisplay />
            </Grid.Column>
          </Grid.Row>
        </Grid>
        <Modal
          open={this.state.shouldShowCreateModal}
          onClose={this.onCreateModalClose}
        >
          <Modal.Header>Create {this.state.createModalType}</Modal.Header>
          <Modal.Content>
            <Modal.Description>
              <Header>Create a new {this.state.createModalType}</Header>
              <Form>
                {includes(['domain', 'group'], this.state.createModalType) && (
                  <Form.Input
                    value={this.state.name}
                    onChange={this.handleNameChange}
                    label="Name"
                    error={this.state.nameError}
                    required
                  />
                )}
                {this.state.createModalType === 'user' && (
                  <span>
                    <Form.Input
                      value={this.state.firstName}
                      onChange={this.handleFirstNameChange}
                      label="First Name"
                      error={this.state.firstNameError}
                      required
                    />
                    <Form.Input
                      value={this.state.lastName}
                      onChange={this.handleLastNameChange}
                      label="Last Name"
                      error={this.state.lastNameError}
                      required
                    />
                    <Form.Input
                      value={this.state.email}
                      onChange={this.handleEmailChange}
                      label="Email"
                      error={this.state.emailError}
                      required
                    />
                  </span>
                )}
              </Form>
            </Modal.Description>
          </Modal.Content>
          <Modal.Actions>
            <Button
              color="green"
              onClick={this.onCreateModalDone}
              loading={this.state.isCreating}
            >
              <Icon name="checkmark" />Done
            </Button>
            <Button color="red" onClick={this.onCreateModalClose}>
              <Icon name="remove" />Cancel
            </Button>
          </Modal.Actions>
        </Modal>
      </Segment>
    );
  }
}

const mapStateToProps = state => ({
  getDomainById: getIdamDomainById(state),
  getGroupById: getIdamGroupById(state),
  getUserById: getIdamUserById(state),
  searchSuggestions: getIdamSearchSuggestions(state),
  searchTerm: getIdamSearchSuggestionsTerm(state),
});

const mapDispatchToProps = dispatch => ({
  fetchSearchSuggestions: term => dispatch(fetchIdamSearchSuggestions(term)),
  selectCurrentEntity: ({ id, type }) =>
    dispatch(selectIdamCurrentEntity({ id, type })),
  updateSearchTerm: term => dispatch(updateIdamSearchTerm(term)),
  fetchEntities: (currentEntity, type) =>
    dispatch(fetchEntities(currentEntity, type)),
  addIdamEntities: ({ type, entities }) =>
    dispatch(addIdamEntities({ type, entities })),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Admin);

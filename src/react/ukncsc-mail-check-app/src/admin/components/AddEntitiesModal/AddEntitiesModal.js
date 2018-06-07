import React from 'react';
import PropTypes from 'prop-types';
import { Button, Header, Icon, Modal } from 'semantic-ui-react';
import EntitySearchableDropdown from '../EntitySearchableDropdown';

const AddEntitiesModal = ({
  shouldShow,
  heading,
  message,
  description,
  onDone,
  onCancel,
  onClose,
  ...dropdownProps
}) => (
  <Modal open={shouldShow} onClose={onClose}>
    <Modal.Header>{heading}</Modal.Header>
    <Modal.Content>
      <Modal.Description>
        <Header>{message}</Header>
        <EntitySearchableDropdown {...dropdownProps} />
      </Modal.Description>
    </Modal.Content>
    <Modal.Actions>
      <Button color="green" onClick={onDone}>
        <Icon name="checkmark" />Done
      </Button>
      <Button color="red" onClick={onCancel}>
        <Icon name="remove" />Cancel
      </Button>
    </Modal.Actions>
  </Modal>
);

AddEntitiesModal.propTypes = {
  shouldShow: PropTypes.bool.isRequired,
  heading: PropTypes.string.isRequired,
  message: PropTypes.string.isRequired,
  description: PropTypes.string.isRequired,
  onDone: PropTypes.func.isRequired,
  onCancel: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default AddEntitiesModal;

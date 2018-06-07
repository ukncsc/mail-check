import React from 'react';
import PropTypes from 'prop-types';
import { Modal, Header, Button, Icon } from 'semantic-ui-react';

const ConfirmationModal = ({
  shouldShow,
  heading,
  message,
  description,
  onConfirm,
  onCancel,
  onClose,
}) => (
  <Modal open={shouldShow} onClose={onClose}>
    <Modal.Header>{heading}</Modal.Header>
    <Modal.Content>
      <Modal.Description>
        <Header>{message}</Header>
        {description}
      </Modal.Description>
    </Modal.Content>
    <Modal.Actions>
      <Button color="green" onClick={onConfirm}>
        <Icon name="checkmark" />OK
      </Button>
      <Button color="red" onClick={onCancel}>
        <Icon name="remove" />Cancel
      </Button>
    </Modal.Actions>
  </Modal>
);

ConfirmationModal.defaultProps = {
  heading: 'Confirm',
  message: 'Are you sure you want to do this?',
};

ConfirmationModal.propTypes = {
  shouldShow: PropTypes.bool.isRequired,
  heading: PropTypes.string,
  message: PropTypes.string,
  description: PropTypes.string.isRequired,
  onConfirm: PropTypes.func.isRequired,
  onCancel: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default ConfirmationModal;

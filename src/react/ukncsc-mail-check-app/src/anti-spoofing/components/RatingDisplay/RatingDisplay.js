import React from 'react';
import { Icon, Header } from 'semantic-ui-react';

const getIconProps = status => {
  switch (status) {
    case 'Error':
      return { name: 'exclamation circle', color: 'red' };
    case 'Warning':
      return { name: 'exclamation triangle', color: 'yellow' };
    case 'Success':
      return { name: 'check circle', color: 'green' };
    case 'Pending':
      return { name: 'time', color: 'blue' };
    case 'None':
    case 'Info':
      return {
        name: 'question circle',
        color: 'grey',
      };
    default:
      return {};
  }
};

export default ({ status }) => (
  <Header as="h3">
    <Icon {...getIconProps(status)} />
  </Header>
);

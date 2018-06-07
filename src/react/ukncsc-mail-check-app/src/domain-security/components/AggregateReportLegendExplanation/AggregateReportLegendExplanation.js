import React from 'react';
import { Header } from 'semantic-ui-react';

export default ({ title, description }) => (
  <React.Fragment>
    <Header as="h3">{title}</Header>
    <p>{description}</p>
  </React.Fragment>
);

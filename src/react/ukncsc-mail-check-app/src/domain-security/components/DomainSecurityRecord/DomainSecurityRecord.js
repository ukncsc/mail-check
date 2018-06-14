import React from 'react';
import { Header } from 'semantic-ui-react';

import './DomainSecurityRecord.css';

const DomainSecurityRecord = ({ inheritedFrom, children }) => (
  <React.Fragment>
    <Header as="h3">
      Record{inheritedFrom ? ` inherited from ${inheritedFrom.name}` : ''}
    </Header>
    <p className="DomainSecurityRecord">{children}</p>
  </React.Fragment>
);

export default DomainSecurityRecord;

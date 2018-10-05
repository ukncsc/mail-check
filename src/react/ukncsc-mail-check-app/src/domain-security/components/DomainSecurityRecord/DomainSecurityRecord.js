import React from 'react';
import { Header } from 'semantic-ui-react';

import './DomainSecurityRecord.css';

const DomainSecurityRecord = ({ type, inheritedFrom, children }) => (
  <React.Fragment>
    <Header as="h3">
      Record{type === 'DMARC' && inheritedFrom
        ? ` inherited from ${inheritedFrom}`
        : ''}
    </Header>
    <p className="DomainSecurityRecord">{children}</p>
  </React.Fragment>
);

export default DomainSecurityRecord;

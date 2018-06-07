import React from 'react';

import './DomainSecurityRecord.css';

const DomainSecurityRecord = ({ inheritedFrom, children }) => (
  <React.Fragment>
    <h3>
      Record{inheritedFrom ? ` inherited from ${inheritedFrom.name}` : ''}
    </h3>
    <p className="DomainSecurityRecord">{children}</p>
  </React.Fragment>
);

export default DomainSecurityRecord;

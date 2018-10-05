import React from 'react';
import { Link } from 'react-router-dom';
import { Breadcrumb } from 'semantic-ui-react';

export default ({ children }) => (
  <div>
    <Breadcrumb>
      <Breadcrumb.Section>
        <Link to="/">Home</Link>
      </Breadcrumb.Section>
      {children}
    </Breadcrumb>
  </div>
);

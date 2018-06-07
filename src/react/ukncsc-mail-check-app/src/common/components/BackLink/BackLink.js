import React from 'react';
import { Link, withRouter } from 'react-router-dom';
import { Icon } from 'semantic-ui-react';

import './BackLink.css';

const DynamicBackLink = ({ link, history, children }) => (
  <p>
    {link ? (
      <Link to={link}>{children}</Link>
    ) : (
      <span className="BackLink" onClick={history.goBack}>
        {children}
      </span>
    )}
  </p>
);

const BackLink = props => (
  <DynamicBackLink {...props}>
    <Icon name="caret left" />Back
  </DynamicBackLink>
);

export default withRouter(BackLink);

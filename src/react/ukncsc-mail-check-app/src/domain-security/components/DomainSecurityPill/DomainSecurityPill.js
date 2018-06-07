import React from 'react';
import { Label } from 'semantic-ui-react';
import PropTypes from 'prop-types';

import './DomainSecurityPill.css';

const getColor = (error, warning, information) =>
  (error && 'red') ||
  (warning && 'yellow') ||
  (information && 'grey') ||
  'green';

const DomainSecurityPill = ({ error, warning, information, children }) => (
  <Label circular color={getColor(error, warning, information)} size="large">
    <span className="DomainSecurityPill">{children}</span>
  </Label>
);

DomainSecurityPill.defaultProps = {
  error: false,
  warning: false,
  information: false,
};

DomainSecurityPill.propTypes = {
  error: PropTypes.bool,
  warning: PropTypes.bool,
  information: PropTypes.bool,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]).isRequired,
};

export default DomainSecurityPill;

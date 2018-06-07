import React from 'react';
import MarkDown from 'react-markdown';
import PropTypes from 'prop-types';
import { Message } from 'semantic-ui-react';
import { removeMailtoHyperlinksRenderer as link } from 'common/helpers/markdown';

import './DomainSecurityMessage.css';

const DomainSecurityMessage = ({ children, markdown, ...props }) => (
  <Message {...props} className="DomainSecurityMessage--container">
    {markdown ? <MarkDown source={children} renderers={{ link }} /> : children}
  </Message>
);

DomainSecurityMessage.defaultProps = {
  error: false,
  warning: false,
  success: false,
  info: false,
  markdown: false,
  size: 'large',
};

DomainSecurityMessage.propTypes = {
  error: PropTypes.bool,
  warning: PropTypes.bool,
  success: PropTypes.bool,
  info: PropTypes.bool,
  markdown: PropTypes.bool,
  size: PropTypes.string,
};

export default DomainSecurityMessage;

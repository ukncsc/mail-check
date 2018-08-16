import React from 'react';
import MarkDown from 'react-markdown';
import PropTypes from 'prop-types';
import { Message } from 'semantic-ui-react';
import { removeMailtoHyperlinksRenderer as link } from 'common/helpers/markdown';

import './MailCheckMessage.css';

/**
 * This component renders markdown from the API in GitHub Flavored Markdown: https://github.github.com/gfm/
 */

const MailCheckMessage = ({ children, markdown, fluid, ...props }) => (
  <Message
    {...props}
    style={{ margin: '0.2em 0em', maxWidth: fluid ? null : 600 }}
    className="MailCheckMessage--container"
  >
    {markdown ? <MarkDown source={children} renderers={{ link }} /> : children}
  </Message>
);

MailCheckMessage.defaultProps = {
  error: false,
  warning: false,
  success: false,
  fluid: false,
  info: false,
  markdown: false,
  size: null,
};

MailCheckMessage.propTypes = {
  error: PropTypes.bool,
  warning: PropTypes.bool,
  success: PropTypes.bool,
  fluid: PropTypes.bool,
  info: PropTypes.bool,
  markdown: PropTypes.bool,
  size: PropTypes.string,
};

export default MailCheckMessage;

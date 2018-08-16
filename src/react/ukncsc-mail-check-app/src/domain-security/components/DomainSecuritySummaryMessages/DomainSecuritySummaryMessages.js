import React from 'react';
import PropTypes from 'prop-types';
import { MailCheckMessage } from 'common/components';

const DomainSecuritySummaryMessages = ({
  type,
  failures,
  warnings,
  inconclusives,
}) => (
  <React.Fragment>
    {!!failures && (
      <MailCheckMessage error>
        {type} configuration has {failures} urgent issue{failures === 1
          ? ''
          : 's'}
      </MailCheckMessage>
    )}
    {!!warnings && (
      <MailCheckMessage warning>
        {type} configuration has {warnings} advisory issue{warnings === 1
          ? ''
          : 's'}
      </MailCheckMessage>
    )}
    {!!inconclusives && (
      <MailCheckMessage>
        {type} configuration has {inconclusives} inconclusive issue{inconclusives ===
        1
          ? ''
          : 's'}
      </MailCheckMessage>
    )}
    {!failures &&
      !warnings &&
      !inconclusives && (
        <MailCheckMessage success>{type} well configured.</MailCheckMessage>
      )}
  </React.Fragment>
);

DomainSecuritySummaryMessages.defaultProps = {
  failures: 0,
  warnings: 0,
  inconclusives: 0,
};

DomainSecuritySummaryMessages.propTypes = {
  type: PropTypes.string.isRequired,
  failures: PropTypes.number,
  warnings: PropTypes.number,
  inconclusives: PropTypes.number,
};

export default DomainSecuritySummaryMessages;

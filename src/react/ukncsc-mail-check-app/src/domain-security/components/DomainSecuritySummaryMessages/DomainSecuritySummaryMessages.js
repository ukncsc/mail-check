import React from 'react';
import PropTypes from 'prop-types';
import { DomainSecurityMessage } from 'domain-security/components';

const DomainSecuritySummaryMessages = ({
  type,
  failures,
  warnings,
  inconclusives,
}) => (
  <React.Fragment>
    {!!failures && (
      <DomainSecurityMessage error>
        {type} configuration has {failures} urgent issue{failures === 1
          ? ''
          : 's'}
      </DomainSecurityMessage>
    )}
    {!!warnings && (
      <DomainSecurityMessage warning>
        {type} configuration has {warnings} advisory issue{warnings === 1
          ? ''
          : 's'}
      </DomainSecurityMessage>
    )}
    {!!inconclusives && (
      <DomainSecurityMessage information>
        {type} configuration has {inconclusives} inconclusive issue{inconclusives ===
        1
          ? ''
          : 's'}
      </DomainSecurityMessage>
    )}
    {!failures &&
      !warnings &&
      !inconclusives && (
        <DomainSecurityMessage success>
          {type} is well configured.
        </DomainSecurityMessage>
      )}
  </React.Fragment>
);

DomainSecuritySummaryMessages.defaultProps = {
  failures: 0,
  warnings: 0,
  inconclusives: 0,
};

DomainSecuritySummaryMessages.propTypes = {
  type: PropTypes.oneOf(['DMARC', 'SPF', 'TLS']).isRequired,
  failures: PropTypes.number,
  warnings: PropTypes.number,
  inconclusives: PropTypes.number,
};

export default DomainSecuritySummaryMessages;

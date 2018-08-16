import React from 'react';
import pluralise from 'pluralize';
import PropTypes from 'prop-types';
import { Divider } from 'semantic-ui-react';
import { DomainSecurityPill } from 'domain-security/components';
import { MailCheckMessage } from 'common/components';
import './DomainSecurityDetailsMessages.css';

const DomainSecurityDetailsMessages = ({
  type,
  markdown,
  failures,
  warnings,
  inconclusives,
}) => (
  <React.Fragment>
    {!!failures.length && (
      <React.Fragment>
        <DomainSecurityPill error>{failures.length} Urgent</DomainSecurityPill>
        <p className="DomainSecuritySummaryMessages--intro">
          Your {type} configuration has{' '}
          {pluralise('element', failures.length, true)} that{' '}
          {pluralise.singular('require', failures.length)} urgent attention.
        </p>
        {failures.map((description, i) => (
          <MailCheckMessage error markdown={markdown} key={i}>
            {description}
          </MailCheckMessage>
        ))}
        <Divider hidden />
      </React.Fragment>
    )}
    {!!warnings.length && (
      <React.Fragment>
        <DomainSecurityPill warning>
          {warnings.length} Advisory
        </DomainSecurityPill>
        <p className="DomainSecuritySummaryMessages--intro">
          Your {type} configuration has{' '}
          {pluralise('element', warnings.length, true)} that can be improved.
        </p>
        {warnings.map((description, i) => (
          <MailCheckMessage warning markdown={markdown} key={i}>
            {description}
          </MailCheckMessage>
        ))}
        <Divider hidden />
      </React.Fragment>
    )}
    {!!inconclusives.length && (
      <React.Fragment>
        <DomainSecurityPill information>
          {inconclusives.length} Inconclusive
        </DomainSecurityPill>
        <p className="DomainSecuritySummaryMessages--intro">
          Your {type} configuration has {inconclusives.length} inconclusive{' '}
          {pluralise('element', inconclusives.length)}.
        </p>
        {inconclusives.map((description, i) => (
          <MailCheckMessage markdown={markdown} key={i}>
            {description}
          </MailCheckMessage>
        ))}
      </React.Fragment>
    )}
    {!failures.length &&
      !warnings.length &&
      !inconclusives.length && (
        <React.Fragment>
          <DomainSecurityPill>1 Positive</DomainSecurityPill>
          <Divider hidden />
          <MailCheckMessage success>{type} well configured.</MailCheckMessage>
        </React.Fragment>
      )}
  </React.Fragment>
);

DomainSecurityDetailsMessages.defaultProps = {
  markdown: false,
  failures: [],
  warnings: [],
  inconclusives: [],
};

DomainSecurityDetailsMessages.propTypes = {
  type: PropTypes.string.isRequired,
  markdown: PropTypes.bool,
  failures: PropTypes.arrayOf(PropTypes.string),
  warnings: PropTypes.arrayOf(PropTypes.string),
  inconclusives: PropTypes.arrayOf(PropTypes.string),
};

export default DomainSecurityDetailsMessages;

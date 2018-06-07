import React from 'react';
import map from 'lodash/map';
import pluralise from 'pluralize';
import PropTypes from 'prop-types';
import { Divider } from 'semantic-ui-react';
import {
  DomainSecurityMessage,
  DomainSecurityPill,
} from 'domain-security/components';

/**
 * This component renders markdown from the API in GitHub Flavored Markdown: https://github.github.com/gfm/
 */

const DomainSecurityDetailsMessages = ({
  type,
  failures,
  warnings,
  inconclusives,
}) => (
  <React.Fragment>
    {!!failures.length && (
      <React.Fragment>
        <DomainSecurityPill error>{failures.length} Urgent</DomainSecurityPill>
        <p className="DomainSecurityDetails--description__error">
          Your {type} has {pluralise('element', failures.length, true)} that{' '}
          {pluralise.singular('require', failures.length)} urgent attention.
        </p>
        {map(failures, (description, i) => (
          <DomainSecurityMessage error markdown key={i}>
            {description}
          </DomainSecurityMessage>
        ))}
        <Divider hidden />
      </React.Fragment>
    )}
    {!!warnings.length && (
      <React.Fragment>
        <DomainSecurityPill warning>
          {warnings.length} Advisory
        </DomainSecurityPill>
        <p className="DomainSecurityDetails--description__warning">
          Your {type} has {pluralise('element', warnings.length, true)} that can
          be improved.
        </p>
        {map(warnings, (description, i) => (
          <DomainSecurityMessage warning markdown key={i}>
            {description}
          </DomainSecurityMessage>
        ))}
        <Divider hidden />
      </React.Fragment>
    )}
    {!!inconclusives.length && (
      <React.Fragment>
        <DomainSecurityPill information>
          {inconclusives.length} Inconclusive
        </DomainSecurityPill>
        <p className="DomainSecurityDetails--description__information">
          Your {type} has {inconclusives.length} inconclusive{' '}
          {pluralise('element', inconclusives.length)}.
        </p>
        {map(inconclusives, (description, i) => (
          <DomainSecurityMessage markdown key={i}>
            {description}
          </DomainSecurityMessage>
        ))}
      </React.Fragment>
    )}
    {!failures.length &&
      !warnings.length &&
      !inconclusives.length && (
        <React.Fragment>
          <DomainSecurityPill>1 Positive</DomainSecurityPill>
          <Divider hidden />
          <DomainSecurityMessage success>
            {type} is well configured.
          </DomainSecurityMessage>
        </React.Fragment>
      )}
  </React.Fragment>
);

DomainSecurityDetailsMessages.defaultProps = {
  failures: [],
  warnings: [],
  inconclusives: [],
};

DomainSecurityDetailsMessages.propTypes = {
  type: PropTypes.oneOf(['DMARC', 'SPF', 'TLS']).isRequired,
  failures: PropTypes.arrayOf(PropTypes.string),
  warnings: PropTypes.arrayOf(PropTypes.string),
  inconclusives: PropTypes.arrayOf(PropTypes.string),
};

export default DomainSecurityDetailsMessages;

import React from 'react';
import { Link } from 'react-router-dom';
import find from 'lodash/find';
import toString from 'lodash/toString';
import trimEnd from 'lodash/trimEnd';
import { Divider, Message } from 'semantic-ui-react';
import {
  DomainSecurityDetailsMessages,
  DomainSecurityTitle,
} from 'domain-security/components';
import { BackLink } from 'common/components';
import { DomainSecurityContext } from 'domain-security/context';

const DomainSecurityDetailsTls = ({ tls = {}, match }) => {
  const record = find(tls.records, r => toString(r.id) === match.params.id);

  return (
    <React.Fragment>
      <DomainSecurityContext.Consumer>
        {value => <BackLink link={`/${value}/${match.params.domainId}`} />}
      </DomainSecurityContext.Consumer>
      <DomainSecurityTitle
        title={record && trimEnd(record.hostname, '.')}
        loading={tls.loading}
        error={tls.error}
        subtitle="TLS"
        lastChecked={record && record.lastChecked}
        failures={record && record.failures}
        warnings={record && record.warnings}
        inconclusives={record && record.inconclusives}
        pending={tls.hostname === null}
      />
      <p>
        <Link to="/domain-security/tls-advice">View NCSC advice on TLS</Link>
      </p>
      <Divider hidden />
      {tls.error && <Message error>{tls.error.message}</Message>}
      {!tls.error &&
        !tls.loading &&
        record && (
          <DomainSecurityDetailsMessages
            type="TLS"
            failures={record.failures}
            warnings={record.warnings}
            inconclusives={record.inconclusives}
          />
        )}
    </React.Fragment>
  );
};

export default DomainSecurityDetailsTls;

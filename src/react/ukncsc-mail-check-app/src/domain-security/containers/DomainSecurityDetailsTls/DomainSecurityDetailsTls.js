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
      {record && (
        <DomainSecurityTitle
          loading={tls.loading}
          subtitle={trimEnd(record.hostname, '.')}
          failures={record.failures}
          warnings={record.warnings}
          inconclusives={record.inconclusives}
          pending={tls.hostname === null}
          lastChecked={record.lastChecked}
        >
          TLS
        </DomainSecurityTitle>
      )}
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

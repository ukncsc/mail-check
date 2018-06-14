import React from 'react';
import flatMap from 'lodash/flatMap';
import { Divider, Grid, Header, Loader, Message } from 'semantic-ui-react';
import {
  AggregateReportSummary,
  DomainSecurityRecordSummary,
  DomainSecurityTitle,
  DomainSecurityTlsSummary,
} from 'domain-security/components';
import { BackLink } from 'common/components';
import { isPending, shouldShowLoader } from 'common/helpers';
import {
  dmarcDescription,
  emailSecurityDescription,
  spfDescription,
  tlsDescription,
} from 'domain-security/data';
import { DomainSecurityContext } from 'domain-security/context';

import './DomainSecuritySummary.css';

const DomainSecuritySummary = ({
  dmarc,
  domain,
  spf,
  tls,
  aggregateReportInfo,
}) => (
  <React.Fragment>
    <DomainSecurityContext.Consumer>
      {value => <BackLink link={`/${value}`} />}
    </DomainSecurityContext.Consumer>
    {!domain.loading &&
      !domain.error &&
      domain.name && <Header as="h1">{domain.name}</Header>}
    {shouldShowLoader(dmarc, domain, spf, tls, aggregateReportInfo) && (
      <Loader active inline style={{ marginRight: 20 }} />
    )}
    {domain.error && (
      <Message error>
        There was a problem retrieving the details of this domain:{' '}
        {domain.error.message}
      </Message>
    )}
    <Header as="h2">Email security summary</Header>
    <p style={{ maxWidth: 550 }}>{emailSecurityDescription}</p>
    <AggregateReportSummary {...aggregateReportInfo} />
    <Divider className="DomainSecuritySummary--divider" />
    <Grid stackable>
      <Grid.Row>
        <Grid.Column width="4">
          <DomainSecurityTitle
            as="h2"
            failures={dmarc.failures}
            warnings={dmarc.warnings}
            inconclusives={dmarc.inconclusives}
            pending={isPending(dmarc)}
          >
            DMARC
          </DomainSecurityTitle>
        </Grid.Column>
        <Grid.Column width="8">
          <DomainSecurityRecordSummary item={dmarc} type="DMARC">
            <p>{dmarcDescription}</p>
          </DomainSecurityRecordSummary>
        </Grid.Column>
      </Grid.Row>
      <Divider className="DomainSecuritySummary--divider" />
      <Grid.Row>
        <Grid.Column width="4">
          <DomainSecurityTitle
            as="h2"
            failures={spf.failures}
            warnings={spf.warnings}
            inconclusives={spf.inconclusives}
            pending={isPending(spf)}
          >
            SPF
          </DomainSecurityTitle>
        </Grid.Column>
        <Grid.Column width="8">
          <DomainSecurityRecordSummary item={spf} type="SPF">
            <p>{spfDescription}</p>
          </DomainSecurityRecordSummary>
        </Grid.Column>
      </Grid.Row>
      <Divider className="DomainSecuritySummary--divider" />
      <Grid.Row>
        <Grid.Column width="4">
          <DomainSecurityTitle
            as="h2"
            failures={flatMap(tls.records, r => r.failures || [])}
            warnings={flatMap(tls.records, r => r.warnings || [])}
            inconclusives={flatMap(tls.records, r => r.inconclusives || [])}
            pending={tls && tls.pending}
          >
            TLS
          </DomainSecurityTitle>
        </Grid.Column>
        <Grid.Column width="8">
          <DomainSecurityTlsSummary {...tls}>
            <p>{tlsDescription}</p>
          </DomainSecurityTlsSummary>
        </Grid.Column>
      </Grid.Row>
      <Divider className="DomainSecuritySummary--divider" />
    </Grid>
  </React.Fragment>
);

export default DomainSecuritySummary;

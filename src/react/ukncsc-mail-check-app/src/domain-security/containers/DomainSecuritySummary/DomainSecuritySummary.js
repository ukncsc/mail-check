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
import {
  dmarcDescription,
  spfDescription,
  tlsDescription,
} from 'domain-security/data';
import { DomainSecurityContext } from 'domain-security/context';

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
    <Header as="h1">
      {domain.loading && <Loader active inline style={{ marginRight: 20 }} />}
      {!domain.loading &&
        !domain.error && (
          <React.Fragment>
            {domain.name}
            <Header.Subheader>Domain Security Summary</Header.Subheader>
          </React.Fragment>
        )}
    </Header>
    {domain.error && (
      <Message error>
        There was a problem retrieving the details of this domain:{' '}
        {domain.error.message}
      </Message>
    )}
    <AggregateReportSummary aggregateReportInfo={aggregateReportInfo} />
    <Divider />
    <Grid stackable>
      <Grid.Row columns={2}>
        <Grid.Column>
          <DomainSecurityTitle
            title="DMARC"
            loading={dmarc.loading}
            error={dmarc.error}
            failures={dmarc.failures}
            warnings={dmarc.warnings}
            inconclusives={dmarc.inconclusives}
            pending={!dmarc.loading && !dmarc.error && !dmarc.records}
          />
        </Grid.Column>
        <Grid.Column>
          <DomainSecurityRecordSummary
            {...dmarc}
            description={dmarcDescription}
            type="DMARC"
          />
        </Grid.Column>
      </Grid.Row>
      <Divider />
      <Grid.Row columns={2}>
        <Grid.Column>
          <DomainSecurityTitle
            title="SPF"
            loading={spf.loading}
            error={spf.error}
            failures={spf.failures}
            warnings={spf.warnings}
            inconclusives={spf.inconclusives}
            pending={!spf.loading && !spf.error && !spf.records}
          />
        </Grid.Column>
        <Grid.Column>
          <DomainSecurityRecordSummary
            {...spf}
            description={spfDescription}
            type="SPF"
          />
        </Grid.Column>
      </Grid.Row>
      <Divider />
      <Grid.Row columns={2}>
        <Grid.Column>
          <DomainSecurityTitle
            title="TLS"
            loading={tls.loading}
            error={tls.error}
            failures={flatMap(tls.records, r => r.failures || [])}
            warnings={flatMap(tls.records, r => r.warnings || [])}
            inconclusives={flatMap(tls.records, r => r.inconclusives || [])}
            pending={tls && tls.pending}
          />
        </Grid.Column>
        <Grid.Column>
          <DomainSecurityTlsSummary {...tls} description={tlsDescription} />
        </Grid.Column>
      </Grid.Row>
      <Divider hidden />
    </Grid>
  </React.Fragment>
);

export default DomainSecuritySummary;

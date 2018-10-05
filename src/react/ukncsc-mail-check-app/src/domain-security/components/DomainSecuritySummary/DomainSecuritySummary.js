import React, { Component } from 'react';
import { Divider, Header, Image } from 'semantic-ui-react';
import { Link } from 'react-router-dom';
import {
  Breadcrumb,
  BreadcrumbItem,
  MailCheckMessage,
} from 'common/components';
import {
  AggregateReportSummaryContainer,
  DomainSecuritySummaryCertificatesContainer,
  DomainSecuritySummaryDkimContainer,
  DomainSecuritySummaryDmarcContainer,
  DomainSecuritySummarySpfContainer,
  DomainSecuritySummarySubdomainsContainer,
  DomainSecuritySummaryTlsContainer,
} from 'domain-security/containers';
import { DomainSecurityLocationContext } from 'domain-security/context';

import doc from 'domain-security/assets/doc.png';

import './DomainSecuritySummary.css';

export default class DomainSecuritySummary extends Component {
  componentDidMount() {
    const {
      getDomainSecurityDomain,
      fetchDomainSecurityDomain,
      match: { params },
    } = this.props;

    if (!getDomainSecurityDomain(params.domainId)) {
      fetchDomainSecurityDomain(params.domainId);
    }
  }

  render() {
    const {
      description,
      getDomainSecurityDomain,
      match: { params },
    } = this.props;

    const domain = getDomainSecurityDomain(params.domainId);

    return (
      <React.Fragment>
        <Breadcrumb>
          <BreadcrumbItem active>{domain && domain.name}</BreadcrumbItem>
        </Breadcrumb>
        {domain && (
          <React.Fragment>
            {domain.name && <Header as="h1">{domain.name}</Header>}
            {domain.error && (
              <MailCheckMessage error fluid>
                There was a problem retrieving the details of this domain:{' '}
                {domain.error.message}
              </MailCheckMessage>
            )}
          </React.Fragment>
        )}
        <Header as="h2">Email security summary</Header>
        <p style={{ maxWidth: 550 }}>{description}</p>
        <AggregateReportSummaryContainer domainId={params.domainId} />
        {this.props.canViewAggregateData(params.domainId) && (
          <React.Fragment>
            <Header as="h3">Download</Header>
            <p>
              <Image src={doc} inline style={{ marginRight: 20 }} />
              <DomainSecurityLocationContext.Consumer>
                {location => (
                  <Link
                    to={`/${location}/${
                      params.domainId
                    }/dmarc/aggregate-export`}
                  >
                    Download aggregate report data as CSV
                  </Link>
                )}
              </DomainSecurityLocationContext.Consumer>
            </p>
          </React.Fragment>
        )}
        <Divider className="DomainSecuritySummary--divider" />
        <DomainSecuritySummaryDmarcContainer domainId={params.domainId} />
        <Divider className="DomainSecuritySummary--divider" />
        <DomainSecuritySummarySpfContainer domainId={params.domainId} />
        <Divider className="DomainSecuritySummary--divider" />
        <DomainSecuritySummaryDkimContainer
          domainId={params.domainId}
          domainName={domain && domain.name}
        />
        <Divider className="DomainSecuritySummary--divider" />
        <DomainSecuritySummaryTlsContainer domainId={params.domainId} />
        <Divider className="DomainSecuritySummary--divider" />
        <DomainSecuritySummaryCertificatesContainer
          domainId={params.domainId}
          domainName={domain && domain.name}
        />
        <Divider className="DomainSecuritySummary--divider" />
        <DomainSecuritySummarySubdomainsContainer
          domainId={params.domainId}
          domainName={domain && domain.name}
        />
      </React.Fragment>
    );
  }
}

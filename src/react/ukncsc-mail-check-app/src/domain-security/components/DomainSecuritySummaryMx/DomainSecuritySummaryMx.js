import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Grid, Header, Label } from 'semantic-ui-react';
import kebabCase from 'lodash/kebabCase';
import flatMap from 'lodash/flatMap';
import {
  DomainSecurityRecord,
  DomainSecuritySummaryMessages,
  DomainSecurityTitle,
} from 'domain-security/components';
import { DomainSecurityLocationContext } from 'domain-security/context';
import { MailCheckMessage } from 'common/components';

const shouldShowNoMxRecordsMessage = (type, { pending, records }) =>
  type.startsWith('TLS') &&
  !pending &&
  records &&
  (!records.length || records.some(record => !record.hostname));

const shouldShowRecords = ({ loading, error, records }) =>
  !loading && !error && records;

export default class DomainSecuritySummaryMx extends Component {
  componentDidMount() {
    const {
      domainId,
      fetchDomainSecurityMx,
      getDomainSecurityMx,
      type,
    } = this.props;

    if (type === 'TLS') {
      if (!getDomainSecurityMx(domainId)) {
        fetchDomainSecurityMx(domainId);
      }
    } else {
      this.fetchDomainBasedMx();
    }
  }

  componentDidUpdate() {
    if (this.props.type !== 'TLS') {
      this.fetchDomainBasedMx();
    }
  }

  getIdentifier = () =>
    this.props.type === 'TLS' ? this.props.domainId : this.props.domainName;

  getRecordPrefix = () => (this.props.type === 'DKIM' ? 'Selector : ' : '');

  getNoResultMessage() {
    switch (this.props.type) {
      case 'TLS Certificates':
      case 'TLS':
        return `We're currently checking your MX records. This usually takes less than 15 minutes, so check back soon.`;
      case 'DKIM':
        return `We haven't seen any DKIM selectors for this domain.`;
      default:
        return `We're currently checking your ${
          this.props.type
        } configuration. Please check back later.`;
    }
  }

  fetchDomainBasedMx() {
    const {
      domainName,
      fetchDomainSecurityMx,
      getDomainSecurityMx,
    } = this.props;

    if (domainName && !getDomainSecurityMx(domainName)) {
      fetchDomainSecurityMx(domainName);
    }
  }

  render() {
    const { description, domainId, getDomainSecurityMx, type } = this.props;
    const mx = getDomainSecurityMx(this.getIdentifier()) || {};
    const recordPrefix = this.getRecordPrefix();
    const noResultMesssage = this.getNoResultMessage();

    return (
      <Grid stackable>
        <Grid.Row>
          <Grid.Column width="4">
            <DomainSecurityTitle
              as="h2"
              title={type}
              failures={flatMap(mx.records, r => r.failures || [])}
              warnings={flatMap(mx.records, r => r.warnings || [])}
              inconclusives={flatMap(mx.records, r => r.inconclusives || [])}
              pending={mx.pending}
              loading={mx.loading}
            />
            { type === 'DKIM' && (
              <div>
                <Label color="blue">In Development</Label>
              </div>
            )}
          </Grid.Column>
          <Grid.Column width="8">
            <p>{description}</p>

            {shouldShowRecords(mx) && (
              <React.Fragment>
                {mx.records.filter(record => !!record.hostname).map(record => (
                  <React.Fragment key={record.hostname}>
                    <Header as="h3">
                      {recordPrefix}
                      {record.hostname}
                    </Header>
                    {record.records &&
                      record.records.map(_ => (
                        <DomainSecurityRecord type="DKIM">
                          {_.record}
                        </DomainSecurityRecord>
                      ))}
                    <p>
                      <DomainSecurityLocationContext.Consumer>
                        {location => (
                          <Link
                            to={`/${location}/${domainId}/${kebabCase(
                              type.toLowerCase()
                            )}/${record.id || record.hostname}`}
                          >
                            View more information
                          </Link>
                        )}
                      </DomainSecurityLocationContext.Consumer>
                    </p>

                    <DomainSecuritySummaryMessages
                      type={type}
                      failures={record.failures.length}
                      warnings={record.warnings.length}
                      inconclusives={record.inconclusives.length}
                    />
                  </React.Fragment>
                ))}

                {shouldShowNoMxRecordsMessage(type, mx) && (
                  <MailCheckMessage success>
                    No MX records found.
                  </MailCheckMessage>
                )}
              </React.Fragment>
            )}

            {mx.pending && (
              <MailCheckMessage info>{noResultMesssage}</MailCheckMessage>
            )}

            {mx.error && (
              <MailCheckMessage error>
                There was a problem retrieving the {type} summary for this
                domain: {mx.error.message}
              </MailCheckMessage>
            )}
          </Grid.Column>
        </Grid.Row>
      </Grid>
    );
  }
}

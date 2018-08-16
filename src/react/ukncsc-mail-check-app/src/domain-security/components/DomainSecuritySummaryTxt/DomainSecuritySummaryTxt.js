import React, { Component } from 'react';
import { Grid } from 'semantic-ui-react';
import { Link } from 'react-router-dom';
import { MailCheckMessage } from 'common/components';
import {
  DomainSecurityRecord,
  DomainSecuritySummaryMessages,
  DomainSecurityTitle,
} from 'domain-security/components';
import { DomainSecurityLocationContext } from 'domain-security/context';

const shouldShowRecords = txt => txt && !!txt.records;

const shouldShowMessages = txt =>
  txt && !txt.pending && !txt.loading && !txt.error;

export default class DomainSecuritySummary extends Component {
  componentDidMount() {
    const {
      domainId,
      fetchDomainSecurityTxt,
      getDomainSecurityTxt,
    } = this.props;

    if (!getDomainSecurityTxt(domainId)) {
      fetchDomainSecurityTxt(domainId);
    }
  }

  render() {
    const { domainId, description, getDomainSecurityTxt, type } = this.props;
    const txt = getDomainSecurityTxt(domainId);

    return (
      <Grid stackable>
        <Grid.Row>
          <Grid.Column width="4">
            <DomainSecurityTitle as="h2" title={type} {...txt} />
          </Grid.Column>
          <Grid.Column width="8">
            <p>{description}</p>

            {shouldShowRecords(txt) &&
              txt.records.map(({ record }, i) => (
                <DomainSecurityRecord
                  type={type}
                  inheritedFrom={txt.inheritedFrom}
                  key={i}
                >
                  {record}
                </DomainSecurityRecord>
              ))}

            {shouldShowMessages(txt) && (
              <React.Fragment>
                <p>
                  <DomainSecurityLocationContext.Consumer>
                    {location => (
                      <Link
                        to={`/${location}/${domainId}/${type.toLowerCase()}`}
                      >
                        View more information
                      </Link>
                    )}
                  </DomainSecurityLocationContext.Consumer>
                </p>
                <DomainSecuritySummaryMessages
                  type={type}
                  failures={txt.failures && txt.failures.length}
                  warnings={txt.warnings && txt.warnings.length}
                  inconclusives={txt.inconclusives && txt.inconclusives.length}
                />
              </React.Fragment>
            )}

            {txt &&
              txt.pending && (
                <MailCheckMessage info>
                  {`We're currently checking your ${type} configuration. This usually takes less than 15 minutes, so check back soon.`}
                </MailCheckMessage>
              )}

            {txt &&
              txt.error && (
                <MailCheckMessage error>
                  There was a problem retrieving the {type} summary for this
                  domain: {txt.error.message}
                </MailCheckMessage>
              )}
          </Grid.Column>
        </Grid.Row>
      </Grid>
    );
  }
}

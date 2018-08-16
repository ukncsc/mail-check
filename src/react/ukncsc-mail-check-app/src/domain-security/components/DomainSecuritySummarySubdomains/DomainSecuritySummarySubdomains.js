import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Divider, Grid, Header, Icon } from 'semantic-ui-react';
import { MailCheckMessage } from 'common/components';
import { DomainSecurityTitle } from 'domain-security/components';

const shouldDisplayShowMore = ({ loading, subdomains, noMoreResults } = {}) =>
  !loading && subdomains && !!subdomains.length && !noMoreResults;

const shouldShowSubdomains = ({ subdomains } = {}) => !!subdomains;

const shouldShowNoSubdomainsMessage = ({ loading, error, noSubdomains } = {}) =>
  !loading && !error && noSubdomains;

export default class DomainSecuritySummarySubdomains extends Component {
  componentDidMount() {
    this.fetchSubdomains();
  }

  componentDidUpdate() {
    this.fetchSubdomains();
  }

  getSelection(...args) {
    if (args.some(_ => _ === 'Error')) {
      return <Icon name="exclamation circle" color="red" size="large" />;
    }

    if (args.some(_ => _ === 'Warning')) {
      return <Icon name="exclamation triangle" color="yellow" size="large" />;
    }

    return null;
  }

  fetchSubdomains() {
    const {
      domainName,
      fetchDomainSecuritySubdomains,
      getDomainSecuritySubdomains,
    } = this.props;

    if (domainName && !getDomainSecuritySubdomains(domainName)) {
      fetchDomainSecuritySubdomains(domainName);
    }
  }

  showMoreOnClick = e => {
    e.preventDefault();

    const {
      domainName,
      fetchDomainSecuritySubdomains,
      getDomainSecuritySubdomains,
    } = this.props;

    const { page, pageSize } = getDomainSecuritySubdomains(domainName);

    fetchDomainSecuritySubdomains(domainName, page + 1, pageSize);
  };

  render() {
    const data = this.props.getDomainSecuritySubdomains(this.props.domainName);

    return (
      <Grid stackable>
        <Grid.Row>
          <Grid.Column width="4">
            <DomainSecurityTitle
              as="h2"
              title="Subdomains"
              loading={data && data.loading}
            />
          </Grid.Column>
          <Grid.Column width="8">
            {shouldShowSubdomains(data) &&
              data.subdomains.map(
                ({ id, domainName, dmarcStatus, spfStatus, tlsStatus }) => (
                  <React.Fragment key={id}>
                    {this.getSelection(dmarcStatus, spfStatus, tlsStatus)}
                    <Header
                      as="h4"
                      style={{ display: 'inline-block', marginTop: 0 }}
                    >
                      {domainName}
                    </Header>
                    <p>
                      <Link to={`/my-domains/${id}`}>View information</Link>
                    </p>
                    <Divider />
                  </React.Fragment>
                )
              )}

            {shouldShowNoSubdomainsMessage(data) && (
              <MailCheckMessage info>
                {`We're currently not checking any subdomains for ${
                  this.props.domainName
                }.`}
              </MailCheckMessage>
            )}

            {shouldDisplayShowMore(data) && (
              <p>
                <a href="" onClick={this.showMoreOnClick}>
                  Show more
                </a>
              </p>
            )}

            {data &&
              data.error && (
                <MailCheckMessage error>{data.error.message}</MailCheckMessage>
              )}
          </Grid.Column>
        </Grid.Row>
      </Grid>
    );
  }
}

import React, { Component } from 'react';
import { Divider, Form, Header } from 'semantic-ui-react';
import WelcomeSearchMessages from '../WelcomeSearchMessages';

export default class Welcome extends Component {
  componentDidMount = () => this.props.resetWelcomeSearch();

  componentDidUpdate = () => {
    const { searchResult, lastSearchTerm, history } = this.props;

    if (searchResult && searchResult.domainName === lastSearchTerm) {
      history.push(`/domain-security/${this.props.searchResult.id}`);
    }
  };

  addDomainOnClick = e => {
    e.preventDefault();
    this.props.fetchAddDomainToMailCheck(this.props.searchTerm);
  };

  searchOnChange = (e, data) =>
    this.props.updateWelcomeSearch({ searchTerm: data.value });

  showResultsOnSubmit = () =>
    this.props.fetchWelcomeSearch(this.props.searchTerm);

  render() {
    const {
      error,
      hasSearched,
      lastSearchTerm,
      loading,
      searchResult,
      searchTerm,
      searchTermIsPublicSectorOrg,
    } = this.props;

    return (
      <React.Fragment>
        <Header as="h1" style={{ marginTop: 10 }}>
          Mail Check
        </Header>
        <Divider hidden />
        <p style={{ maxWidth: 600 }}>
          Mail Check helps you understand how secure your email server
          configuration is, and how to improve and maintain it. The tool checks
          DMARC, SPF, and TLS configuration, and processes DMARC reports to help
          you understand them.
        </p>
        <Header as="h4" style={{ display: 'inline-block' }}>
          Check your email domain
        </Header>
        <Form onSubmit={this.showResultsOnSubmit}>
          <Form.Input
            fluid
            onChange={this.searchOnChange}
            style={{ maxWidth: 600 }}
            type="text"
            value={searchTerm}
          />
          <Form.Button color="blue" disabled={!searchTerm} loading={loading}>
            Show results
          </Form.Button>
        </Form>
        <Divider hidden />
        <WelcomeSearchMessages
          addDomainOnClick={this.addDomainOnClick}
          error={error}
          hasSearched={hasSearched}
          lastSearchTerm={lastSearchTerm}
          loading={loading}
          searchResult={searchResult}
          searchTermIsPublicSectorOrg={searchTermIsPublicSectorOrg}
        />
      </React.Fragment>
    );
  }
}

import React from 'react';
import debounce from 'lodash/debounce';

import { connect } from 'react-redux';
import { Grid, Input, Loader, Header, Message } from 'semantic-ui-react';
import {
  Pagination,
  PaginationDisplay,
  DomainsStatusDisplay,
} from 'anti-spoofing/components';
import { fetchAntiSpoofingDomains } from 'anti-spoofing/store/domains';

export class AntiSpoofing extends React.Component {
  constructor(props) {
    super(props);
    this.fetchOnSearchChange = debounce(
      search =>
        this.props.fetchAntiSpoofingDomains(
          1,
          this.props.domain.pageSize,
          search
        ),
      300
    );
    this.fetchInitial = () => {
      this.props.fetchAntiSpoofingDomains(
        this.props.domain.page,
        this.props.domain.pageSize,
        this.props.domain.search
      );
    };
    this.fetchOnPageChange = page => {
      this.props.fetchAntiSpoofingDomains(
        page,
        this.props.domain.pageSize,
        this.props.domain.search
      );
    };
    this.handleSearchChanged = event =>
      this.fetchOnSearchChange(event.target.value);
  }
  componentWillMount() {
    this.fetchInitial();
  }
  render() {
    const { isLoading, error, page, pageSize, results } = this.props.domain;
    return (
      <Grid>
        <Grid.Row>
          <Grid.Column width={16}>
            <Header as="h1">Domain Security</Header>
          </Grid.Column>
        </Grid.Row>
        <Grid.Row>
          <Grid.Column width={16}>
            <Input
              fluid
              icon="search"
              type="text"
              placeholder="Search..."
              onChange={this.handleSearchChanged}
            />
          </Grid.Column>
        </Grid.Row>
        {isLoading && (
          <Grid.Row>
            <Grid.Column width={16}>
              <Loader active size="big" />
            </Grid.Column>
          </Grid.Row>
        )}
        {!isLoading &&
          error && (
            <Grid.Row>
              <Grid.Column width={16}>
                <Message negative>
                  <Message.Header>
                    The following error occurred with your request:
                  </Message.Header>
                  <p>{error}</p>
                </Message>
              </Grid.Column>
            </Grid.Row>
          )}
        {!isLoading &&
          !error &&
          results.domainCount === 0 && (
            <Grid.Row>
              <Grid.Column width={16}>
                <Message>
                  <Message.Header>No Results</Message.Header>
                  <p>
                    {results.search == null
                      ? 'There are currently no domains available in Mail Check.'
                      : 'Your search returned no results.'}
                  </p>
                </Message>
              </Grid.Column>
            </Grid.Row>
          )}
        {!isLoading &&
          !error &&
          results.domainCount > 0 && (
            <React.Fragment>
              <Grid.Row>
                <Grid.Column width={4}>
                  <PaginationDisplay
                    page={page}
                    pageSize={pageSize}
                    collectionSize={results.domainCount}
                  />
                </Grid.Column>
                <Grid.Column floated="right" width={12}>
                  <DomainsStatusDisplay results={results} />
                </Grid.Column>
              </Grid.Row>
              <Grid.Row>
                <Grid.Column floated="right" width={3}>
                  <Pagination
                    page={page}
                    pageSize={pageSize}
                    collectionSize={results.domainCount}
                    selectPage={this.fetchOnPageChange}
                  />
                </Grid.Column>
              </Grid.Row>
            </React.Fragment>
          )}
      </Grid>
    );
  }
}
const mapStateToProps = ({ domain }) => ({ domain });
const mapDispatchToProps = dispatch => ({
  fetchAntiSpoofingDomains: (page, pageSize, search) =>
    dispatch(fetchAntiSpoofingDomains(page, pageSize, search)),
});
export default connect(mapStateToProps, mapDispatchToProps)(AntiSpoofing);

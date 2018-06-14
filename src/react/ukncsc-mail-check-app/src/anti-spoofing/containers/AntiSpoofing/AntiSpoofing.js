import React from 'react';
import debounce from 'lodash/debounce';

import { connect } from 'react-redux';
import {
  Grid,
  Input,
  Loader,
  Header,
  Message,
  Divider,
  Statistic,
} from 'semantic-ui-react';
import { Pagination, DomainsStatusDisplay } from 'anti-spoofing/components';
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
      <React.Fragment>
        <Header as="h1">Domain Security</Header>
        <Divider hidden />
        <Input
          fluid
          icon="search"
          type="text"
          placeholder="Search..."
          onChange={this.handleSearchChanged}
        />
        <Divider hidden />
        {isLoading && <Loader active size="big" />}
        {!isLoading &&
          error && (
            <Message negative>
              <Message.Header>
                The following error occurred with your request:
              </Message.Header>
              <p>{error}</p>
            </Message>
          )}
        {!isLoading &&
          !error &&
          results.domainCount === 0 && (
            <Message>
              <Message.Header>No Results</Message.Header>
              <p>
                {results.search == null
                  ? 'There are currently no domains available in Mail Check.'
                  : 'Your search returned no results.'}
              </p>
            </Message>
          )}
        {!isLoading &&
          !error &&
          results.domainCount > 0 && (
            <Grid stackable>
              <Grid.Row>
                <Grid.Column width={2} floated="right">
                  <Statistic
                    label="Domains"
                    value={results.domainCount}
                    size="small"
                  />
                </Grid.Column>
                <Grid.Column width={10}>
                  <DomainsStatusDisplay results={results} />
                </Grid.Column>
              </Grid.Row>
              <Grid.Row>
                <Grid.Column floated="right">
                  <Pagination
                    page={page}
                    pageSize={pageSize}
                    collectionSize={results.domainCount}
                    selectPage={this.fetchOnPageChange}
                  />
                </Grid.Column>
              </Grid.Row>
            </Grid>
          )}
      </React.Fragment>
    );
  }
}
const mapStateToProps = ({ domain }) => ({ domain });
const mapDispatchToProps = dispatch => ({
  fetchAntiSpoofingDomains: (page, pageSize, search) =>
    dispatch(fetchAntiSpoofingDomains(page, pageSize, search)),
});
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AntiSpoofing);

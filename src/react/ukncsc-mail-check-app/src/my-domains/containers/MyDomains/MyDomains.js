import React from 'react';
import debounce from 'lodash/debounce';
import {
  Divider,
  Grid,
  Input,
  Loader,
  Header,
  Message,
  Statistic,
} from 'semantic-ui-react';
import { connect } from 'react-redux';
import { Pagination, DomainsStatusDisplay } from 'anti-spoofing/components';
import { fetchMyDomains } from 'my-domains/store/my-domains';
import { addMoreDomainsCopy, addDomainsCopy } from 'my-domains/data';

export class MyDomains extends React.Component {
  state = {
    userHasDomains: false,
  };

  fetchOnSearchChange = debounce(
    search =>
      this.props.fetchMyDomains(1, this.props.mydomains.pageSize, search),
    300
  );

  fetchInitial = () => {
    this.props.fetchMyDomains(
      this.props.mydomains.page,
      this.props.mydomains.pageSize,
      this.props.mydomains.search
    );
  };

  fetchOnPageChange = page => {
    this.props.fetchMyDomains(
      page,
      this.props.mydomains.pageSize,
      this.props.mydomains.search
    );
  };

  handleSearchChanged = event => this.fetchOnSearchChange(event.target.value);

  componentDidMount() {
    this.fetchInitial();
  }

  componentWillReceiveProps(nextProps) {
    const userHasDomains =
      nextProps.mydomains.results &&
      nextProps.mydomains.results.userDomainCount > 0;

    if (nextProps.mydomains.results) {
      this.setState({ userHasDomains });
    }
  }

  render() {
    const { isLoading, error, page, pageSize, results } = this.props.mydomains;

    const showLoading = isLoading;

    const showError = !isLoading && error;

    const success = !isLoading && !error && results;

    const showNoDomains = success && results.userDomainCount === 0;

    const showResults = success && results.domainCount > 0;

    const showNoResults =
      success && results.domainCount === 0 && results.userDomainCount !== 0;

    const showSearchBar = this.state.userHasDomains;

    const showAddMoreDomains = showSearchBar;

    return (
      <React.Fragment>
        <Header as="h1">My Domains</Header>
        {showAddMoreDomains && <p>{addMoreDomainsCopy}</p>}
        {showNoDomains && (
          <Message>
            <Message.Header>No Domains Assigned</Message.Header>
            <p>{addDomainsCopy}</p>
          </Message>
        )}
        {showSearchBar && (
          <Input
            fluid
            icon="search"
            type="text"
            placeholder="Search..."
            onChange={this.handleSearchChanged}
          />
        )}
        <Divider hidden />
        {showLoading && (
          <Loader
            active
            inline="centered"
            style={{ marginTop: '10px' }}
            size="big"
          />
        )}
        {showError && (
          <Message negative>
            <Message.Header>
              The following error occurred with your request:
            </Message.Header>
            <p>{error}</p>
          </Message>
        )}
        {showNoResults && (
          <Message>
            <Message.Header>No Results</Message.Header>
          </Message>
        )}
        {showResults && (
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
const mapStateToProps = ({ mydomains }) => ({ mydomains });

const mapDispatchToProps = dispatch => ({
  fetchMyDomains: (page, pageSize, search) =>
    dispatch(fetchMyDomains(page, pageSize, search)),
});
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(MyDomains);

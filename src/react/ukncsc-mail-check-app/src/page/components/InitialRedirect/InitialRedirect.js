import React, { Component } from 'react';
import { Loader } from 'semantic-ui-react';
import { Redirect } from 'react-router-dom';

export default class InitialRedirect extends Component {
  state = {
    fetched: false,
  };

  async componentDidMount() {
    const { fetchMyDomains, page, pageSize } = this.props;
    await fetchMyDomains(page, pageSize, '');

    // eslint-disable-next-line
    this.setState({ fetched: true });
  }

  render() {
    const { fetched } = this.state;
    const { error, isLoading, results } = this.props;

    return (
      <React.Fragment>
        <Loader active={isLoading} />
        {fetched &&
          !isLoading &&
          !error &&
          results &&
          (results.domainCount > 0 ? (
            <Redirect to="/my-domains" />
          ) : (
            <Redirect to="/home" />
          ))}
        {fetched && (error || !results) && <Redirect to="/home" />}
      </React.Fragment>
    );
  }
}

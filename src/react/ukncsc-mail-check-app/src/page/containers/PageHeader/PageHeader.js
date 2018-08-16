import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import { Container } from 'semantic-ui-react';
import {
  fetchCurrentUser,
  getCurrentUser,
  agreedToTermsCurrentUser,
} from 'common/store/reducers/current-user';
import { AgreeToTermsMessage, Banner, NavBar } from 'page/components';
import { companyLogo } from 'page/assets';
import { navLinks } from 'page/data';

import './PageHeader.css';

class PageHeader extends React.Component {
  componentWillMount() {
    this.props.fetchCurrentUser();
  }
  render() {
    const { currentUser, location, onTermsAgree } = this.props;

    return (
      <Container className="PageHeader" fluid style={{ background: 'white' }}>
        <AgreeToTermsMessage
          show={!currentUser.agreedToTerms}
          onAgree={onTermsAgree}
        />
        <Banner
          companyLogo={companyLogo}
          productName="Mail Check"
          currentUser={currentUser}
        />
        <NavBar
          routes={navLinks}
          userRole={currentUser.user && currentUser.user.roleType}
          location={location}
        />
      </Container>
    );
  }
}

const mapStateToProps = state => ({
  currentUser: getCurrentUser(state),
});

const mapDispatchToProps = dispatch => ({
  fetchCurrentUser: () => dispatch(fetchCurrentUser()),
  onTermsAgree: () => {
    localStorage.setItem('mailCheck/userHasAgreedToTerms', 'true');
    dispatch(agreedToTermsCurrentUser());
  },
});

export default withRouter(
  connect(
    mapStateToProps,
    mapDispatchToProps
  )(PageHeader)
);

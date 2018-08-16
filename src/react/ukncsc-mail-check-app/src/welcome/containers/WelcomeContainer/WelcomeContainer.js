import { Welcome } from 'welcome/components';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import {
  fetchAddDomainToMailCheck,
  fetchWelcomeSearch,
  getWelcomeSearch,
  resetWelcomeSearch,
  updateWelcomeSearch,
} from 'welcome/store';

const mapStateToProps = state => getWelcomeSearch(state);

const mapDispatchToProps = dispatch => ({
  fetchAddDomainToMailCheck: domainName =>
    dispatch(fetchAddDomainToMailCheck(domainName)),
  fetchWelcomeSearch: (searchTerm, page, pageSize) =>
    dispatch(fetchWelcomeSearch(searchTerm, page, pageSize)),
  resetWelcomeSearch: () => dispatch(resetWelcomeSearch()),
  updateWelcomeSearch: newState => dispatch(updateWelcomeSearch(newState)),
});

export default withRouter(
  connect(
    mapStateToProps,
    mapDispatchToProps
  )(Welcome)
);

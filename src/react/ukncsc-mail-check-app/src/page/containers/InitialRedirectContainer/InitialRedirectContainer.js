import { connect } from 'react-redux';
import { InitialRedirect } from 'page/components';
import { fetchMyDomains } from 'my-domains/store/my-domains';

const mapStateToProps = state => state.mydomains;

const mapDispatchToProps = dispatch => ({
  fetchMyDomains: (page, pageSize, search) =>
    dispatch(fetchMyDomains(page, pageSize, search)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(InitialRedirect);

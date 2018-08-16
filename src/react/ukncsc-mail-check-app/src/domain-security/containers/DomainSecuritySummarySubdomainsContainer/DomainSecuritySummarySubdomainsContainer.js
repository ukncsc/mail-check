import { connect } from 'react-redux';
import DomainSecuritySummarySubdomains from 'domain-security/components/DomainSecuritySummarySubdomains';
import {
  fetchDomainSecuritySubdomains,
  getDomainSecuritySubdomains,
} from 'domain-security/store';

const mapStateToProps = state => ({
  getDomainSecuritySubdomains: getDomainSecuritySubdomains(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecuritySubdomains: (domainName, page, pageSize) =>
    dispatch(fetchDomainSecuritySubdomains(domainName, page, pageSize)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecuritySummarySubdomains);

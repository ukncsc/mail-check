import { connect } from 'react-redux';
import { AggregateReportSummary } from 'domain-security/components';
import {
  fetchDomainSecurityAggregateData,
  getDomainSecurityAggregateData,
} from 'domain-security/store';

const mapStateToProps = state => ({
  getDomainSecurityAggregateData: getDomainSecurityAggregateData(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityAggregateData: (id, days, includeSubdomains) =>
    dispatch(fetchDomainSecurityAggregateData(id, days, includeSubdomains)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AggregateReportSummary);

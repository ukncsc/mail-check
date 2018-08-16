import React from 'react';
import { connect } from 'react-redux';
import { DomainSecuritySummary } from 'domain-security/components';
import { canViewAggregateData } from 'common/store';
import {
  fetchDomainSecurityDomain,
  getDomainSecurityDomain,
} from 'domain-security/store';
import { emailSecurityDescription } from 'domain-security/data';

const mapStateToProps = state => ({
  getDomainSecurityDomain: getDomainSecurityDomain(state),
  canViewAggregateData: canViewAggregateData(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityDomain: domainId =>
    dispatch(fetchDomainSecurityDomain(domainId)),
});

const DomainSecuritySummaryContainer = props => (
  <DomainSecuritySummary description={emailSecurityDescription} {...props} />
);

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecuritySummaryContainer);

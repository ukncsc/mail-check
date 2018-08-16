import React from 'react';
import { connect } from 'react-redux';
import { DomainSecuritySummaryMx } from 'domain-security/components';
import {
  fetchDomainSecurityDkim,
  getDomainSecurityDkim,
} from 'domain-security/store';
import { dkimDescription } from 'domain-security/data';

const mapStateToProps = state => ({
  getDomainSecurityMx: getDomainSecurityDkim(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityMx: domainName =>
    dispatch(fetchDomainSecurityDkim(domainName)),
});

const DomainSecuritySummaryDkimContainer = props => (
  <DomainSecuritySummaryMx
    description={dkimDescription}
    type="DKIM"
    {...props}
  />
);

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecuritySummaryDkimContainer);

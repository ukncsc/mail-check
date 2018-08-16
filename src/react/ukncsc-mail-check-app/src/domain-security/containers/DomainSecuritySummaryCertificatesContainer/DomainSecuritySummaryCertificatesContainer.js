import React from 'react';
import { connect } from 'react-redux';
import { DomainSecuritySummaryMx } from 'domain-security/components';
import { certsDescription } from 'domain-security/data';
import {
  fetchDomainSecurityCertificates,
  getDomainSecurityCertificates,
} from 'domain-security/store';

const mapStateToProps = state => ({
  getDomainSecurityMx: getDomainSecurityCertificates(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityMx: domainName =>
    dispatch(fetchDomainSecurityCertificates(domainName)),
});

const DomainSecuritySummaryCertificatesContainer = props => (
  <DomainSecuritySummaryMx
    description={certsDescription}
    type="TLS Certificates"
    {...props}
  />
);

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecuritySummaryCertificatesContainer);

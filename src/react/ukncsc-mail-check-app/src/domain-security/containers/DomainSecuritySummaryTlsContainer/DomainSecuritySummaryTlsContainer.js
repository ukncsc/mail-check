import React from 'react';
import { connect } from 'react-redux';
import { DomainSecuritySummaryMx } from 'domain-security/components';
import { tlsDescription } from 'domain-security/data';
import {
  fetchDomainSecurityTls,
  getDomainSecurityTls,
} from 'domain-security/store';

const mapStateToProps = state => ({
  getDomainSecurityMx: getDomainSecurityTls(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityMx: domainId => dispatch(fetchDomainSecurityTls(domainId)),
});

const DomainSecuritySummaryTlsContainer = props => (
  <DomainSecuritySummaryMx description={tlsDescription} type="TLS" {...props} />
);

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecuritySummaryTlsContainer);

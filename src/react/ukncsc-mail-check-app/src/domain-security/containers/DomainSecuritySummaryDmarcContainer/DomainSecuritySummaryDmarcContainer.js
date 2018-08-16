import React from 'react';
import { connect } from 'react-redux';
import { DomainSecuritySummaryTxt } from 'domain-security/components';
import { dmarcDescription } from 'domain-security/data';
import {
  fetchDomainSecurityDmarc,
  getDomainSecurityDmarc,
} from 'domain-security/store';

const mapStateToProps = state => ({
  getDomainSecurityTxt: getDomainSecurityDmarc(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityTxt: domainId =>
    dispatch(fetchDomainSecurityDmarc(domainId)),
});

const DomainSecuritySummaryDmarcContainer = props => (
  <DomainSecuritySummaryTxt
    description={dmarcDescription}
    type="DMARC"
    {...props}
  />
);

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecuritySummaryDmarcContainer);

import React from 'react';
import { connect } from 'react-redux';
import { DomainSecuritySummaryTxt } from 'domain-security/components';
import { spfDescription } from 'domain-security/data';
import {
  fetchDomainSecuritySpf,
  getDomainSecuritySpf,
} from 'domain-security/store';

const mapStateToProps = state => ({
  getDomainSecurityTxt: getDomainSecuritySpf(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityTxt: domainId =>
    dispatch(fetchDomainSecuritySpf(domainId)),
});

const DomainSecuritySummarySpfContainer = props => (
  <DomainSecuritySummaryTxt
    description={spfDescription}
    type="SPF"
    {...props}
  />
);

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecuritySummarySpfContainer);

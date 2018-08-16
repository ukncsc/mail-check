import React, { Component } from 'react';
import { connect } from 'react-redux';
import { DomainSecurityDetailsTxt } from 'domain-security/components';
import {
  fetchDomainSecurityDmarc,
  fetchDomainSecurityDomain,
  getDomainSecurityDmarc,
  getDomainSecurityDomain,
} from 'domain-security/store';

class DomainSecurityDetailsDmarc extends Component {
  state = {
    domain: {},
    dmarc: {},
  };

  static getDerivedStateFromProps = props => {
    const { domainId } = props.match.params;
    const dmarc = props.getDomainSecurityDmarc(domainId);
    const domain = props.getDomainSecurityDomain(domainId);

    if (!dmarc) {
      props.fetchDomainSecurityDmarc(domainId);
      return null;
    }

    if (!domain) {
      props.fetchDomainSecurityDomain(domainId);
      return null;
    }

    return { dmarc, domain };
  };

  render() {
    const { domainId } = this.props.match.params;
    const { dmarc, domain } = this.state;

    return (
      <DomainSecurityDetailsTxt
        type="DMARC"
        domainId={domainId}
        domainName={domain.name}
        {...dmarc}
      />
    );
  }
}

const mapStateToProps = state => ({
  getDomainSecurityDmarc: getDomainSecurityDmarc(state),
  getDomainSecurityDomain: getDomainSecurityDomain(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityDmarc: id => dispatch(fetchDomainSecurityDmarc(id)),
  fetchDomainSecurityDomain: id => dispatch(fetchDomainSecurityDomain(id)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecurityDetailsDmarc);

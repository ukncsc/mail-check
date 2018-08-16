import React, { Component } from 'react';
import { connect } from 'react-redux';
import { DomainSecurityDetailsTxt } from 'domain-security/components';
import {
  fetchDomainSecuritySpf,
  fetchDomainSecurityDomain,
  getDomainSecuritySpf,
  getDomainSecurityDomain,
} from 'domain-security/store';

class DomainSecurityDetailsSpf extends Component {
  state = {
    domain: {},
    spf: {},
  };

  static getDerivedStateFromProps = props => {
    const { domainId } = props.match.params;
    const spf = props.getDomainSecuritySpf(domainId);
    const domain = props.getDomainSecurityDomain(domainId);

    if (!spf) {
      props.fetchDomainSecuritySpf(domainId);
      return null;
    }

    if (!domain) {
      props.fetchDomainSecurityDomain(domainId);
      return null;
    }

    return { spf, domain };
  };

  render() {
    const { domainId } = this.props.match.params;
    const { spf, domain } = this.state;

    return (
      <DomainSecurityDetailsTxt
        type="SPF"
        domainId={domainId}
        domainName={domain.name}
        tagsProperty="terms"
        {...spf}
      />
    );
  }
}

const mapStateToProps = state => ({
  getDomainSecuritySpf: getDomainSecuritySpf(state),
  getDomainSecurityDomain: getDomainSecurityDomain(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecuritySpf: id => dispatch(fetchDomainSecuritySpf(id)),
  fetchDomainSecurityDomain: id => dispatch(fetchDomainSecurityDomain(id)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecurityDetailsSpf);

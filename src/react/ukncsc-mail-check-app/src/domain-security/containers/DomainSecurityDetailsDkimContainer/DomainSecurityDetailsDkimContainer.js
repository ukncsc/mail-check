import React, { Component } from 'react';
import { connect } from 'react-redux';
import {
  DomainSecurityDetailsMx,
  DomainSecurityRecord,
} from 'domain-security/components';
import {
  getDomainSecurityDkim,
  getDomainSecurityDomain,
  fetchDomainSecurityDkim,
  fetchDomainSecurityDomain,
} from 'domain-security/store';

class DomainSecurityDetailsDkimContainer extends Component {
  state = {
    dkim: null,
    domain: null,
  };

  static getDerivedStateFromProps = props => {
    const { domainId } = props.match.params;
    const domain = props.getDomainSecurityDomain(domainId);

    if (!domain) {
      props.fetchDomainSecurityDomain(domainId);
      return null;
    }

    if (domain.name) {
      const dkim = props.getDomainSecurityDkim(domain.name);

      if (!dkim) {
        props.fetchDomainSecurityDkim(domain.name);
        return null;
      }

      return { dkim, domain };
    }

    return null;
  };

  render() {
    const { dkim, domain } = this.state;

    return (
      <DomainSecurityDetailsMx
        type="DKIM"
        {...this.props.match.params}
        {...dkim}
        domainName={domain && domain.name}
      >
        {dkim &&
          dkim.records &&
          dkim.records
            .filter(
              record => record.hostname === this.props.match.params.hostname
            )
            .map(
              record =>
                record.records &&
                record.records.map(_ => (
                  <DomainSecurityRecord type="DKIM">
                    {_.record}
                  </DomainSecurityRecord>
                ))
            )}
      </DomainSecurityDetailsMx>
    );
  }
}

const mapStateToProps = state => ({
  getDomainSecurityDkim: getDomainSecurityDkim(state),
  getDomainSecurityDomain: getDomainSecurityDomain(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityDkim: domainName =>
    dispatch(fetchDomainSecurityDkim(domainName)),
  fetchDomainSecurityDomain: id => dispatch(fetchDomainSecurityDomain(id)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecurityDetailsDkimContainer);

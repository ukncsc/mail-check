import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { DomainSecurityDetailsMx } from 'domain-security/components';
import {
  getDomainSecurityTls,
  fetchDomainSecurityTls,
} from 'domain-security/store';

class DomainSecurityDetailsTls extends Component {
  state = {
    tls: null,
  };

  static getDerivedStateFromProps = props => {
    const { domainId } = props.match.params;
    const tls = props.getDomainSecurityTls(domainId);

    if (!tls) {
      props.fetchDomainSecurityTls(domainId);
      return null;
    }

    return { tls };
  };

  render() {
    return (
      <DomainSecurityDetailsMx
        type="TLS"
        {...this.props.match.params}
        {...this.state.tls}
      >
        <p>
          <Link to="/domain-security/tls-advice">View NCSC advice on TLS</Link>
        </p>
      </DomainSecurityDetailsMx>
    );
  }
}

const mapStateToProps = state => ({
  getDomainSecurityTls: getDomainSecurityTls(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityTls: id => dispatch(fetchDomainSecurityTls(id)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecurityDetailsTls);

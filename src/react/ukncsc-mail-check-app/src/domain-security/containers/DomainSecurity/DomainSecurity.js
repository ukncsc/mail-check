import { Component } from 'react';
import { connect } from 'react-redux';
import { withRouter } from 'react-router-dom';
import isEmpty from 'lodash/isEmpty';
import {
  fetchDomainSecurityDomain,
  fetchDomainSecurityDmarc,
  fetchDomainSecuritySpf,
  fetchDomainSecurityTls,
  fetchAggregateReportInfo,
  getDomainSecurityDmarc,
  getDomainSecurityDomain,
  getDomainSecuritySpf,
  getDomainSecurityTls,
  getAggregateReportInfo,
} from 'domain-security/store';

import { canViewAggregateData } from 'common/store/reducers/current-user';

class DomainSecurity extends Component {
  componentWillMount = () => {
    const { domainId } = this.props.match.params;
    const domain = this.props.getDomainSecurityDomain(domainId);

    if (isEmpty(domain)) {
      this.props.fetchDomainSecurity(domainId);
    }
  };

  render() {
    const { domainId } = this.props.match.params;

    return this.props.render({
      ...this.props,
      dmarc: this.props.getDomainSecurityDmarc(domainId),
      domain: this.props.getDomainSecurityDomain(domainId),
      spf: this.props.getDomainSecuritySpf(domainId),
      tls: this.props.getDomainSecurityTls(domainId),
      aggregateReportInfo: this.props.getAggregateReportInfo(domainId),
      canViewAggregateData: this.props.canViewAggregateData(domainId),
    });
  }
}

const mapStateToProps = state => ({
  getDomainSecurityDmarc: getDomainSecurityDmarc(state),
  getDomainSecurityDomain: getDomainSecurityDomain(state),
  getDomainSecuritySpf: getDomainSecuritySpf(state),
  getDomainSecurityTls: getDomainSecurityTls(state),
  getAggregateReportInfo: getAggregateReportInfo(state),
  canViewAggregateData: canViewAggregateData(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurity: id => {
    dispatch(fetchDomainSecurityDomain(id));
    dispatch(fetchDomainSecurityDmarc(id));
    dispatch(fetchDomainSecuritySpf(id));
    dispatch(fetchDomainSecurityTls(id));
    dispatch(fetchAggregateReportInfo(id));
  },
});

export default connect(mapStateToProps, mapDispatchToProps)(
  withRouter(DomainSecurity)
);

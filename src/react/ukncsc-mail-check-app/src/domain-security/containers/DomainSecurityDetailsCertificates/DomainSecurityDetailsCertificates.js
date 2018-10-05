import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Divider, Header, Table } from 'semantic-ui-react';
import startCase from 'lodash/startCase';
import { DomainSecurityDetailsMx } from 'domain-security/components';
import { ShowMoreDropdown } from 'common/components';
import {
  getDomainSecurityCertificates,
  getDomainSecurityDomain,
  fetchDomainSecurityCertificates,
  fetchDomainSecurityDomain,
} from 'domain-security/store';

class DomainSecurityDetailsCertificates extends Component {
  state = {
    certificates: null,
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
      const certificates = props.getDomainSecurityCertificates(domain.name);

      if (!certificates) {
        props.fetchDomainSecurityCertificates(domain.name);
        return null;
      }

      return { certificates, domain };
    }

    return null;
  };

  render() {
    const { certificates, domain } = this.state;
    const record =
      certificates &&
      certificates.records &&
      certificates.records.find(
        r => r.hostname === this.props.match.params.hostname
      );
    return (
      <DomainSecurityDetailsMx
        type="TLS Certificate"
        {...this.props.match.params}
        {...certificates}
        domainName={domain && domain.name}
      >
        <ShowMoreDropdown title="Certificate chain details">
          {record ? (
            record.certs.map(cert => (
              <React.Fragment>
                <Header as="h3">{cert.subject}</Header>
                <Table>
                  <Table.Body>
                    {Object.keys(cert).map(key => (
                      <Table.Row key={key}>
                        <Table.Cell>
                          <strong>{startCase(key)}</strong>
                        </Table.Cell>
                        <Table.Cell>{cert[key]}</Table.Cell>
                      </Table.Row>
                    ))}
                  </Table.Body>
                </Table>
              </React.Fragment>
            ))
          ) : (
            <p>Nothing to show</p>
          )}
        </ShowMoreDropdown>
        <Divider hidden />
      </DomainSecurityDetailsMx>
    );
  }
}

const mapStateToProps = state => ({
  getDomainSecurityCertificates: getDomainSecurityCertificates(state),
  getDomainSecurityDomain: getDomainSecurityDomain(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityCertificates: domainName =>
    dispatch(fetchDomainSecurityCertificates(domainName)),
  fetchDomainSecurityDomain: id => dispatch(fetchDomainSecurityDomain(id)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecurityDetailsCertificates);

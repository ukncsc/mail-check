import React from 'react';
import { Link } from 'react-router-dom';
import { Table, Header } from 'semantic-ui-react';
import { DomainSecurityLocationContext } from 'domain-security/context';
import RatingDisplay from '../RatingDisplay';

export default ({ results }) => (
  <Table basic="very">
    <Table.Header>
      <Table.Row>
        <Table.HeaderCell width={6}>
          <Header as="h3">Domain</Header>
        </Table.HeaderCell>
        <Table.HeaderCell width={2}>
          <Header as="h3">DMARC</Header>
        </Table.HeaderCell>
        <Table.HeaderCell width={2}>
          <Header as="h3">SPF</Header>
        </Table.HeaderCell>
        <Table.HeaderCell width={2}>
          <Header as="h3">TLS</Header>
        </Table.HeaderCell>
      </Table.Row>
    </Table.Header>

    <Table.Body>
      {results &&
        results.domainSecurityInfos.map(
          ({ domain, tlsStatus, dmarcStatus, spfStatus }, i) => (
            <Table.Row key={i}>
              <Table.Cell>
                <Header as="h4">{domain.name}</Header>
                {
                  // waiting on https://github.com/airbnb/javascript/pull/1648 to fix below
                  /* eslint-disable jsx-a11y/anchor-is-valid */
                }

                <DomainSecurityLocationContext.Consumer>
                  {location => (
                    <Link to={`/${location}/${domain.id}`}>
                      View Information
                    </Link>
                  )}
                </DomainSecurityLocationContext.Consumer>
              </Table.Cell>
              <Table.Cell>
                <RatingDisplay status={dmarcStatus} />
              </Table.Cell>
              <Table.Cell>
                <RatingDisplay status={spfStatus} />
              </Table.Cell>
              <Table.Cell>
                <RatingDisplay status={tlsStatus} />
              </Table.Cell>
            </Table.Row>
          )
        )}
    </Table.Body>
  </Table>
);

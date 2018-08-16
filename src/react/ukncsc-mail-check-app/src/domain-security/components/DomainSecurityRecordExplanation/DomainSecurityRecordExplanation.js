import React from 'react';
import { Header, Table } from 'semantic-ui-react';

import './DomainSecurityRecordExplanation.css';

const DomainSecurityRecordExplanation = ({ title, data }) =>
  !!data.length && (
    <React.Fragment>
      <Header as="h3">{title}</Header>
      <Table className="DomainSecurityRecordExplanation--table">
        <Table.Header>
          <Table.Row>
            <Table.HeaderCell width={3}>Element</Table.HeaderCell>
            <Table.HeaderCell width={9}>Explanation</Table.HeaderCell>
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {data.map(({ value, explanation }) => (
            <Table.Row key={value}>
              <Table.Cell>{value}</Table.Cell>
              <Table.Cell>{explanation}</Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table>
    </React.Fragment>
  );

export default DomainSecurityRecordExplanation;

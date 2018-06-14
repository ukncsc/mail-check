import React, { Component } from 'react';
import { SingleDatePicker } from 'react-dates';
import map from 'lodash/map';
import replace from 'lodash/replace';
import moment from 'moment';
import { Button, Divider, Header, Message, Table } from 'semantic-ui-react';
import { BackLink, ShowMoreDropdown } from 'common/components';
import { mailCheckApiDownload } from 'common/helpers';

import { aggregateReportExportFields as fields } from 'domain-security/data';

export default class DomainSecurityAggregateExport extends Component {
  state = {
    focused: null,
    date: moment().subtract(1, 'day'),
    loading: false,
    error: null,
  };

  onDownloadCsv = async () => {
    this.setState({ loading: true, error: null });

    const formattedDate = this.state.date.format('YYYY-MM-DD');
    const { id, name } = this.props.domain;

    try {
      await mailCheckApiDownload(
        `/domainstatus/domain/aggregate-export/${id}/${formattedDate}`,
        `AggregateReport_${formattedDate}_${replace(name, /\./g, '-')}.csv`
      );
    } catch (error) {
      this.setState({ error });
    } finally {
      this.setState({ loading: false });
    }
  };

  onDateChange = date => this.setState({ date });

  onFocusChange = ({ focused }) => this.setState({ focused });

  render() {
    const now = moment();

    return (
      <React.Fragment>
        <BackLink />
        <Header as="h1">Export aggregate report data</Header>
        <Header as="h2">{this.props.domain.name}</Header>    
        <ShowMoreDropdown title="Explain report information">
          <Table>
            <Table.Body>
              {map(fields, _ => (
                <Table.Row key={_.title}>
                  <Table.Cell>
                    <strong>{_.title}</strong>
                  </Table.Cell>
                  <Table.Cell>{_.description}</Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
          </Table>
        </ShowMoreDropdown>
        <Divider hidden />
        <SingleDatePicker
          showDefaultInputIcon
          keepFocusOnInput
          focused={this.state.focused}
          date={this.state.date}
          onDateChange={this.onDateChange}
          onFocusChange={this.onFocusChange}
          displayFormat="DD/MM/YYYY"
          isOutsideRange={date => date.isAfter(now)}
        />
        <Divider hidden />
        <Button
          primary
          size="large"
          loading={this.state.loading}
          disabled={this.state.loading || !this.state.date}
          onClick={this.onDownloadCsv}
        >
          Download CSV
        </Button>
        {this.state.error && (
          <React.Fragment>
            <Divider hidden />
            <Message error onDismiss={() => this.setState({ error: null })}>
              {this.state.error.message}
            </Message>
          </React.Fragment>
        )}
        <Divider hidden />
        <p>
          <strong>Note:</strong> DMARC Reporting often takes a full day to be
          sent by email providers, so a report for a recent date may change if
          you re-download at a future date.
        </p>
      </React.Fragment>
    );
  }
}

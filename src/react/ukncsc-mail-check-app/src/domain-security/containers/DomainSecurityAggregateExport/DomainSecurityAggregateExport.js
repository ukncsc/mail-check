import React, { Component } from 'react';
import { connect } from 'react-redux';
import { SingleDatePicker } from 'react-dates';
import replace from 'lodash/replace';
import moment from 'moment';
import { Button, Divider, Header, Message, Table } from 'semantic-ui-react';
import { BackLink, ShowMoreDropdown } from 'common/components';
import { mailCheckApiDownload } from 'common/helpers';
import {
  fetchDomainSecurityDomain,
  getDomainSecurityDomain,
} from 'domain-security/store';

import { aggregateReportExportFields as fields } from 'domain-security/data';

class DomainSecurityAggregateExport extends Component {
  state = {
    domain: {},
    focused: null,
    date: moment().subtract(1, 'day'),
    loading: false,
    error: null,
  };

  static getDerivedStateFromProps = (props, state) => ({
    ...state,
    domain: props.getDomainSecurityDomain(props.match.params.domainId) || {},
  });

  componentDidMount() {
    if (!this.props.getDomainSecurityDomain(this.props.match.params.domainId)) {
      this.props.fetchDomainSecurityDomain(this.props.match.params.domainId);
    }
  }

  onDownloadCsv = async () => {
    this.setState({ loading: true, error: null });

    const formattedDate = this.state.date.format('YYYY-MM-DD');
    const { id, name } = this.state.domain;

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
        <Header as="h1">Download aggregate report data as CSV</Header>
        <Header as="h2">{this.state.domain.name || null}</Header>
        <ShowMoreDropdown title="Explain report information">
          <Table>
            <Table.Body>
              {fields.map(({ title, description }) => (
                <Table.Row key={title}>
                  <Table.Cell>
                    <strong>{title}</strong>
                  </Table.Cell>
                  <Table.Cell>{description}</Table.Cell>
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

const mapStateToProps = state => ({
  getDomainSecurityDomain: getDomainSecurityDomain(state),
});

const mapDispatchToProps = dispatch => ({
  fetchDomainSecurityDomain: id => dispatch(fetchDomainSecurityDomain(id)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(DomainSecurityAggregateExport);

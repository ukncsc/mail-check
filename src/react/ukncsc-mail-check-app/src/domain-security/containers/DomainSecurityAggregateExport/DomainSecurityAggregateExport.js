import React, { Component } from 'react';
import { connect } from 'react-redux';
import { DateRangePicker } from 'react-dates';
import replace from 'lodash/replace';
import moment from 'moment';
import { Button, Divider, Header, Message, Table } from 'semantic-ui-react';
import {
  Breadcrumb,
  BreadcrumbItem,
  ShowMoreDropdown,
} from 'common/components';
import { mailCheckApiDownload } from 'common/helpers';
import {
  fetchDomainSecurityDomain,
  getDomainSecurityDomain,
} from 'domain-security/store';
import { DomainSecurityLocationContext } from 'domain-security/context';

import { aggregateReportExportFields as fields } from 'domain-security/data';

class DomainSecurityAggregateExport extends Component {
  state = {
    domain: {},
    focusedInput: null,
    startDate: moment().subtract(1, 'month'),
    endDate: moment(),
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

    const startDateFormatted = this.state.startDate.format('YYYY-MM-DD');
    const endDateFormatted = this.state.endDate.format('YYYY-MM-DD');
    const { id, name } = this.state.domain;
    const domain = replace(name, /\./g, '-');

    try {
      await mailCheckApiDownload(
        `/domainstatus/domain/aggregate-export/${id}/${startDateFormatted}/${endDateFormatted}`,
        `AggregateReport_${startDateFormatted}_${endDateFormatted}_${domain}.csv`
      );
    } catch (error) {
      this.setState({ error });
    } finally {
      this.setState({ loading: false });
    }
  };

  onDatesChange = ({ startDate, endDate }) => {
    this.setState({
      startDate,
      endDate:
        endDate && endDate.diff(startDate, 'months', true) > 1
          ? startDate.clone().add(1, 'month')
          : endDate,
    });
  };

  onFocusChange = focusedInput => this.setState({ focusedInput });

  isOutsideRange = date => {
    if (this.state.focusedInput === 'endDate') {
      return (
        !!this.state.startDate &&
        date.isAfter(this.state.startDate.clone().add(1, 'month'))
      );
    }

    if (date.isAfter(new Date())) {
      return true;
    }

    return false;
  };

  render() {
    return (
      <React.Fragment>
        <Breadcrumb>
          <DomainSecurityLocationContext.Consumer>
            {location => (
              <BreadcrumbItem link={`/${location}/${this.state.domain.id}`}>
                {this.state.domain.name}
              </BreadcrumbItem>
            )}
          </DomainSecurityLocationContext.Consumer>
          <BreadcrumbItem active>Aggregate Report Export</BreadcrumbItem>
        </Breadcrumb>
        <Header as="h1">Download aggregate report data as CSV</Header>
        <Header as="h2">{this.state.domain.name || null}</Header>
        <p>
          You can download up to a month of aggregate report data at a time.
          Downloads may take up to a minute to complete.
        </p>
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
        <DateRangePicker
          startDate={this.state.startDate}
          startDateId="AggregateExportStartDate"
          endDate={this.state.endDate}
          endDateId="AggregateExportEndDate"
          showDefaultInputIcon
          keepFocusOnInput
          minimumNights={0}
          onDatesChange={this.onDatesChange}
          focusedInput={this.state.focusedInput}
          onFocusChange={this.onFocusChange}
          displayFormat="D/M/YYYY"
          isOutsideRange={this.isOutsideRange}
        />
        <Divider hidden />
        <Button
          primary
          size="large"
          loading={this.state.loading}
          disabled={
            this.state.loading || !this.state.startDate || !this.state.endDate
          }
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

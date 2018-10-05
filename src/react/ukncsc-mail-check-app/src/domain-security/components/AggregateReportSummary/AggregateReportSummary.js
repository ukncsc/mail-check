import React, { Component } from 'react';
import startsWith from 'lodash/startsWith';
import numeral from 'numeral';
import { Divider, Header, Grid, Loader, Statistic } from 'semantic-ui-react';
import { MailCheckMessage } from 'common/components';
import {
  AggregateReportChart,
  AggregateReportChartLegend,
  AggregateReportDateSelector,
} from 'domain-security/components';
import { graphDescriptions } from 'domain-security/data';

import './AggregateReportSummary.css';

const showNoData = ({ loading, error, data }) => !loading && !error && !data;

const showDontHavePermission = ({ error }) =>
  error && startsWith(error.message, '403');

const showError = ({ error }) => error && !startsWith(error.message, '403');

const showChart = ({ loading, error, data }) => !loading && !error && data;

export default class AggregateReportSummary extends Component {
  componentDidMount() {
    const {
      domainId,
      fetchDomainSecurityAggregateData,
      getDomainSecurityAggregateData,
    } = this.props;

    if (!getDomainSecurityAggregateData(domainId)) {
      fetchDomainSecurityAggregateData(domainId);
    }
  }

  render() {
    const {
      domainId,
      fetchDomainSecurityAggregateData,
      getDomainSecurityAggregateData,
    } = this.props;

    const aggregateData = getDomainSecurityAggregateData(domainId) || {};

    return (
      <React.Fragment>
        {!showDontHavePermission(aggregateData) && (
          <AggregateReportDateSelector
            className="AggregateReportSummary--date-range"
            fetchDomainSecurityAggregateData={fetchDomainSecurityAggregateData}
            id={aggregateData.id}
            selectedDays={aggregateData.selectedDays}
            includeSubdomains={aggregateData.includeSubdomains}
          />
        )}

        {showNoData(aggregateData) && (
          <MailCheckMessage info fluid>
            Currently no aggregate report information to show for the selected
            date range.
          </MailCheckMessage>
        )}

        {showDontHavePermission(aggregateData) && (
          <MailCheckMessage info fluid>
            You do not have permission to view aggregate reporting for this domain.
          </MailCheckMessage>
        )}

        {showError(aggregateData) && (
          <MailCheckMessage error fluid>
            There was a problem retrieving aggregate report information for this
            domain: {aggregateData.error.message}
          </MailCheckMessage>
        )}

        {aggregateData.loading && (
          <div style={{ margin: 22 }}>
            <Loader active inline="centered" />
          </div>
        )}

        {showChart(aggregateData) && (
          <React.Fragment>
            <div>
              <Statistic size="small">
                <Statistic.Label style={{ fontWeight: 400 }}>
                  Total emails:
                </Statistic.Label>
                <Statistic.Value style={{ fontWeight: 500 }}>
                  {numeral(aggregateData.data.totalEmail).format('0.0a')}
                </Statistic.Value>
              </Statistic>
            </div>
            <Grid stackable>
              <Grid.Row>
                <Grid.Column width={10} style={{ padding: '0px' }}>
                  <AggregateReportChart
                    data={aggregateData.data.results}
                    descriptions={[...graphDescriptions].reverse()}
                  />
                </Grid.Column>
                <Grid.Column
                  width={2}
                  verticalAlign="bottom"
                  style={{ padding: '0px' }}
                >
                  <AggregateReportChartLegend>
                    {graphDescriptions.map(
                      ({ title: name, color: background, stroke }) => ({
                        name,
                        background,
                        stroke,
                      })
                    )}
                  </AggregateReportChartLegend>
                </Grid.Column>
              </Grid.Row>
              <Grid.Row columns={graphDescriptions.length}>
                {graphDescriptions.map(({ title, description }) => (
                  <Grid.Column key={title}>
                    <Header as="h5">{title}</Header>
                    <p>{description}</p>
                  </Grid.Column>
                ))}
              </Grid.Row>
            </Grid>
          </React.Fragment>
        )}
        <Divider hidden />
      </React.Fragment>
    );
  }
}

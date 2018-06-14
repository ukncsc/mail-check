import React from 'react';
import startsWith from 'lodash/startsWith';
import numeral from 'numeral';
import { Divider, Header, Grid, Message, Statistic } from 'semantic-ui-react';
import {
  AggregateReportChart,
  AggregateReportChartLegend,
} from 'domain-security/components';
import { graphDescriptions } from 'domain-security/data';

import './AggregateReportSummary.css';

const showNoData = ({ loading, error, data }) => !loading && !error && !data;

const showDontHavePermission = ({ error }) => error && startsWith(error, '403');

const showError = ({ error }) => error && !startsWith(error, '403');

const showChart = ({ loading, error, data }) => !loading && !error && data;

export default ({ data, ...props }) => (
  <React.Fragment>
    {showNoData({ data, ...props }) && (
      <Message info>
        Currently no aggregate report information to show for domain.
      </Message>
    )}

    {showDontHavePermission(props) && (
      <Message info>
        You do have permission to view aggregate reporting for this domain.
      </Message>
    )}

    {showError(props) && (
      <Message error>
        There was a problem retrieving aggregate report information for this
        domain: {props.error}
      </Message>
    )}

    {showChart({ data, ...props }) && (
      <React.Fragment>
        <span className="AggregateReportSummary--date-range">Last 7 days</span>
        <div>
          <Statistic size="small">
            <Statistic.Label style={{ fontWeight: 400 }}>
              Total emails:
            </Statistic.Label>
            <Statistic.Value style={{ fontWeight: 500 }}>
              {numeral(data.totalEmail).format('0.0a')}
            </Statistic.Value>
          </Statistic>
        </div>
        <Grid stackable>
          <Grid.Row>
            <Grid.Column width={10} style={{ padding: '0px' }}>
              <AggregateReportChart
                data={data.results}
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

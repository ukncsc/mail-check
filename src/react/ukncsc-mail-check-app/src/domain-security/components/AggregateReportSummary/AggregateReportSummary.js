import React from 'react';
import startsWith from 'lodash/startsWith';
import values from 'lodash/values';
import { Grid, Header, Loader, Message } from 'semantic-ui-react';
import { items } from 'domain-security/data';
import {
  AggregateReportChart,
  AggregateReportLegendExplanation,
  AggregateReportStatistic,
} from 'domain-security/components';

export default ({ aggregateReportInfo }) => {
  const items1 = values(items);

  const showLoader = aggregateReportInfo.loading;

  const showNoData =
    !aggregateReportInfo.loading &&
    !aggregateReportInfo.error &&
    !aggregateReportInfo.data;

  const showDontHavePermission =
    aggregateReportInfo.error && startsWith(aggregateReportInfo.error, '403');

  const showError =
    aggregateReportInfo.error && !startsWith(aggregateReportInfo.error, '403');

  const showChart =
    !aggregateReportInfo.loading &&
    !aggregateReportInfo.error &&
    aggregateReportInfo.data;

  return (
    <React.Fragment>
      {showLoader && (
        <Header as="h1">
          <Loader active inline style={{ marginRight: 20 }} />
        </Header>
      )}

      {showNoData && (
        <Message info>
          Currently no aggregate report information to show for domain.
        </Message>
      )}

      {showDontHavePermission && (
        <Message info>
          You do have permission to view aggregate reporting for this domain.
        </Message>
      )}

      {showError && (
        <Message error>
          There was a problem retrieving aggregate report information for this
          domain: {aggregateReportInfo.error}
        </Message>
      )}

      {showChart && (
        <React.Fragment>
          <AggregateReportStatistic
            title="Total emails"
            value={aggregateReportInfo.data.totalEmail}
            period="last 7 days"
          />
          <AggregateReportChart data={aggregateReportInfo.data.results} />
          {items1.length > 0 && (
            <Grid stackable>
              <Grid.Row columns={items1.length}>
                {items1.map(_ => (
                  <Grid.Column key={_.title}>
                    <AggregateReportLegendExplanation {..._} />
                  </Grid.Column>
                ))}
              </Grid.Row>
            </Grid>
          )}
        </React.Fragment>
      )}
    </React.Fragment>
  );
};

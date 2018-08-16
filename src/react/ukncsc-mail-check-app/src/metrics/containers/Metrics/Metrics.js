import React from 'react';
import { connect } from 'react-redux';
import moment from 'moment';
import numeral from 'numeral';
import { Loader, Message } from 'semantic-ui-react';
import {
  MetricsDateRange,
  MetricsChart,
  MetricsHeadlineStats,
} from 'metrics/components';
import { getMetrics, fetchMetrics } from 'metrics/store';

import './Metrics.css';

class Metrics extends React.Component {
  state = {
    startDate: moment().subtract(1, 'year'),
    endDate: moment(),
    focusedInput: null,
    dataPoints: [],
    latest: {},
  };

  componentDidMount() {
    this.loadMetrics();
  }

  static getDerivedStateFromProps(props, state) {
    if (props.metrics && props.metrics.data) {
      const { data } = props.metrics;
      const dataPoints = Object.keys(data).sort((a, b) =>
        moment(a).diff(moment(b))
      );

      return {
        ...state,
        dataPoints,
        latest: data[dataPoints[dataPoints.length - 1]] || {},
      };
    }

    return null;
  }

  setDateRange = (startDate, endDate) =>
    this.setState({ startDate, endDate }, this.loadMetrics);

  setFocusedInput = focusedInput => this.setState({ focusedInput });

  sumDataValues = property => {
    const sumDataValuesReducer = (accumulator, point) =>
      this.props.metrics.data[point][property] + accumulator;

    const sum = this.state.dataPoints.reduce(sumDataValuesReducer, 0);

    return numeral(sum).format('0.0a');
  };

  loadMetrics = () => {
    const { startDate, endDate } = this.state;

    if (startDate && endDate) {
      this.props.fetchMetrics(
        startDate.format('YYYY-MM-DD'),
        endDate.format('YYYY-MM-DD')
      );
    }
  };

  render() {
    const { dataPoints, latest } = this.state;
    const { data = {}, isLoading, error } = this.props.metrics;
    return (
      <div>
        <h1>Metrics</h1>
        <MetricsDateRange
          startDate={this.state.startDate}
          endDate={this.state.endDate}
          focusedInput={this.state.focusedInput}
          setDateRange={this.setDateRange}
          setFocusedInput={this.setFocusedInput}
        />
        {isLoading && (
          <div style={{ textAlign: 'center' }}>
            <Loader active inline size="massive" style={{ marginTop: 20 }} />
          </div>
        )}
        {!isLoading &&
          error && (
            <Message negative>
              <Message.Header>An error has occurred</Message.Header>
              <p>{error}</p>
            </Message>
          )}
        {!isLoading &&
          !error &&
          Object.keys(data).length === 0 && (
            <Message warning>
              <Message.Header>Nothing to Show</Message.Header>
              <p>
                There is no data to show for the date range that you are
                interested in at the moment. Check back soon!
              </p>
            </Message>
          )}
        {!isLoading &&
          !error &&
          Object.keys(data).length > 0 && (
            <React.Fragment>
              <h2>DMARC</h2>
              <MetricsHeadlineStats
                values={{
                  'Domains with DMARC': latest.dmarcAny,
                  'Domains in Monitor Mode': latest.dmarcMonitor,
                  'Domains in Active Mode': latest.dmarcActive,
                }}
              />
              <MetricsChart
                data={data}
                properties={['', 'dmarcMonitor', 'dmarcActive']}
                dataPoints={dataPoints}
                stacked
              />
              <h2>Domains Reporting</h2>
              <MetricsHeadlineStats
                values={{
                  'Domains Reporting Aggregate':
                    latest.domainsAggregateReporting,
                }}
              />
              <MetricsChart
                data={data}
                properties={['domainsAggregateReporting']}
                dataPoints={dataPoints}
              />
              <h2>Reports Received</h2>
              <MetricsHeadlineStats
                values={{
                  'Aggregate Reports Received': this.sumDataValues(
                    'aggregateReportsReceived'
                  ),
                }}
                size="large"
              />
              <MetricsChart
                data={data}
                dataPoints={dataPoints}
                properties={['aggregateReportsReceived']}
              />
              <h2>Mail Check Usage</h2>
              <MetricsHeadlineStats
                values={{
                  'Domains Registered': latest.domainsRegistered,
                  'Users Registered': latest.usersRegistered,
                }}
              />
              <MetricsChart
                data={data}
                properties={['domainsRegistered', 'usersRegistered']}
                dataPoints={this.state.dataPoints}
                filled
              />
              <h2>Reporting URI(s)</h2>
              <MetricsHeadlineStats
                values={{
                  'RUA Configured for Mail Check':
                    latest.ruaConfiguredForMailCheck,
                }}
              />
              <MetricsChart
                data={data}
                properties={['ruaConfiguredForMailCheck']}
                dataPoints={this.state.dataPoints}
              />
              <h2>Mail Check Impact</h2>
              <MetricsHeadlineStats
                values={{
                  'Emails Blocked': this.sumDataValues('emailsBlocked'),
                }}
              />
              <MetricsChart
                data={data}
                properties={['emailsBlocked']}
                dataPoints={this.state.dataPoints}
              />
            </React.Fragment>
          )}
      </div>
    );
  }
}
const mapStateToProps = state => ({ metrics: getMetrics(state) });

const mapDispatchToProps = dispatch => ({
  fetchMetrics: (start, end) => dispatch(fetchMetrics(start, end)),
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Metrics);

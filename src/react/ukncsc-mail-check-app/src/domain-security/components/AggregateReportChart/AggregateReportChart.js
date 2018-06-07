import React, { Component } from 'react';
import moment from 'moment';
import reduce from 'lodash/reduce';
import numeral from 'numeral';
import { Grid } from 'semantic-ui-react';
import {
  VictoryArea,
  VictoryAxis,
  VictoryStack,
  VictoryTooltip,
  VictoryVoronoiContainer,
} from 'victory';
import { items } from 'domain-security/data';
import { IeFriendlyVictoryChart } from 'common/components';
import AggregateReportChartLegend from '../AggregateReportChartLegend';

const fill = ['#53ccc2', '#ff9bf6', '#ffbf47', '#93d1ff', '#c4cf81'];
const stroke = ['#002421', '#400038', '#241800', '#001f3f', '#191e00'];

const pointsReducer = (prevAccumulator, previousValues, x) => (
  accumulator,
  value
) => ({
  ...accumulator,
  [value]: [...(prevAccumulator[value] || []), { x, y: previousValues[value] }],
});

const dataReducer = (accumulator, value, key) => ({
  ...accumulator,
  ...reduce(Object.keys(value), pointsReducer(accumulator, value, key), {}),
});

const CustomTooltip = ({ seriesNames, text, ...props }) => (
  <VictoryTooltip
    {...props}
    text={seriesNames
      .map((_, i) => `${items[_].title || _}: ${text[i]}`)
      .reverse()}
  />
);

export default class AggregateReportChart extends Component {
  state = {
    chartData: {},
    seriesNames: [],
  };

  static getDerivedStateFromProps(nextProps) {
    const chartData = reduce(nextProps.data, dataReducer, {});

    return {
      chartData,
      seriesNames: Object.keys(chartData).reverse(),
    };
  }

  render() {
    const { chartData, seriesNames } = this.state;

    return (
      <Grid stackable>
        <Grid.Row columns={2}>
          <Grid.Column width={13} style={{ padding: '0px' }}>
            <div>
              <IeFriendlyVictoryChart
                width={700}
                height={400}
                containerComponent={
                  <VictoryVoronoiContainer
                    voronoiDimension="x"
                    labels={d => `${d.y}`}
                    labelComponent={<CustomTooltip seriesNames={seriesNames} />}
                  />
                }
              >
                <VictoryAxis
                  style={{
                    axis: { strokeWidth: 0 },
                    tickLabels: { fontSize: 12, fontWeight: 500 },
                    grid: { stroke: '#c1c2c4', strokeWidth: 1 },
                  }}
                  tickFormat={t => numeral(t).format('0.0a')}
                  fixLabelOverlap
                  dependentAxis
                />
                <VictoryAxis
                  style={{
                    axis: { stroke: '#c1c2c4' },
                    tickLabels: { fontSize: 12, fontWeight: 500 },
                    grid: { strokeWidth: 0 },
                  }}
                  tickValues={Object.keys(this.props.data)}
                  tickFormat={t => moment(t).format('Do MMM YY')}
                  fixLabelOverlap
                />
                <VictoryStack>
                  {seriesNames.map((key, i) => (
                    <VictoryArea
                      data={chartData[key]}
                      key={key}
                      style={{
                        data: {
                          fill: fill[i],
                          stroke: stroke[i],
                          strokeWidth: 2,
                        },
                      }}
                    />
                  ))}
                </VictoryStack>
              </IeFriendlyVictoryChart>
            </div>
          </Grid.Column>
          <Grid.Column
            width={3}
            verticalAlign="bottom"
            style={{ padding: '0px' }}
          >
            <AggregateReportChartLegend
              data={seriesNames
                .map((_, i) => ({
                  name: items[_].title || _,
                  color: fill[i],
                  stroke: fill[i],
                }))
                .reverse()}
            />
          </Grid.Column>
        </Grid.Row>
      </Grid>
    );
  }
}

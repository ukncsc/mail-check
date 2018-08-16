import React, { Component } from 'react';
import moment from 'moment';
import reduce from 'lodash/reduce';
import numeral from 'numeral';
import {
  VictoryArea,
  VictoryAxis,
  VictoryStack,
  VictoryTooltip,
  VictoryVoronoiContainer,
} from 'victory';
import { IeFriendlyVictoryChart } from 'common/components';

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

const CustomTooltip = ({ descriptions, text, date, datum, ...props }) => (
  <VictoryTooltip
    {...props}
    text={[
      date(datum.x),
      ...descriptions.map(({ title }, i) => `${title}: ${text[i]}`).reverse(),
    ]}
  />
);

const tickLabels = {
  fontSize: 12,
  fontWeight: 500,
  fontFamily: 'Helvetica Neue',
};

export default class AggregateReportChart extends Component {
  state = {
    chartData: {},
    tickValues: [],
  };

  static getDerivedStateFromProps = ({ data }) => ({
    chartData: reduce(data, dataReducer, {}),
    tickValues: Object.keys(data),
  });

  render() {
    const { descriptions } = this.props;
    const { chartData, tickValues } = this.state;

    return (
      <IeFriendlyVictoryChart
        width={700}
        height={400}
        containerComponent={
          <VictoryVoronoiContainer
            voronoiDimension="x"
            labels={d => `${d.y}`}
            labelComponent={
              <CustomTooltip
                descriptions={descriptions}
                date={x => moment(x).format('Do MMMM YYYY')}
              />
            }
          />
        }
      >
        <VictoryAxis
          style={{
            tickLabels,
            axis: { strokeWidth: 0 },
            grid: { stroke: '#c1c2c4', strokeWidth: 1 },
          }}
          tickFormat={t => numeral(t).format('0.0a')}
          fixLabelOverlap
          dependentAxis
        />
        <VictoryAxis
          style={{
            tickLabels,
            axis: { stroke: '#c1c2c4' },
            grid: { strokeWidth: 0 },
          }}
          tickValues={tickValues}
          tickFormat={t => moment(t).format('Do MMM YY')}
          fixLabelOverlap
        />
        <VictoryStack>
          {descriptions.map(({ key, color, stroke }) => (
            <VictoryArea
              data={chartData[key]}
              key={key}
              style={{
                data: { fill: color, stroke, strokeWidth: 2 },
              }}
            />
          ))}
        </VictoryStack>
      </IeFriendlyVictoryChart>
    );
  }
}

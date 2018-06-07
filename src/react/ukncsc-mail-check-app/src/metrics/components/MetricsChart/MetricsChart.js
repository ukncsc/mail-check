import React from 'react';
import moment from 'moment';
import numeral from 'numeral';
import {
  VictoryArea,
  VictoryAxis,
  VictoryChart,
  VictoryGroup,
  VictoryLine,
  VictoryStack,
} from 'victory';

const colourScale = ['#4a9893', '#be8724', '#3772b7'];
export default ({
  data,
  properties,
  dataPoints,
  filled = false,
  stacked = false,
}) => {
  const createXYPlots = property =>
    dataPoints.map(x => ({ x, y: (data[x] && data[x][property]) || 0 }));
  return (
    <VictoryChart>
      <VictoryAxis
        style={{
          tickLabels: { fontSize: 11 },
        }}
        tickFormat={t => numeral(t).format('0.0a')}
        fixLabelOverlap
        dependentAxis
      />
      <VictoryAxis
        style={{
          axisLabel: { fontSize: 8 },
          tickLabels: { fontSize: 11 },
        }}
        tickValues={dataPoints}
        tickFormat={t => moment(t).format('Do MMM YY')}
        label="Week Commencing"
        fixLabelOverlap
      />
      {stacked ? (
        <VictoryStack colorScale={colourScale}>
          {properties.map(property => (
            <VictoryArea key={property} data={createXYPlots(property)} />
          ))}
        </VictoryStack>
      ) : (
        <VictoryGroup colorScale={colourScale}>
          {filled
            ? properties.map(property => (
                <VictoryArea key={property} data={createXYPlots(property)} />
              ))
            : properties.map(property => (
                <VictoryLine key={property} data={createXYPlots(property)} />
              ))}
        </VictoryGroup>
      )}
    </VictoryChart>
  );
};

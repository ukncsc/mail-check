import React from 'react';

import AggregateReportChartLegendItem from '../AggregateReportChartLegendItem';

import './AggregateReportChartLegend.css';

export default ({ data }) => (
  <ul className="ChartLegend">
    {data.map(_ => (
      <li key={_.name}>
        <AggregateReportChartLegendItem {..._} />
      </li>
    ))}
  </ul>
);

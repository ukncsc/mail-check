import React from 'react';
import { Header } from 'semantic-ui-react';
import './AggregateReportChartLegendItem.css';

export default ({ name, color, stroke }) => (
  <React.Fragment>
    <Header as="h2" className="ChartLegendHeader">
      -
    </Header>
    <div
      className="ChartLegendMarker"
      style={{
        background: color,
        stroke,
      }}
    />
    {name}
  </React.Fragment>
);

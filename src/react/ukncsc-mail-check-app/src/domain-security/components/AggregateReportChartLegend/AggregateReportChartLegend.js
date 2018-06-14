import React from 'react';
import { Header } from 'semantic-ui-react';

import './AggregateReportChartLegend.css';

export default ({ children }) => (
  <ul className="ChartLegend">
    {children.map(({ name, background, stroke }) => (
      <li key={name}>
        <Header as="h2" className="ChartLegendHeader">
          -
        </Header>
        <div className="ChartLegendMarker" style={{ background, stroke }} />
        {name}
      </li>
    ))}
  </ul>
);

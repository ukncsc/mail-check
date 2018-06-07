import React from 'react';
import { Header } from 'semantic-ui-react';
import numeral from 'numeral';

import './AggregateReportStatistic.css';

export default ({ title, value, period }) => (
  <div className="Statistic">
    <Header as="h3" style={{ margin: 0 }} className="StatisticTitle">
      {title}
    </Header>
    <Header className="StatisticValue" style={{ margin: 0 }} as="h1">
      {numeral(value).format('0.0a')}
    </Header>
    <p className="StatisticPeriod">{period}</p>
  </div>
);

import React from 'react';
import { render } from 'react-testing-library';
import AggregateReportChartLegend from './AggregateReportChartLegend';

describe('AggregateReportChartLegend', () => {
  let container;

  describe('when getting a chart legend', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportChartLegend>
          {[
            { name: 'item 1', background: '#fff000', stroke: '#ffffff' },
            { name: 'item 2', background: '#ffff00', stroke: '#fffff0' },
          ]}
        </AggregateReportChartLegend>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

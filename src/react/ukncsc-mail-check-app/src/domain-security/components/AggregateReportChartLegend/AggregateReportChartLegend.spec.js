import React from 'react';
import { render } from 'react-testing-library';
import AggregateReportChartLegend from './AggregateReportChartLegend';

describe('AggregateReportChartLegend', () => {
  let container;

  describe('when getting a chart legend', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportChartLegend
          data={[
            { name: 'item 1', color: '#fff000', stroke: '#ffffff' },
            { name: 'item 2', color: '#ffff00', stroke: '#fffff0' },
          ]}
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

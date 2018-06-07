import React from 'react';
import { render } from 'react-testing-library';
import AggregateReportChartLegendItem from './AggregateReportChartLegendItem';

describe('AggregateReportChartLegendItem', () => {
  let container;

  describe('when getting a chart legend item', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportChartLegendItem
          name="legend item"
          color="#ffffff"
          stoke="#fff000"
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

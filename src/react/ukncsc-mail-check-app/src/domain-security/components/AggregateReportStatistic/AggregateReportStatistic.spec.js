import React from 'react';
import { render } from 'react-testing-library';
import AggregateReportStatistic from './AggregateReportStatistic';

describe('AggregateReportStatistic', () => {
  let container;

  describe('when getting an aggregate report statistic', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportStatistic
          title="stat"
          value="2000"
          period="last week"
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

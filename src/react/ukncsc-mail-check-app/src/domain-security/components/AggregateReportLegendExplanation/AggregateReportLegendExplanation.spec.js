import React from 'react';
import { render } from 'react-testing-library';
import AggregateReportLegendExplanation from './AggregateReportLegendExplanation';

describe('AggregateReportLegendExplanation', () => {
  let container;

  describe('when getting a aggregate report legend explanation', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportLegendExplanation
          title="Title"
          description="Explanation"
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

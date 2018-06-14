import React from 'react';
import { render } from 'react-testing-library';
import { graphDescriptions } from 'domain-security/data';
import AggregateReportChart from './AggregateReportChart';

describe('AggregateReportChart', () => {
  let container;

  describe('when getting an aggregate report chart', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportChart
          descriptions={graphDescriptions}
          data={{
            '2017-12-10T00:00:00': {
              fullyTrusted: 1,
              partiallyTrusted: 2,
              untrusted: 3,
              quarantined: 4,
              rejected: 5,
            },
            '2017-12-11T00:00:00': {
              fullyTrusted: 6,
              partiallyTrusted: 7,
              untrusted: 8,
              quarantined: 9,
              rejected: 10,
            },
            '2017-12-12T00:00:00': {
              fullyTrusted: 11,
              partiallyTrusted: 12,
              untrusted: 13,
              quarantined: 14,
              rejected: 15,
            },
          }}
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

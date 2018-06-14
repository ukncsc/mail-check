import React from 'react';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';

import AggregateReportSummary from './AggregateReportSummary';

describe('AggregateReportSummary', () => {
  let container;

  describe('when no data returned', () => {
    beforeEach(() => {
      ({ container } = render(<AggregateReportSummary />));
    });

    test('it should show info message', () =>
      expect(
        container.getElementsByClassName('ui info message')[0]
      ).toHaveTextContent(
        'Currently no aggregate report information to show for domain.'
      ));
  });

  describe('when user doesnt have permission', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportSummary error="403 Forbidden" />
      ));
    });

    test('it should show info message', () =>
      expect(
        container.getElementsByClassName('ui info message')[0]
      ).toHaveTextContent(
        'You do have permission to view aggregate reporting for this domain.'
      ));
  });

  describe('when an error occurs', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportSummary error="404 Not found" />
      ));
    });

    test('it should show error message', () =>
      expect(
        container.getElementsByClassName('ui error message')[0]
      ).toHaveTextContent(
        'There was a problem retrieving aggregate report information for this domain: '
      ));
  });

  describe('when successfully retrieves data', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportSummary
          data={{
            results: {
              '2017-12-10T00:00:00': {
                fullyTrusted: 4,
                partiallyTrusted: 6,
                untrusted: 0,
                quarantined: 0,
                rejected: 0,
              },
              '2017-12-11T00:00:00': {
                fullyTrusted: 5,
                partiallyTrusted: 8,
                untrusted: 0,
                quarantined: 0,
                rejected: 0,
              },
              '2017-12-12T00:00:00': {
                fullyTrusted: 6,
                partiallyTrusted: 7,
                untrusted: 0,
                quarantined: 0,
                rejected: 0,
              },
            },
            totalEmail: 178,
          }}
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

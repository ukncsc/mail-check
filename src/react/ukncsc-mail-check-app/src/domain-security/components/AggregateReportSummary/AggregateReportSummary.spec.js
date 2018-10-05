import React from 'react';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';

import { AggregateReportSummary } from 'domain-security/components';

const fetchDomainSecurityAggregateData = jest.fn();
const getDomainSecurityAggregateData = jest.fn();

describe('AggregateReportSummary', () => {
  let container;

  afterEach(() => jest.resetAllMocks());

  describe('when no data returned', () => {
    beforeEach(() => {
      ({ container } = render(
        <AggregateReportSummary
          domainId={123}
          fetchDomainSecurityAggregateData={fetchDomainSecurityAggregateData}
          getDomainSecurityAggregateData={getDomainSecurityAggregateData}
        />
      ));
    });

    test('it should show info message', () =>
      expect(
        container.getElementsByClassName('ui info message')[0]
      ).toHaveTextContent(
        'Currently no aggregate report information to show for the selected date range.'
      ));
  });

  describe('when user doesnt have permission', () => {
    beforeEach(() => {
      getDomainSecurityAggregateData.mockImplementation(() => ({
        error: { message: '403 Forbidden' },
      }));

      ({ container } = render(
        <AggregateReportSummary
          domainId={123}
          fetchDomainSecurityAggregateData={fetchDomainSecurityAggregateData}
          getDomainSecurityAggregateData={getDomainSecurityAggregateData}
        />
      ));
    });

    test('it should show info message', () =>
      expect(
        container.getElementsByClassName('ui info message')[0]
      ).toHaveTextContent(
        'You do not have permission to view aggregate reporting for this domain.'
      ));
  });

  describe('when an error occurs', () => {
    beforeEach(() => {
      getDomainSecurityAggregateData.mockImplementation(() => ({
        error: { message: '404 Not found' },
      }));

      ({ container } = render(
        <AggregateReportSummary
          domainId={123}
          fetchDomainSecurityAggregateData={fetchDomainSecurityAggregateData}
          getDomainSecurityAggregateData={getDomainSecurityAggregateData}
        />
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
      getDomainSecurityAggregateData.mockImplementation(() => ({
        data: {
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
        },
      }));

      ({ container } = render(
        <AggregateReportSummary
          domainId={123}
          fetchDomainSecurityAggregateData={fetchDomainSecurityAggregateData}
          getDomainSecurityAggregateData={getDomainSecurityAggregateData}
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

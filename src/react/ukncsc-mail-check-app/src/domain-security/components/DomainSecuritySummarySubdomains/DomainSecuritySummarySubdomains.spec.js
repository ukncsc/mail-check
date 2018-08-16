import React from 'react';
import { MemoryRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';

import { DomainSecuritySummarySubdomains } from 'domain-security/components';

const fetchDomainSecuritySubdomains = jest.fn();
const getDomainSecuritySubdomains = jest.fn();

describe('DomainSecuritySubdomains', () => {
  let container;

  afterEach(() => jest.resetAllMocks());

  describe('when there is empty subdomains', () => {
    beforeEach(() => {
      getDomainSecuritySubdomains.mockImplementation(() => ({
        loading: false,
        noMoreResults: true,
        noSubdomains: false,
        subdomains: [],
      }));

      ({ container } = render(
        <MemoryRouter>
          <DomainSecuritySummarySubdomains
            domainName="ncsc.gov.uk"
            fetchDomainSecuritySubdomains={fetchDomainSecuritySubdomains}
            getDomainSecuritySubdomains={getDomainSecuritySubdomains}
          />
        </MemoryRouter>
      ));
    });

    test('it should not display a show more link', () =>
      expect(container).not.toHaveTextContent('Show more'));

    test('it should not display anything in the list', () =>
      expect(
        container.getElementsByClassName('eight wide column')[0].firstChild
      ).toBeNull());
  });

  describe('when there are no subdomains', () => {
    beforeEach(() => {
      getDomainSecuritySubdomains.mockImplementation(() => ({
        loading: false,
        noMoreResults: true,
        noSubdomains: true,
        subdomains: [],
      }));

      ({ container } = render(
        <MemoryRouter>
          <DomainSecuritySummarySubdomains
            domainName="ncsc.gov.uk"
            fetchDomainSecuritySubdomains={fetchDomainSecuritySubdomains}
            getDomainSecuritySubdomains={getDomainSecuritySubdomains}
          />
        </MemoryRouter>
      ));
    });

    test('it should not display a show more link', () =>
      expect(container).not.toHaveTextContent('Show more'));

    test('it should display an informative message', () =>
      expect(container).not.toBeNull());

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when there are subdomains with more results', () => {
    beforeEach(() => {
      getDomainSecuritySubdomains.mockImplementation(() => ({
        loading: false,
        noMoreResults: false,
        subdomains: [{ id: 123, domainName: 'ncsc.gov.uk' }],
      }));

      ({ container } = render(
        <MemoryRouter>
          <DomainSecuritySummarySubdomains
            domainName="ncsc.gov.uk"
            fetchDomainSecuritySubdomains={fetchDomainSecuritySubdomains}
            getDomainSecuritySubdomains={getDomainSecuritySubdomains}
          />
        </MemoryRouter>
      ));
    });

    test('it should display a show more link', () =>
      expect(container).toHaveTextContent('Show more'));

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when there are subdomains with no more results', () => {
    beforeEach(() => {
      getDomainSecuritySubdomains.mockImplementation(() => ({
        loading: false,
        noMoreResults: true,
        subdomains: [{ id: 123, domainName: 'ncsc.gov.uk' }],
      }));

      ({ container } = render(
        <MemoryRouter>
          <DomainSecuritySummarySubdomains
            domainName="ncsc.gov.uk"
            fetchDomainSecuritySubdomains={fetchDomainSecuritySubdomains}
            getDomainSecuritySubdomains={getDomainSecuritySubdomains}
          />
        </MemoryRouter>
      ));
    });

    test('it should not display a show more link', () =>
      expect(container).not.toHaveTextContent('Show more'));

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

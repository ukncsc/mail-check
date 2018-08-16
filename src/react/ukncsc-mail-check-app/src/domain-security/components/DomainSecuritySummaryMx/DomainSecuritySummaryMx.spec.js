import React from 'react';
import { MemoryRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';
import DomainSecuritySummaryMx from './DomainSecuritySummaryMx';

const fetchDomainSecurityMx = jest.fn();
const getDomainSecurityMx = jest.fn();

describe('DomainSecuritySummaryMx', () => {
  let container;

  afterEach(() => jest.resetAllMocks());

  describe('when no records are provided', () => {
    beforeEach(() => {
      getDomainSecurityMx.mockImplementation(() => ({ records: [] }));

      ({ container } = render(
        <MemoryRouter>
          <DomainSecuritySummaryMx
            description=""
            domainId={123}
            fetchDomainSecurityMx={fetchDomainSecurityMx}
            getDomainSecurityMx={getDomainSecurityMx}
            type="TLS"
          />
        </MemoryRouter>
      ));
    });

    test('it should display a success message', () =>
      expect(
        container.getElementsByClassName('ui success message')[0]
      ).toHaveTextContent('No MX records found.'));

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when no failures, warnings or inconclusives are present', () => {
    beforeEach(() => {
      getDomainSecurityMx.mockImplementation(() => ({
        records: [
          {
            id: 1,
            hostname: '1.com',
            failures: [],
            warnings: [],
            inconclusives: [],
          },
        ],
      }));

      ({ container } = render(
        <MemoryRouter>
          <DomainSecuritySummaryMx
            description=""
            domainId={123}
            fetchDomainSecurityMx={fetchDomainSecurityMx}
            getDomainSecurityMx={getDomainSecurityMx}
            type="TLS"
          />
        </MemoryRouter>
      ));
    });

    test('it should display a success message', () =>
      expect(
        container.getElementsByClassName('ui success message')[0]
      ).toHaveTextContent('TLS well configured.'));

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when an error is present', () => {
    beforeEach(() => {
      getDomainSecurityMx.mockImplementation(() => ({
        error: { message: 'Oh noes!' },
      }));

      ({ container } = render(
        <MemoryRouter>
          <DomainSecuritySummaryMx
            description=""
            domainId={123}
            fetchDomainSecurityMx={fetchDomainSecurityMx}
            getDomainSecurityMx={getDomainSecurityMx}
            type="TLS"
          />
        </MemoryRouter>
      ));
    });

    test('it should display a message with the error', () =>
      expect(
        container.getElementsByClassName('ui error message')[0]
      ).toHaveTextContent('oh noes!'));

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';
import DomainSecurityTlsSummary from './DomainSecurityTlsSummary';

describe('DomainSecurityTlsSummary', () => {
  let container;

  describe('when no records are provided', () => {
    beforeEach(() => {
      ({ container } = render(
        <BrowserRouter>
          <DomainSecurityTlsSummary records={[]}>
            <p>foo</p>
          </DomainSecurityTlsSummary>
        </BrowserRouter>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should display a success message', () =>
      expect(
        container.getElementsByClassName('ui success message')[0]
      ).toHaveTextContent('No MX records found.'));
  });

  describe('when no failures, warnings or inconclusives are present', () => {
    beforeEach(() => {
      ({ container } = render(
        <BrowserRouter>
          <DomainSecurityTlsSummary
            records={[
              {
                id: '1',
                hostname: '1.com',
                failures: [],
                warnings: [],
                inconclusives: [],
              },
            ]}
          >
            <p>bar</p>
          </DomainSecurityTlsSummary>
        </BrowserRouter>
      ));
    });

    test('it should display a success message', () =>
      expect(
        container.getElementsByClassName('ui success message')[0]
      ).toHaveTextContent('TLS is well configured.'));
  });

  describe('when an error is present', () => {
    beforeEach(() => {
      ({ container } = render(
        <BrowserRouter>
          <DomainSecurityTlsSummary error={Error('oh noes!')}>
            <p>flum</p>
          </DomainSecurityTlsSummary>
        </BrowserRouter>
      ));
    });

    test('it should display a message with the error', () =>
      expect(
        container.getElementsByClassName('ui error message')[0]
      ).toHaveTextContent('oh noes!'));
  });
});

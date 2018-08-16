import React from 'react';
import { render } from 'react-testing-library';
import 'dom-testing-library/extend-expect';
import WelcomeSearchMessages from './WelcomeSearchMessages';

describe('WelcomeSearchMessages', () => {
  let container;

  describe('when loading', () => {
    beforeEach(() => {
      ({ container } = render(<WelcomeSearchMessages loading />));
    });

    test('it should render nothing', () =>
      expect(container.firstChild).toBeNull());
  });

  describe('when no search has happened', () => {
    beforeEach(() => {
      ({ container } = render(<WelcomeSearchMessages hasSearched={false} />));
    });

    test('it should render nothing', () =>
      expect(container.firstChild).toBeNull());
  });

  describe('when an error occurs', () => {
    beforeEach(() => {
      ({ container } = render(
        <WelcomeSearchMessages
          error={{ message: 'something has gone wrong' }}
          hasSearched
          loading={false}
        />
      ));
    });

    test('it should render the error message', () =>
      expect(container.firstChild).toHaveTextContent(
        'something has gone wrong'
      ));

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when there is no result for a public sector domain', () => {
    beforeEach(() => {
      ({ container } = render(
        <WelcomeSearchMessages
          hasSearched
          lastSearchTerm="foo.gov.uk"
          loading={false}
          searchResult={null}
          searchTermIsPublicSectorOrg
        />
      ));
    });

    test('it should render the please add to mail check message', () =>
      expect(container.firstChild).toHaveTextContent(
        'foo.gov.uk is not currently monitored by Mail Check. Add foo.gov.uk to Mail Check.'
      ));

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when is no result for a non public sector domain', () => {
    beforeEach(() => {
      ({ container } = render(
        <WelcomeSearchMessages
          hasSearched
          loading={false}
          lastSearchTerm="foo.com"
          searchResult={null}
        />
      ));
    });

    test('it should render please contact mail check message', () =>
      expect(container.firstChild).toHaveTextContent(
        'foo.com is not currently monitored by Mail Check. Please email mailcheck@digital.ncsc.gov.uk to have foo.com added to Mail Check.'
      ));

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

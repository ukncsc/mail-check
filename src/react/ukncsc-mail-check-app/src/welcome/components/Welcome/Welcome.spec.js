import React from 'react';
import { render } from 'react-testing-library';
import { MemoryRouter } from 'react-router-dom';
import Welcome from './Welcome';

describe('Welcome', () => {
  let container;

  describe('when displayed for the first time', () => {
    beforeEach(() => {
      ({ container } = render(
        <MemoryRouter>
          <Welcome
            lastSearchTerm=""
            resetWelcomeSearch={jest.fn()}
            searchResult={null}
            searchTerm=""
          />
        </MemoryRouter>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });

  describe('when the user has searched', () => {
    beforeEach(() => {
      ({ container } = render(
        <MemoryRouter>
          <Welcome
            hasSearched
            lastSearchTerm="foo.gov.uk"
            resetWelcomeSearch={jest.fn()}
            searchResult={{ id: 123, domainName: 'foo.gov.uk' }}
            searchTerm="foo.gov.uk"
            searchTermIsPublicSectorOrg
          />
        </MemoryRouter>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

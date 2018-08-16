import React from 'react';
import { MemoryRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import BackLink from './BackLink';

describe('BackLink', () => {
  let container;

  describe('when rendered with a link', () => {
    beforeEach(() => {
      ({ container } = render(
        <MemoryRouter>
          <BackLink link="/bleh" />
        </MemoryRouter>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should pass the provided link to the Link component', () =>
      expect(container.querySelector('a').getAttribute('href')).toEqual(
        '/bleh'
      ));
  });

  describe('when rendered without a link', () => {
    beforeEach(() => {
      ({ container } = render(
        <MemoryRouter>
          <BackLink />
        </MemoryRouter>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

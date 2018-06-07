import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import { render } from 'react-testing-library';
import BackLink from './BackLink';

describe('BackLink', () => {
  let container;

  describe('when rendered with a link', () => {
    beforeEach(() => {
      ({ container } = render(
        <BrowserRouter>
          <BackLink link="/bleh" />
        </BrowserRouter>
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
        <BrowserRouter>
          <BackLink />
        </BrowserRouter>
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());
  });
});

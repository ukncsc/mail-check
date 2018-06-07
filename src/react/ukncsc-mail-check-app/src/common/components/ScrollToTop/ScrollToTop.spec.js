import React from 'react';
import { render } from 'react-testing-library';
import ScrollToTop from './ScrollToTop';

describe('ScrollToTop', () => {
  let container;

  beforeEach(() => {
    ({ container } = render(
      <ScrollToTop>
        <p>some content</p>
      </ScrollToTop>
    ));
  });

  test('it should match the snapshot', () =>
    expect(container).toMatchSnapshot());
});
